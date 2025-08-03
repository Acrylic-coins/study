using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Collections.Generic;
using System.Collections;

public class GameStartTextShader : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
	public static float SinTime 
	{
		get
		{
			return sinTime;
		}
		private set
		{
			if (sinTime > 360)
			{
				sinTime = 360 % value;
			}
			else
			{
				sinTime = value;
			}
		}
	}
	private static float sinTime = 0f;


	[SerializeField] private GameObject startObj;

	private Coroutine textEffectCo; // 주기적으로 이펙트를 생성하기 위한 코루틴
	private Coroutine textDisappearEffectCo; // 버튼 클릭후 텍스트를 지우기 위한 코루틴

	private WaitForSeconds textEffectTerm; // 이펙트 생성 텀

	private TextMeshProUGUI tmpUGUIComponent;	// 버튼 오브젝트 아래 텍스트를 숨기기 위함

	private List<StartUIShader> startTMProUGUIList = new List<StartUIShader>(); // 오브젝트 내 이펙트를 발동시키는 스크립트들을 담은 리스트

	private bool isDesappear = false;

	private void Awake()
	{
		textEffectCo = null;
		textEffectTerm = new WaitForSeconds(0.1f);

		tmpUGUIComponent = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

		// TMP 리스트 초기화
		startTMProUGUIList.Clear();
		for (int i = 0; i < startObj.transform.childCount; i++)
		{
			startTMProUGUIList.Add(startObj.transform.GetChild(i).GetComponent<StartUIShader>());
			startObj.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

    private void OnEnable()
    {
        tmpUGUIComponent.fontMaterial.SetColor("_Color", Color.white);
    }

    // 커서가 해당 오브젝트를 벗어날 시 한 번 호출됨
    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		StopCoroutine(textEffectCo);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		textEffectCo = StartCoroutine(TextEffectCo());
	}

	IEnumerator TextEffectCo()
	{
		while(true)
		{
			for (int i = 0; i < startTMProUGUIList.Count; i++)
			{
				if (!startTMProUGUIList[i].gameObject.activeSelf)
				{
					startTMProUGUIList[i].gameObject.SetActive(true);
					startTMProUGUIList[i].InitShader(true);
					break;
				}				
			}

			yield return textEffectTerm;
            yield return textEffectTerm;
            yield return textEffectTerm;
            yield return textEffectTerm;
        }

		yield return null;
	}

	public void DisappearButton()
	{
		if (isDesappear) { return; }

		isDesappear = false;

		StopCoroutine(textEffectCo);

		textDisappearEffectCo = StartCoroutine(TextDisappearEffect());

    }
	// 텍스트를 서서히 없애기 위한 코루틴
	IEnumerator TextDisappearEffect()
	{
		Color deColor = tmpUGUIComponent.fontMaterial.GetColor("_Color");
		Color color = new Color(40f / 255f, 40f / 255f, 40f / 255f, 0f);
		float alpha = 0.1f;
		for (int i = 0; i < 8; i++)
		{
			deColor -= color;

			if (i > 3) { alpha += 0.4f; }

            tmpUGUIComponent.fontMaterial.SetColor("_Color", deColor);
			tmpUGUIComponent.fontMaterial.SetFloat("_AlphaGage", alpha);
            yield return textEffectTerm;
        }

		StopCoroutine(textDisappearEffectCo);
        this.gameObject.SetActive(false);
        yield return null;
	}


	private void Update()
	{
		SinTime += Time.deltaTime;
	}
}
