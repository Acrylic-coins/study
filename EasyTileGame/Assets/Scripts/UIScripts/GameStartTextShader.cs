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

	private Coroutine textEffectCo; // �ֱ������� ����Ʈ�� �����ϱ� ���� �ڷ�ƾ
	private Coroutine textDisappearEffectCo; // ��ư Ŭ���� �ؽ�Ʈ�� ����� ���� �ڷ�ƾ

	private WaitForSeconds textEffectTerm; // ����Ʈ ���� ��

	private TextMeshProUGUI tmpUGUIComponent;	// ��ư ������Ʈ �Ʒ� �ؽ�Ʈ�� ����� ����

	private List<StartUIShader> startTMProUGUIList = new List<StartUIShader>(); // ������Ʈ �� ����Ʈ�� �ߵ���Ű�� ��ũ��Ʈ���� ���� ����Ʈ

	private bool isDesappear = false;

	private void Awake()
	{
		textEffectCo = null;
		textEffectTerm = new WaitForSeconds(0.1f);

		tmpUGUIComponent = this.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

		// TMP ����Ʈ �ʱ�ȭ
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
	// �ؽ�Ʈ�� ������ ���ֱ� ���� �ڷ�ƾ
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
