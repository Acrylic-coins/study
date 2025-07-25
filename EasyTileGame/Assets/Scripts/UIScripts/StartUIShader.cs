using TMPro;
using UnityEngine;

public class StartUIShader : MonoBehaviour
{   
	// 좌표의 기준이 될 트랜스폼(여기서는 텍스트를 자식으로 둔 버튼의 트랜스폼)
	[SerializeField] private Transform startTMProBtnTrans;

	private TextMeshProUGUI startTMProUGUI; // 자신의 TextMeshProUGUI 컴포넌트

	private Color startTMProColor;
	private Color defaultColor = Color.white;

	private float textXSize = 1f; // 기존의 텍스트 X사이즈 배율
	private float textYSize = 1f; // 기존의 텍스트 Y사이즈 배율

	private float textXSizeChange = 1f; // 텍스트 X사이즈 현재 배율
	private float textYSizeChange = 1f; // 텍스트 Y사이즈 현재 배율

	private float textXOffset = 0f; // 텍스트의 x위치와 원점의 x위치(0)까지의 거리 차이
	private float textYOffset = 0f; // 텍스트의 y위치와 원점의 y위치(0)까지의 거리 차이

	private float defaultAlpha = 0.65f;    // textAlpha의 초기값
	private float textAlpha = 0.65f; // 텍스트의 알파값 변화를 주기위한 값

	private float sin1 = 0f; // 랜덤한 색상을 위한 변수
	private float sin2 = 0f;
	private float sin3 = 0f;

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

	}

	public void InitShader(bool isStr)
	{
		// 텍스트 사이즈가 원래대로 돌아오도록 tmpro의 마테리얼 변수를 초기화
		// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSize);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSize);

		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", defaultAlpha);

		startTMProUGUI.fontMaterial.SetColor("_OutLineColor", defaultColor);

		// 텍스트 사이즈 초기화
		textXSizeChange = textXSize;
		textYSizeChange = textYSize;

		// 텍스트 알파값 초기화
		textAlpha = defaultAlpha;

		// 텍스트 색상 초기화
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
		// 삼항연산자
		// 텍스트 사이즈를 점진적으로 ( 1f / Time.deltaTIme) 만큼 증가시킴. 사이즈의 최대치는 2임
		textXSizeChange = ((textXSizeChange + (Time.deltaTime)) < 2f ? (textXSizeChange + (Time.deltaTime)) : 2f);
		textYSizeChange = ((textYSizeChange + (Time.deltaTime)) < 2f ? (textYSizeChange + (Time.deltaTime)) : 2f);

		// 텍스트의 알파값에 빼줄 값임 점진적으로 증가시킴. 최대값은 1임
		textAlpha = ((textAlpha + (Time.deltaTime * 0.6f)) < 1f ? (textAlpha + (Time.deltaTime * 0.6f)) : 1f);

		// 텍스트 사이즈가 변화하도록 tmpro의 마테리얼 변수를 수정
		// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSizeChange);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSizeChange);

		// 텍스트 알파값이 변화하도록 tmpro의 마테리얼 변수를 수정
		startTMProUGUI.fontMaterial.SetFloat("_AlphaGage", textAlpha);
		Debug.Log(textAlpha);
		// (0 ~ 1 값을 가지는 변수)
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
