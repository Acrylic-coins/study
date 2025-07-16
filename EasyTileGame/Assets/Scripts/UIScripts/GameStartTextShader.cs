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

	private Coroutine textEffectCo;	// 주기적으로 이펙트를 생성하기 위한 코루틴

	private WaitForSeconds textEffectTerm; // 이펙트 생성 텀

	private List<StartUIShader> startTMProUGUIList = new List<StartUIShader>();	// 오브젝트 내 이펙트를 발동시키는 스크립트들을 담은 리스트

	private void Awake()
	{
		textEffectCo = null;
		textEffectTerm = new WaitForSeconds(0.4f);

		// TMP 리스트 초기화
		startTMProUGUIList.Clear();
		for (int i = 0; i < startObj.transform.childCount; i++)
		{
			startTMProUGUIList.Add(startObj.transform.GetChild(i).GetComponent<StartUIShader>());
			startObj.transform.GetChild(i).gameObject.SetActive(false);
		}
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
		}

		yield return null;
	}

	private void Update()
	{
		SinTime += Time.deltaTime;
	}
}
