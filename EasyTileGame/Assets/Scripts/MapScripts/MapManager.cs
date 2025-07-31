using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tileParent; // 해당 오브젝트 내 자식오브젝트. 활성화 타일 오브젝트들은 해당 오브젝트의 자식임
    [SerializeField] private GameObject tileParentFalse; // 해당 오브젝트 내 자식오브젝트. 비활성화 타일 오브젝트들은 해당 오브젝트의 자식임

    private Transform tileParentTrans;  // tileParent의 transform
    private Transform tileParentFalseTrans; // tileParentFalse의 transform
    private Transform tilePrefabTrans;  // 사전에 저장해둔 tile 오브젝트들을 자식으로 둔 오브젝트의 transform

    private bool isMakeMap = false; // 맵생성 함수가 호출되었는지 여부. true라면 더 호출할 수 없음

    void Awake()
    {
        tileParentTrans = tileParent.transform;
        tileParentFalseTrans = tileParentFalse.transform;
    }

    // 준비된 오브젝트들과 주소를 통해 맵을 만듦
    public void MakeMap()
    {
		if (isMakeMap) { return;}
		isMakeMap = true;
        // GetResourceOrder함수는 원하는 리소스를 ResourceLoadingScript에 요구하는 기능임
        tilePrefabTrans = ResourceLoadingScript.Instance.GetResourceOrder(0).transform;

        int x = 72;
        int y = -72;

        // 오브젝트들을 가져옴
        while(tilePrefabTrans.childCount > 0)
        {
            tilePrefabTrans.GetChild(0).parent = tileParentFalseTrans;
        }

        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 15; k++)
            {
                if (tileParentFalseTrans.childCount > 0)
                {
					tileParentFalseTrans.GetChild(0).gameObject.SetActive(true);
					tileParentFalseTrans.GetChild(0).localPosition = new Vector3(72 - (i * 16), -72 + (k * 16), 0f);
                    tileParentFalseTrans.GetChild(0).parent = tileParentTrans;
				}
			}
		}

    }
}
