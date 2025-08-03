using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject tileParent; // �ش� ������Ʈ �� �ڽĿ�����Ʈ. Ȱ��ȭ Ÿ�� ������Ʈ���� �ش� ������Ʈ�� �ڽ���
    [SerializeField] private GameObject tileParentFalse; // �ش� ������Ʈ �� �ڽĿ�����Ʈ. ��Ȱ��ȭ Ÿ�� ������Ʈ���� �ش� ������Ʈ�� �ڽ���
    [SerializeField] private GameObject playerObj;  // �� ���� �� �÷��̾ �θ��� ����

    private Transform tileParentTrans;  // tileParent�� transform
    private Transform tileParentFalseTrans; // tileParentFalse�� transform
    private Transform tilePrefabTrans;  // ������ �����ص� tile ������Ʈ���� �ڽ����� �� ������Ʈ�� transform

    private WaitForSeconds term = new WaitForSeconds(0.05f);
    private Coroutine firstCo;
    private Coroutine playerCo;

    private Dictionary<string, Sprite> tileSprDic = new Dictionary<string, Sprite>();
    private List<Type> tileEleList = new List<Type>();

    private bool isMakeMap = false; // �ʻ��� �Լ��� ȣ��Ǿ����� ����. true��� �� ȣ���� �� ����

    void Awake()
    {
        tileParentTrans = tileParent.transform;
        tileParentFalseTrans = tileParentFalse.transform;

        firstCo = null;
        playerCo = null;
        tileEleList = FindSubClass.FindSubclassOf<TileAttribute>();
        
    }

    // �غ�� ������Ʈ��� �ּҸ� ���� ���� ����
    public void MakeMap()
    {
		if (isMakeMap) { return;}
		isMakeMap = true;
        // GetResourceOrder�Լ��� ���ϴ� ���ҽ��� ResourceLoadingScript�� �䱸�ϴ� �����
        tilePrefabTrans = ResourceLoadingScript.Instance.GetResourceOrder(0).transform;
        tileSprDic = ResourceLoadingScript.Instance.GetResourceOrder(0).GetSprite();

		// ������Ʈ���� ������
		while (tilePrefabTrans.childCount > 0)
        {
            tilePrefabTrans.GetChild(0).parent = tileParentFalseTrans;
        }

        // �ڷ�ƾ ȣ�� -> Ÿ���� ���� �ΰ� ���������� �����ϱ� ����
		firstCo = StartCoroutine(SettingFirstTiles());
    }
    // ó���� �����ؾߵǴ� Ÿ�� 150���� Ȱ��ȭ��Ű�� �ڷ�ƾ��
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
                    GameObject ob = tileParentFalseTrans.GetChild(0).gameObject;

                    ob.SetActive(true);
                    ob.transform.localPosition = new Vector3(72 - ((indX - j) * 16), -72 + (((indY + j)) * 16), 0f);
                    ob.transform.parent = tileParentTrans;
                    SettingPrefab(ob);

				}
            }
			yield return term;
		}

        // ȭ�� �ۿ� ������ Ÿ���� ����
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

        // �� �ڷ�ƾ�� ������
        StopCoroutine(firstCo);

        yield return null;
    }

    // ������ Ÿ�Ͽ� �˸´� ������Ʈ�� ������. ������Ʈ���� Ÿ�Ͽ� ���� �Ӽ��� �׿� ���� ��Ģ�� ���
    private void SettingPrefab(GameObject obj)
    {
        obj.AddComponent(tileEleList[Random.Range(0, tileEleList.Count)]);

        string oS = obj.GetComponent<TileAttribute>().strObj;
        string eS = obj.GetComponent<TileAttribute>().strEle;

        Sprite s0 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_0")).Value;
        Sprite s1 = tileSprDic.FirstOrDefault(s => s.Key.Contains(oS) && s.Key.Contains(eS) && s.Key.EndsWith("_1")).Value;

        obj.GetComponent<TileAttribute>().InitSprite(s0, s1);
	}

    IEnumerator ActivePlayer()
    {
        // ���� ���� ������ �ڷ�ƾ�� ��ȯ�ϱ⵵ ���� ����Ǽ� playerCo�� null�� ��
        yield return term;

        playerObj.SetActive(true);
        playerObj.transform.localPosition = new Vector3(8f, -8f - (16f * 3f), 0f);;

        StopCoroutine(playerCo);

        yield return null;
    }
}
