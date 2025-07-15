using TMPro;
using UnityEngine;

public class StartUIShader : MonoBehaviour
{   
	// ��ǥ�� ������ �� Ʈ������(���⼭�� �ؽ�Ʈ�� �ڽ����� �� ��ư�� Ʈ������)
	[SerializeField] private Transform startTMProBtnTrans;

	private TextMeshProUGUI startTMProUGUI;	// �ڽ��� TextMeshProUGUI ������Ʈ

	private float textXSize = 1f; // ������ �ؽ�Ʈ X������ ����
	private float textYSize = 1f; // ������ �ؽ�Ʈ Y������ ����

	private float textXSizeChange = 1f; // �ؽ�Ʈ X������ ���� ����
	private float textYSizeChange = 1f; // �ؽ�Ʈ Y������ ���� ����

	private float textXOffset = 0f; // �ؽ�Ʈ�� x��ġ�� ������ x��ġ(0)������ �Ÿ� ����
	private float textYOffset = 0f; // �ؽ�Ʈ�� y��ġ�� ������ y��ġ(0)������ �Ÿ� ����

	private float defaultAlpha = 0.9f;    // ������ ���İ�
	private float textAlpha = 0.9f; // �ؽ�Ʈ�� ���İ� ��ȭ�� �ֱ����� ��

	private bool isStart = false;

	private void Awake()
	{
		startTMProUGUI = this.GetComponent<TextMeshProUGUI>();

		// �������� ����� ���� x,y�� ���ϱ�
		textXOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.x;
		textYOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.y;

		// �ؽ�Ʈ ũ�� ��ȯ ������ ��ġ�̵��� �����ϱ� ���� ��ġ�� �������� �ؾߵ�
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetX", textXOffset);
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetY", textYOffset);
	}

	public void InitShader(bool isStr)
	{
		// �ؽ�Ʈ ����� ������� ���ƿ����� tmpro�� ���׸��� ������ �ʱ�ȭ
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSize);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSize);

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);

		// �ؽ�Ʈ ������ �ʱ�ȭ
		textXSizeChange = textXSize;
		textYSizeChange = textYSize;

		// �ؽ�Ʈ ���İ� �ʱ�ȭ
		textAlpha = defaultAlpha;

		isStart = isStr;

		if (!isStart)
		{
			this.gameObject.SetActive(false);
		}
	}

	private void UpdateShader()
	{
		// ���׿�����
		// �ؽ�Ʈ ����� ���������� ( 1f / Time.deltaTIme) ��ŭ ������Ŵ. �������� �ִ�ġ�� 2��
		textXSizeChange = ((textXSizeChange + Time.deltaTime) < 2f ? (textXSizeChange + Time.deltaTime) : 2f);
		textYSizeChange = ((textYSizeChange + Time.deltaTime) < 2f ? (textYSizeChange + Time.deltaTime) : 2f);

		// �ؽ�Ʈ�� ���İ��� ���������� ���ҽ�Ŵ. �ּҰ��� 0��
		textAlpha = ((textAlpha - Time.deltaTime) > 0f ? (textAlpha - Time.deltaTime) : 0f);

		// �ؽ�Ʈ ����� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSizeChange);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSizeChange);

		// �ؽ�Ʈ ���İ��� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", textAlpha);

		if (textXSizeChange + textYSizeChange == 4f && textAlpha == 0f)
		{
			InitShader(false);
		}
	}

	private void Update()
	{
		if (isStart)
		{
			UpdateShader();
		}
	}
}
