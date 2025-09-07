using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

// 마력 UI를 갱신하는 스크립트
public class ManaUIScripts : MonoBehaviour
{
    [SerializeField] Constant.ElementType[] eleArr;
    [SerializeField] GameObject[] manaUIArr;

    private Image[] manaLogoImageArr; // 마력 로고 스프라이트
    private TextMeshProUGUI[] manaUITextArr;    // 마력 수치 텍스트

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        manaLogoImageArr = new Image[manaUIArr.Length];
        manaUITextArr = new TextMeshProUGUI[manaUIArr.Length];

        for (int i = 0; i < manaUIArr.Length; i++)
        {
            manaLogoImageArr[i] = manaUIArr[i].GetComponent<Image>();
            manaUITextArr[i] = manaUIArr[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }
    }

    public void UpdateManaAmountText(Constant.ElementType type, int amount)
    {
        int idx = Array.IndexOf(eleArr, type);

        if (amount >= 100)
        {
            manaUITextArr[idx].text
                = $"<sprite name=\"{amount.ToString().Substring(0,1)}\">" +
                $"<sprite name=\"{amount.ToString().Substring(1, 1)}\">" +
                $"<sprite name=\"{amount.ToString().Substring(2, 1)}\">";
        }
        else if (amount >= 10)
        {
            manaUITextArr[idx].text
                = $"<sprite name=\"0\">" +
                $"<sprite name=\"{amount.ToString().Substring(0, 1)}\">" +
                $"<sprite name=\"{amount.ToString().Substring(1, 1)}\">";
        }
        else
        {
            manaUITextArr[idx].text
            = $"<sprite name=\"0\">" +
            $"<sprite name=\"0\">" +
            $"<sprite name=\"{amount.ToString().Substring(0, 1)}\">";

        }
    }
}
