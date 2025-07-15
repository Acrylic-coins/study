using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GameStartTextShader : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField]private TextMeshProUGUI startTMProUGUI;

	private Transform startTMProBtnTrans;

	private float textXSize = 1f; // ������ �ؽ�Ʈ X������ ����
	private float textYSize = 1f; // ������ �ؽ�Ʈ Y������ ����

	private float textXSizeChange = 1f; // �ؽ�Ʈ X������ ���� ����
	private float textYSizeChange = 1f; // �ؽ�Ʈ Y������ ���� ����

	private float textXOffset = 0f; // �ؽ�Ʈ�� x��ġ�� ������ x��ġ(0)������ �Ÿ� ����
	private float textYOffset = 0f; // �ؽ�Ʈ�� y��ġ�� ������ y��ġ(0)������ �Ÿ� ����

	private bool isMove = false;

	

	private void Awake()
	{
		startTMProBtnTrans = this.gameObject.transform;
		textXOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.x;
		textYOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.y;
		Debug.Log(textXOffset + "&" + textYOffset);
		// �ؽ�Ʈ ũ�� ��ȯ ������ ��ġ�̵��� �����ϱ� ���� ��ġ�� �������� �ؾߵ�
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetX", textXOffset);
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetY", textYOffset);
	}

	// Ŀ���� �ش� ������Ʈ�� ��� �� �� �� ȣ���
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		// �ؽ�Ʈ ����� ������� ���ƿ����� tmpro�� ���׸��� ������ �ʱ�ȭ
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSize);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSize);

		textXSizeChange = textXSize;
		textYSizeChange = textYSize;
		isMove = false;
	}

	// Ŀ���� �ش� ������Ʈ ���� ���� �� �� ������ ȣ���
	void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
	{
		isMove = true;
	}

	private void Update()
	{
		if (isMove)
		{
			// ���׿�����
			// �ؽ�Ʈ ����� ���������� ( 1f / Time.deltaTIme) ��ŭ ������Ŵ. �������� �ִ�ġ�� 2��
			textXSizeChange = ((textXSizeChange + Time.deltaTime) < 2f ? (textXSizeChange + Time.deltaTime) : 2f);
			textYSizeChange = ((textYSizeChange + Time.deltaTime) < 2f ? (textYSizeChange + Time.deltaTime) : 2f);

			// �ؽ�Ʈ ����� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
			// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
			startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSizeChange);
			startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSizeChange);
		}
	}
}
