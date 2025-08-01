using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tileParent; // �ش� ������Ʈ �� �ڽĿ�����Ʈ. Ȱ��ȭ Ÿ�� ������Ʈ���� �ش� ������Ʈ�� �ڽ���
    [SerializeField] private GameObject tileParentFalse; // �ش� ������Ʈ �� �ڽĿ�����Ʈ. ��Ȱ��ȭ Ÿ�� ������Ʈ���� �ش� ������Ʈ�� �ڽ���

    private Transform tileParentTrans;  // tileParent�� transform
    private Transform tileParentFalseTrans; // tileParentFalse�� transform
    private Transform tilePrefabTrans;  // ������ �����ص� tile ������Ʈ���� �ڽ����� �� ������Ʈ�� transform

    private WaitForSeconds term = new WaitForSeconds(0.05f);
    private Coroutine firstCo;

    private bool isMakeMap = false; // �ʻ��� �Լ��� ȣ��Ǿ����� ����. true��� �� ȣ���� �� ����

    void Awake()
    {
        tileParentTrans = tileParent.transform;
        tileParentFalseTrans = tileParentFalse.transform;

        firstCo = null;
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
            tilePrefabTrans.GetChild(0).parent = tileParentFalseTrans;
        }

        // �ڷ�ƾ ȣ�� -> Ÿ���� ���� �ΰ� ���������� �����ϱ� ����
		firstCo = StartCoroutine(SettingFirstTiles());

    }

    IEnumerator SettingFirstTiles()
    {
        int indX = 0;
        int indY = 0;

        // �밢���� �� ���� : 18��
        for (int i = 0; i < 19; i++)
        {
            if (i < 10) { indX = i; indY = 0; }
            else { indX = 9; indY = i - indX; }

            // �밢�� ���⿡ �ִ� Ÿ�� ���� : 0 ~ 10��
            for (int j = 0; j < (indX - indY) + 1; j++)
            {
                if (tileParentFalseTrans.childCount > 0)
                {
                    tileParentFalseTrans.GetChild(0).gameObject.SetActive(true);
                    tileParentFalseTrans.GetChild(0).localPosition = new Vector3(72 - ((indX - j) * 16), -72 + (((indY + j)) * 16), 0f);
                    tileParentFalseTrans.GetChild(0).parent = tileParentTrans;
                }

            }
			yield return term;
		}

        // ȭ�� �ۿ� ������ Ÿ���� ����
        for (int i = 0; i < 10; i++)
        {
            for (int j = 10; j < 15; j++)
            {
                tileParentFalseTrans.GetChild(0).gameObject.SetActive(true);
                tileParentFalseTrans.GetChild(0).localPosition = new Vector3(72 - (i * 16), -72 + (j * 16), 0f);
                tileParentFalseTrans.GetChild(0).parent = tileParentTrans;
            }
        }

        StopCoroutine(firstCo);


        yield return null;
    }
}
