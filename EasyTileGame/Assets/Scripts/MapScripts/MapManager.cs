using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tileParent; // 해당 오브젝트 내 자식오브젝트. 활성화 타일 오브젝트들은 해당 오브젝트의 자식임
    [SerializeField] private GameObject tileParentFalse; // 해당 오브젝트 내 자식오브젝트. 비활성화 타일 오브젝트들은 해당 오브젝트의 자식임
    [SerializeField] private GameObject playerObj;  // 맵 생성 후 플레이어를 부르기 위함
    [SerializeField] private GameObject coordObj;   // 좌표를 탐지하는 게임오브젝트
    [SerializeField] private GameObject temObj; // 임시로 자식들을 보관하는 게임오브젝트
    [SerializeField] private GameObject InGameUIObj;  // 플레이어의 마력 수치를 보여주는 게임오브젝트

    private Transform tileParentTrans;  // tileParent의 transform
    private Transform tileParentFalseTrans; // tileParentFalse의 transform
    private Transform tilePrefabTrans;  // 사전에 저장해둔 tile 오브젝트들을 자식으로 둔 오브젝트의 transform
    private Transform mapTrans; // 해당 오브젝트의 트랜스폼

    private WaitForSeconds term = new WaitForSeconds(0.05f);
    private Coroutine firstCo;  // 시작 시, 초기 타일맵 생성을 담당하는 코루틴
    private Coroutine playerCo;
    private Coroutine lateCo;   // 플레이 도중 타일 갱신을 담당하는 코루틴

    private Dictionary<string, Sprite> tileSprDic = new Dictionary<string, Sprite>();
    private List<Type> tileEleList = new List<Type>();

    private Vector3 startPos;

    private bool isMakeMap = false; // 맵생성 함수가 호출되었는지 여부. true라면 더 호출할 수 없음

    void Awake()
    {
        mapTrans = this.transform;
        tileParentTrans = tileParent.transform;
        tileParentFalseTrans = tileParentFalse.transform;

        startPos = mapTrans.position;

        firstCo = null;
        playerCo = null;
        tileEleList = FindSubClass.FindSubclassOf<TileAttribute>();
        
    }

    // 준비된 오브젝트들과 주소를 통해 맵을 만듦
    public void MakeMap()
    {
		if (isMakeMap) { return;}
		isMakeMap = true;
        // GetResourceOrder함수는 원하는 리소스를 ResourceLoadingScript에 요구하는 기능임
        tilePrefabTrans = ResourceLoadingScript.Instance.GetResourceOrder(0).transform;
        tileSprDic = ResourceLoadingScript.Instance.GetResourceOrder(0).GetSprite();

		// 오브젝트들을 가져옴
		while (tilePrefabTrans.childCount > 0)
        {
            tilePrefabTrans.GetChild(0).parent = tileParentFalseTrans;
        }

        // 코루틴 호출 -> 타일을 텀을 두고 순차적으로 생성하기 위함
		firstCo = StartCoroutine(SettingFirstTiles());
    }
    // 처음에 등장해야되는 타일 150개를 활성화시키는 코루틴임
    IEnumerator SettingFirstTiles()
    {
        int indX = 0;
        int indY = 0;

        // 대각방향 줄 개수 : 18개
        for (int i = 0; i < 19; i++)
        {
            if (i < 10) { indX = i; indY = 0; }
            else { indX = 9; indY = i - indX; }

            // 대각선 방향에 있는 타일 개수 : 0 ~ 10개
            for (int j = 0; j < (indX - indY) + 1; j++)
            {
                if (tileParentFalseTrans.childCount > 0)
                {
                    GameObject ob = tileParentFalseTrans.GetChild(0).gameObject;

                    ob.SetActive(true);
                    ob.transform.localPosition = new Vector3(72 - ((indX - j) * 16), -72 + (((indY + j)) * 16), 0f);
                    ob.transform.parent = tileParentTrans;
                    SettingPrefab(ob);

				}
            }
			yield return term;
		}

        // 마력 UI를 킴
        InGameUIObj.SetActive(true);

        // 화면 밖에 여분의 타일을 생성
        for (int i = 0; i < 10; i++)
        {
            for (int j = 10; j < 15; j++)
            {
                GameObject ob = tileParentFalseTrans.GetChild(0).gameObject;

                ob.SetActive(true);
                ob.transform.localPosition = new Vector3(72 - (i * 16), -72 + (j * 16), 0f);
				ob.transform.parent = tileParentTrans;
				SettingPrefab(ob);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            yield return term;
        }

        playerCo = StartCoroutine(ActivePlayer());

        // 이 코루틴을 종료함
        StopCoroutine(firstCo);

        yield return null;
    }
    // 플레이어가 이동함에 따라 새로이 타일을 갱신하는 코루틴
    IEnumerator SettingLateTiles()
    {
        // 한 행을 구성하는 타일의 개수
        int indX = Constant.TILEXCOUNT;

        for (int i = 0; i < indX; i++)
        {
            // 여분의 타일이 준비되어 있을 경우에 발동
            if (tileParentFalseTrans.childCount > 0)
            {
                // 여분의 타일을 가져옴
                GameObject ob = tileParentFalseTrans.GetChild(0).gameObject;
                // 타일 활성화
                ob.SetActive(true);
                // 타일의 위치를 잡아줌
                ob.transform.position = new Vector3(i * Constant.TILESIZE - 72f, (15f - 1f) * Constant.TILESIZE - 72f, 0f);
                // 타일의 부모 오브젝트를 설정해줌
                ob.transform.parent = tileParentTrans;
                // 타일에 필요한 컴포넌트와 이미지를 부착함
                SettingPrefab(ob);
            }
        }

        yield return term;

        StopCoroutine(lateCo);

        yield return null;
    }

    // 생성한 타일에 알맞는 컴포넌트를 부착함. 컴포넌트에는 타일에 대한 속성과 그에 따른 규칙이 담김
    private void SettingPrefab(GameObject obj)
    {
        if (obj.GetComponent<TileAttribute>() != null)
        {
            obj.GetComponent<TileAttribute>().InitSprite();
            return;
        }

        obj.AddComponent(tileEleList[Random.Range(0, tileEleList.Count)]);

        string oS = obj.GetComponent<TileAttribute>().strObj;
        string eS = obj.GetComponent<TileAttribute>().strEle;

        Sprite s0 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_0")).Value;
        Sprite s1 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_1")).Value;

        obj.GetComponent<TileAttribute>().InitSprite(s0, s1);
	}

    IEnumerator ActivePlayer()
    {
        // 텀을 두지 않으면 코루틴을 반환하기도 전에 종료되서 playerCo가 null이 됨
        yield return term;

        //----------------------------좌표 탐지기 활성화--------------------------------------------
        coordObj.SetActive(true);

        GameObject obX = coordObj.transform.GetChild(0).gameObject;
        GameObject obY = coordObj.transform.GetChild(1).gameObject;

        for (int i = 0; i < Constant.TILEXCOUNT; i++)
        {
            obX.transform.GetChild(i).GetComponent<CheckCoordinate>().SettingCoord(i, -1);
        }

        for (int i = 0; i < Constant.TILEYCOUNT; i++)
        {
            obY.transform.GetChild(i).GetComponent<CheckCoordinate>().SettingCoord(-1, i);
        }
        //-------------------------------------------------------------------------------------------

        playerObj.SetActive(true);
        playerObj.transform.localPosition = new Vector3(8f, -((float)Constant.TILESIZE * 3f), 0f);;

        StopCoroutine(playerCo);

        yield return null;
    }

    // 맵을 구성하는 오브젝트들을 이동시켜 플레이어가 나아가는 듯한 연출을 주기 위한 함수
    public void UpdateMapPosition(Vector3 move, bool isUpdate)
    {
        // 아래로 내리기만 하면 되므로, y값만 사용함
        mapTrans.position = startPos - new Vector3(0f, move.y);
        // 오브젝트의 position이 끝 없이 내려가는 것을 방지하기 위해, position 재설정
        if (isUpdate)
        {
            // 자식 오브젝트가 따라서 움직이지 않도록 다른 오브젝트 자식으로 임시 보관
            while (mapTrans.childCount > 0)
            {
                mapTrans.GetChild(0).parent = temObj.transform;
            }
            // y값이 내려간 오브젝트의 position 재설정
            mapTrans.position = Vector3.zero;
            // 임시 보관 오브젝트에서 다시 자식들을 복귀시킴
            while (temObj.transform.childCount > 0)
            {
                temObj.transform.GetChild(0).parent = mapTrans;
            }

            lateCo = StartCoroutine(SettingLateTiles());
        }
    }
}
