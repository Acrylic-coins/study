using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ���� UI�� �����ϴ� ��ũ��Ʈ
public class ManaUIScripts : MonoBehaviour
{
    [SerializeField] Constant.ElementType[] eleArr;
    [SerializeField] GameObject[] manaUIArr;

    private Image[] manaLogoImageArr; // ���� �ΰ� ��������Ʈ
    private TextMeshPro[] manaUITextArr;    // ���� ��ġ �ؽ�Ʈ

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        manaLogoImageArr = new Image[manaUIArr.Length];
        manaUITextArr = new TextMeshPro[manaUIArr.Length];

        for (int i = 0; i < manaUIArr.Length; i++)
        {
            manaLogoImageArr[i] = manaUIArr[i].GetComponent<Image>();
            manaUITextArr[i] = manaUIArr[i].transform.GetChild(0).GetComponent<TextMeshPro>();
        }


    }
}
