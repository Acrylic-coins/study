using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tileParent; // �ش� ������Ʈ �� �ڽĿ�����Ʈ. Ÿ�� ������Ʈ���� �ش� ������Ʈ�� �ڽ���

    private Transform tileParentTrans;  // tileParent�� transform
    private Transform tilePrefabTrans;  // ������ �����ص� tile ������Ʈ���� �ڽ����� �� ������Ʈ�� transform

    private bool isMakeMap = false; // �ʻ��� �Լ��� ȣ��Ǿ����� ����. true��� �� ȣ���� �� ����

    void Awake()
    {
        tileParentTrans = tileParent.transform;
    }

    // �غ�� ������Ʈ��� �ּҸ� ���� ���� ����
    public void MakeMap()
    {
		if (isMakeMap) { return;}
		isMakeMap = true;
        // GetResourceOrder�Լ��� ���ϴ� ���ҽ��� ResourceLoadingScript�� �䱸�ϴ� �����
        tilePrefabTrans = ResourceLoadingScript.Instance.GetResourceOrder(0).transform;

        int x = 72;
        int y = -72;

        // ������Ʈ���� ������
        while(tilePrefabTrans.childCount > 0)
        {
            tilePrefabTrans.GetChild(0).parent = tileParentTrans;
        }

        for (int i = 0; i < 10; i++)
        {
            for (int k = 0; k < 15; k++)
            {
				int index = 0;
				for (int j = 0; j < tileParentTrans.childCount; j++)
				{
					if (!tileParentTrans.GetChild(j).gameObject.activeSelf)
					{
						index = j;
						break;
					}
				}
                tileParentTrans.GetChild(index).gameObject.SetActive(true);
				tileParentTrans.GetChild(index).localPosition = new Vector3(72 - (i * 16), -72 + (k * 16), 0f);
			}
		}

    }
}
