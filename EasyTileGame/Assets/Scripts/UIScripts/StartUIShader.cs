using TMPro;
using UnityEngine;

public class StartUIShader : MonoBehaviour
{
	// 좌표의 기준이 될 트랜스폼(여기서는 텍스트를 자식으로 둔 버튼의 트랜스폼)
	[SerializeField] private Transform startTMProBtnTrans;

	private TextMeshProUGUI startTMProUGUI; // 자신의 TextMeshProUGUI 컴포넌트

	private Color startTMProColor;
	private Color defaultColor = Color.white;
	private Color addColor = Color.white * 0.5f;


	private float textXRatio = 0f; // 기존의 텍스트 X사이즈에서 특정 거리까지의 이동진행 퍼센트
	private float textYRatio = 0f; // 기존의 텍스트 Y사이즈에서 특정 거리까지의 이동진행 퍼센트

	private float textXRatioChange = 0f; // 텍스트 X사이즈에서 특정 거리까지의 현재 이동진행 퍼센트
	private float textYRatioChange = 0f; // 텍스트 Y사이즈에서 특정 거리까지의 현재 이동진행 퍼센트

	private float textXOffset = 0f; // 텍스트의 x위치와 원점의 x위치(0)까지의 거리 차이
	private float textYOffset = 0f; // 텍스트의 y위치와 원점의 y위치(0)까지의 거리 차이

	private float defaultAlpha = 0.9f;    // textAlpha의 초기값
	private float textAlpha = 0.9f; // 텍스트의 알파값 변화를 주기위한 값

	private float defaultWidthRadius = 10f;	// 타원부분의 투명부분을 위한 타원의 초기 가로 반지름
	private float defaultHeightRadius = 5f; // 타원부분의 투명부분을 위한 타원의 초기 세로 반지름

	private float widthRadius = 10f;	// 타원부분의 투명부분을 위한 타원의 현재 가로 반지름
	private float heightRadius = 5f;	// 타원부분의 투명부분을 위한 타원의 현재 세로 반지름

	private float sin1 = 0f; // 랜덤한 색상을 위한 변수
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

		// 원점으로 만들기 위한 x,y값 구하기
		textXOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.x;
		textYOffset = 0f - startTMProBtnTrans.GetComponent<RectTransform>().localPosition.y;

		// 텍스트 크기 변환 이전에 위치이동을 방지하기 위해 위치를 원점으로 해야됨
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetX", textXOffset);
		startTMProUGUI.fontMaterial.SetFloat("_OriginOffsetY", textYOffset);

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);
	}

	public void InitShader(bool isStr)
	{
		// 텍스트 사이즈가 원래대로 돌아오도록 tmpro의 마테리얼 변수를 초기화
		// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXRatio);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYRatio);

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);

		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", defaultColor);

		// 텍스트 사이즈 초기화
		textXRatioChange = textXRatio;
		textYRatioChange = textYRatio;

		// 텍스트 알파값 초기화
		textAlpha = defaultAlpha;

		// 타원크기 초기화
		widthRadius = defaultHeightRadius;
		heightRadius = defaultWidthRadius;

		// 텍스트 색상 초기화
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
		// 텍스트 사이를 증가시킴.
		textXRatioChange = Mathf.Lerp(textXRatioChange, 1f, Time.deltaTime * 3f);
		textYRatioChange = Mathf.Lerp(textYRatioChange, 1f, Time.deltaTime * 3f);

		widthRadius = Mathf.Lerp(widthRadius, 350f, Time.deltaTime * 2f);
		heightRadius = Mathf.Lerp(heightRadius, 150f, Time.deltaTime * 2f);

		// 텍스트의 알파값에 빼줄 값임. Lerp를 통해 증가시킴. 최대값은 1임
		textAlpha = Mathf.Lerp(textAlpha, 1f, Time.deltaTime * 2.5f);

		// 텍스트 사이즈가 변화하도록 tmpro의 마테리얼 변수를 수정
		// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXRatioChange);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYRatioChange);

		// 타원크기 수정
		startTMProUGUI.fontMaterial.SetFloat("_WidthRadius", widthRadius);
		startTMProUGUI.fontMaterial.SetFloat("_HeightRadius", heightRadius);

		// (0 ~ 1 값을 가지는 변수)
		// 반드시 0~1 사이여야만 한다. 
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
		// 쉐이더의 반짝임을 위해 추가로 더해줄 색상(rgb값이 1을 넘어가면 더 반짝임. -> bloom효과 때문)
		addColor = Color.Lerp(addColor, Color.black, Time.deltaTime * 1.3f);
		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", startTMProColor + addColor);

		// 텍스트 알파값이 변화하도록 tmpro의 마테리얼 변수를 수정
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
