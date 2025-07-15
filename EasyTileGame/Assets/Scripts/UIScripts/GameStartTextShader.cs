using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GameStartTextShader : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField]private TextMeshProUGUI startTMProUGUI;
	
	private float textXSize = 1f; // 기존의 텍스트 X사이즈 배율
	private float textYSize = 1f; // 기존의 텍스트 Y사이즈 배율

	private float textXSizeChange = 1f; // 텍스트 X사이즈 현재 배율
	private float textYSizeChange = 1f; // 텍스트 Y사이즈 현재 배율

	private bool isMove = false;

	// 커서가 해당 오브젝트를 벗어날 시 한 번 호출됨
	void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
	{
		// 텍스트 사이즈가 원래대로 돌아오도록 tmpro의 마테리얼 변수를 초기화
		// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
		startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSize);
		startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSize);

		textXSizeChange = textXSize;
		textYSizeChange = textYSize;
		isMove = false;
	}

	// 커서가 해당 오브젝트 위에 있을 시 매 프레임 호출됨
	void IPointerMoveHandler.OnPointerMove(PointerEventData eventData)
	{
		isMove = true;
	}

	private void Update()
	{
		if (isMove)
		{
			// 삼항연산자
			// 텍스트 사이즈를 점진적으로 ( 1f / Time.deltaTIme) 만큼 증가시킴. 사이즈의 최대치는 2임
			textXSizeChange = ((textXSizeChange + Time.deltaTime) < 2f ? (textXSizeChange + Time.deltaTime) : 2f);
			textYSizeChange = ((textYSizeChange + Time.deltaTime) < 2f ? (textYSizeChange + Time.deltaTime) : 2f);
			// 텍스트 사이즈가 변화하도록 tmpro의 마테리얼 변수를 수정
			// (텍스트이므로 material이 아니라 반드시 fontMaterial을 쓸것!)
			startTMProUGUI.fontMaterial.SetFloat("_MultiX", textXSizeChange);
			startTMProUGUI.fontMaterial.SetFloat("_MultiY", textYSizeChange);
		}
	}
}
