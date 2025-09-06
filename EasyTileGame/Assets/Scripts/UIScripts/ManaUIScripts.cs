using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 마력 UI를 갱신하는 스크립트
public class ManaUIScripts : MonoBehaviour
{
    [SerializeField] Constant.ElementType[] eleArr;
    [SerializeField] GameObject[] manaUIArr;

    private Image[] manaLogoImageArr; // 마력 로고 스프라이트
    private TextMeshPro[] manaUITextArr;    // 마력 수치 텍스트

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
