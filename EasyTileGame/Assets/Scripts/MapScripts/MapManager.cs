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

    private Transform tileParentTrans;  // tileParent의 transform
    private Transform tileParentFalseTrans; // tileParentFalse의 transform
    private Transform tilePrefabTrans;  // 사전에 저장해둔 tile 오브젝트들을 자식으로 둔 오브젝트의 transform

    private WaitForSeconds term = new WaitForSeconds(0.05f);
    private Coroutine firstCo;

    private Dictionary<string, Sprite> tileSprDic = new Dictionary<string, Sprite>();
    private List<Type> tileEleList = new List<Type>();

    private bool isMakeMap = false; // 맵생성 함수가 호출되었는지 여부. true라면 더 호출할 수 없음

    void Awake()
    {
        tileParentTrans = tileParent.transform;
        tileParentFalseTrans = tileParentFalse.transform;

        firstCo = null;
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

        StopCoroutine(firstCo);

        yield return null;
    }

    private void SettingPrefab(GameObject obj)
    {
        obj.AddComponent(tileEleList[Random.Range(0, tileEleList.Count)]);

        string oS = obj.GetComponent<TileAttribute>().strObj;
        string eS = obj.GetComponent<TileAttribute>().strEle;

        Sprite s0 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_0")).Value;
        Sprite s1 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_1")).Value;

        obj.GetComponent<TileAttribute>().InitSprite(s0, s1);
	}
}
