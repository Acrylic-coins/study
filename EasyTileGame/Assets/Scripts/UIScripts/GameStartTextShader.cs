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

	private Coroutine textEffectCo;	// �ֱ������� ����Ʈ�� �����ϱ� ���� �ڷ�ƾ

	private WaitForSeconds textEffectTerm; // ����Ʈ ���� ��

	private List<StartUIShader> startTMProUGUIList = new List<StartUIShader>();	// ������Ʈ �� ����Ʈ�� �ߵ���Ű�� ��ũ��Ʈ���� ���� ����Ʈ

	private void Awake()
	{
		textEffectCo = null;
		textEffectTerm = new WaitForSeconds(0.4f);

		// TMP ����Ʈ �ʱ�ȭ
		startTMProUGUIList.Clear();
		for (int i = 0; i < startObj.transform.childCount; i++)
		{
			startTMProUGUIList.Add(startObj.transform.GetChild(i).GetComponent<StartUIShader>());
			startObj.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	// Ŀ���� �ش� ������Ʈ�� ��� �� �� �� ȣ���
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
