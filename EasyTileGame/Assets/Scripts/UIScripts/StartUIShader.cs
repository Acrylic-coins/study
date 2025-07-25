using TMPro;
using UnityEngine;

public class StartUIShader : MonoBehaviour
{   
	// ��ǥ�� ������ �� Ʈ������(���⼭�� �ؽ�Ʈ�� �ڽ����� �� ��ư�� Ʈ������)
	[SerializeField] private Transform startTMProBtnTrans;

	private TextMeshProUGUI startTMProUGUI; // �ڽ��� TextMeshProUGUI ������Ʈ

	private Color startTMProColor;
	private Color defaultColor = Color.white;

	private float textXSize = 1f; // ������ �ؽ�Ʈ X������ ����
	private float textYSize = 1f; // ������ �ؽ�Ʈ Y������ ����

	private float textXSizeChange = 1f; // �ؽ�Ʈ X������ ���� ����
	private float textYSizeChange = 1f; // �ؽ�Ʈ Y������ ���� ����

	private float textXOffset = 0f; // �ؽ�Ʈ�� x��ġ�� ������ x��ġ(0)������ �Ÿ� ����
	private float textYOffset = 0f; // �ؽ�Ʈ�� y��ġ�� ������ y��ġ(0)������ �Ÿ� ����

	private float defaultAlpha = 0.65f;    // textAlpha�� �ʱⰪ
	private float textAlpha = 0.65f; // �ؽ�Ʈ�� ���İ� ��ȭ�� �ֱ����� ��

	private float sin1 = 0f; // ������ ������ ���� ����
	private float sin2 = 0f;
	private float sin3 = 0f;

	private bool isStart = false;

	private void Awake()
	{
		startTMProUGUI = this.GetComponent<TextMeshProUGUI>();

		startTMProColor = defaultColor;

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

		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", defaultColor);

		// �ؽ�Ʈ ������ �ʱ�ȭ
		textXSizeChange = textXSize;
		textYSizeChange = textYSize;

		// �ؽ�Ʈ ���İ� �ʱ�ȭ
		textAlpha = defaultAlpha;

		// �ؽ�Ʈ ���� �ʱ�ȭ
		startTMProColor = defaultColor;
		sin1 = 0f;
		sin2 = 0f;
		sin3 = 0f;

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
		textXSizeChange = ((textXSizeChange + (Time.deltaTime)) < 2f ? (textXSizeChange + (Time.deltaTime)) : 2f);
		textYSizeChange = ((textYSizeChange + (Time.deltaTime)) < 2f ? (textYSizeChange + (Time.deltaTime)) : 2f);

		// �ؽ�Ʈ�� ���İ��� ���� ���� ���������� ������Ŵ. �ִ밪�� 1��
		textAlpha = ((textAlpha + (Time.deltaTime * 0.6f)) < 1f ? (textAlpha + (Time.deltaTime * 0.6f)) : 1f);

		// �ؽ�Ʈ ����� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSizeChange);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSizeChange);

		// �ؽ�Ʈ ���İ��� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", textAlpha);
		Debug.Log(textAlpha);
		// (0 ~ 1 ���� ������ ����)
		sin1 = (Mathf.Sin(GameStartTextShader.SinTime)) * 0.05f;
		sin2 = (Mathf.Sin(GameStartTextShader.SinTime + 70)) * 0.05f;
		sin3 = (Mathf.Sin(GameStartTextShader.SinTime + 160)) * 0.05f;
		startTMProColor = startTMProUGUI.fontMaterial.GetColor("_OutLineColor") + new Color(sin1, sin2, sin3, Time.deltaTime * -1);
		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", startTMProColor);

		if (textXSizeChange + textYSizeChange == 4f && textAlpha >= 1f)
		{
			InitShader(false);
			return;
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
