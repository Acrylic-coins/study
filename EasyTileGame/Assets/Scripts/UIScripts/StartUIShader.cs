using TMPro;
using UnityEngine;

public class StartUIShader : MonoBehaviour
{
	// ��ǥ�� ������ �� Ʈ������(���⼭�� �ؽ�Ʈ�� �ڽ����� �� ��ư�� Ʈ������)
	[SerializeField] private Transform startTMProBtnTrans;

	private TextMeshProUGUI startTMProUGUI; // �ڽ��� TextMeshProUGUI ������Ʈ

	private Color startTMProColor;
	private Color defaultColor = Color.white;
	private Color addColor = Color.white * 0.5f;


	private float textXRatio = 0f; // ������ �ؽ�Ʈ X������� Ư�� �Ÿ������� �̵����� �ۼ�Ʈ
	private float textYRatio = 0f; // ������ �ؽ�Ʈ Y������� Ư�� �Ÿ������� �̵����� �ۼ�Ʈ

	private float textXRatioChange = 0f; // �ؽ�Ʈ X������� Ư�� �Ÿ������� ���� �̵����� �ۼ�Ʈ
	private float textYRatioChange = 0f; // �ؽ�Ʈ Y������� Ư�� �Ÿ������� ���� �̵����� �ۼ�Ʈ

	private float textXOffset = 0f; // �ؽ�Ʈ�� x��ġ�� ������ x��ġ(0)������ �Ÿ� ����
	private float textYOffset = 0f; // �ؽ�Ʈ�� y��ġ�� ������ y��ġ(0)������ �Ÿ� ����

	private float defaultAlpha = 0.9f;    // textAlpha�� �ʱⰪ
	private float textAlpha = 0.9f; // �ؽ�Ʈ�� ���İ� ��ȭ�� �ֱ����� ��

	private float defaultWidthRadius = 10f;	// Ÿ���κ��� ����κ��� ���� Ÿ���� �ʱ� ���� ������
	private float defaultHeightRadius = 5f; // Ÿ���κ��� ����κ��� ���� Ÿ���� �ʱ� ���� ������

	private float widthRadius = 10f;	// Ÿ���κ��� ����κ��� ���� Ÿ���� ���� ���� ������
	private float heightRadius = 5f;	// Ÿ���κ��� ����κ��� ���� Ÿ���� ���� ���� ������

	private float sin1 = 0f; // ������ ������ ���� ����
	private float sin2 = 0f;
	private float sin3 = 0f;

	private float temR = 0f;
	private float temG = 0f;
	private float temB = 0f;

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

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);
	}

	public void InitShader(bool isStr)
	{
		// �ؽ�Ʈ ����� ������� ���ƿ����� tmpro�� ���׸��� ������ �ʱ�ȭ
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXRatio);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYRatio);

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);

		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", defaultColor);

		// �ؽ�Ʈ ������ �ʱ�ȭ
		textXRatioChange = textXRatio;
		textYRatioChange = textYRatio;

		// �ؽ�Ʈ ���İ� �ʱ�ȭ
		textAlpha = defaultAlpha;

		// Ÿ��ũ�� �ʱ�ȭ
		widthRadius = defaultHeightRadius;
		heightRadius = defaultWidthRadius;

		// �ؽ�Ʈ ���� �ʱ�ȭ
		startTMProColor = defaultColor;
		addColor = Color.white * 0.5f;
		sin1 = 0f;
		sin2 = 0f;
		sin3 = 0f;
		temR = 0f;
		temG = 0f;
		temB = 0f;

		isStart = isStr;

		if (!isStart)
		{
			this.gameObject.SetActive(false);
		}
	}

	private void UpdateShader()
	{
		// �ؽ�Ʈ ���̸� ������Ŵ.
		textXRatioChange = Mathf.Lerp(textXRatioChange, 1f, Time.deltaTime * 3f);
		textYRatioChange = Mathf.Lerp(textYRatioChange, 1f, Time.deltaTime * 3f);

		widthRadius = Mathf.Lerp(widthRadius, 350f, Time.deltaTime * 2f);
		heightRadius = Mathf.Lerp(heightRadius, 150f, Time.deltaTime * 2f);

		// �ؽ�Ʈ�� ���İ��� ���� ����. Lerp�� ���� ������Ŵ. �ִ밪�� 1��
		textAlpha = Mathf.Lerp(textAlpha, 1f, Time.deltaTime * 2.5f);

		// �ؽ�Ʈ ����� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		// (�ؽ�Ʈ�̹Ƿ� material�� �ƴ϶� �ݵ�� fontMaterial�� ����!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXRatioChange);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYRatioChange);

		// Ÿ��ũ�� ����
		startTMProUGUI.fontMaterial.SetFloat("_WidthRadius", widthRadius);
		startTMProUGUI.fontMaterial.SetFloat("_HeightRadius", heightRadius);

		// (0 ~ 1 ���� ������ ����)
		// �ݵ�� 0~1 ���̿��߸� �Ѵ�. 
		sin1 = (float)Random.Range(0, 256) / 255f;
		sin2 = (float)Random.Range(0, 256) / 255f;
		sin3 = (float)Random.Range(0, 256) / 255f;

		startTMProColor = startTMProUGUI.fontMaterial.GetColor("_OutLineColor");

		temR = startTMProColor.r + sin1;
		temG = startTMProColor.g + sin2;
		temB = startTMProColor.b + sin3;

		if (temR > 1f || temR < 0f) { temR = startTMProColor.r - sin1; }
		if (temG > 1f || temG < 0f) { temG = startTMProColor.g - sin2; }
		if (temB > 1f || temB < 0f) { temB = startTMProColor.b - sin3; }

		startTMProColor = new Color(temR, temG, temB, 0f);
		// ���̴��� ��¦���� ���� �߰��� ������ ����(rgb���� 1�� �Ѿ�� �� ��¦��. -> bloomȿ�� ����)
		addColor = Color.Lerp(addColor, Color.black, Time.deltaTime * 1.3f);
		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", startTMProColor + addColor);

		// �ؽ�Ʈ ���İ��� ��ȭ�ϵ��� tmpro�� ���׸��� ������ ����
		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", textAlpha);

		if (textXRatioChange + textYRatioChange >= 1.98f && textAlpha >= 0.99f)
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
