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

    [SerializeField] Material material;
    [SerializeField] GameObject temEffectManaUI;

    private Image[] manaLogoImageArr; // 마력 로고 스프라이트
    private TextMeshProUGUI[] manaUITextArr;    // 마력 수치 텍스트
    private Color[] defaultManaLogoColorArr;   // 마력 로고 기본 색상

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        manaLogoImageArr = new Image[manaUIArr.Length];
        manaUITextArr = new TextMeshProUGUI[manaUIArr.Length];
        defaultManaLogoColorArr = new Color[manaUIArr.Length];

        for (int i = 0; i < manaUIArr.Length; i++)
        {
            manaLogoImageArr[i] = manaUIArr[i].GetComponent<Image>();
            manaUITextArr[i] = manaUIArr[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            defaultManaLogoColorArr[i] = manaLogoImageArr[i].color;
            var insMat = new Material(material);
            manaLogoImageArr[i].material = insMat;
        }

        for (int i = 0; i < temEffectManaUI.transform.childCount; i++)
        {
            temEffectManaUI.transform.GetChild(i).GetComponent<TextEffectManaUI>().SettingEffect(this, temEffectManaUI.gameObject);
            temEffectManaUI.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 갱신한 마력 수치를 보여줌
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

        if (amount >= 200)
        {
            manaLogoImageArr[idx].material.SetFloat("_InsanityAmount", 15f);
        }
        else if (amount >= 150)
        {
            manaLogoImageArr[idx].material.SetFloat("_InsanityAmount", 6f);
        }
        else if (amount >= 100)
        {
            manaLogoImageArr[idx].material.SetFloat("_InsanityAmount", 3f);
        }
        else if (amount >= 50)
        {
            manaLogoImageArr[idx].material.SetFloat("_InsanityAmount", 1.5f);
        }
        else
        {
            manaLogoImageArr[idx].material.SetFloat("_InsanityAmount", 1f);
        }

        for (int i = 0; i < temEffectManaUI.transform.childCount; i++)
        {
            if (!temEffectManaUI.transform.GetChild(i).gameObject.activeSelf)
            {
                var tem = temEffectManaUI.transform.GetChild(i).gameObject;
                tem.SetActive(true);
                tem.transform.parent = manaUITextArr[idx].gameObject.transform;
                tem.GetComponent<RectTransform>().localPosition = Vector3.zero;
                tem.GetComponent<TextEffectManaUI>().SettingString(manaUITextArr[idx].text);
                break;
            }
        }
    }
}
