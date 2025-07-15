using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class GameStartTextShader : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [SerializeField]private TextMeshProUGUI startTMProUGUI;
	
	private float textXSize = 1f; // ������ �ؽ�Ʈ X������ ����
	private float textYSize = 1f; // ������ �ؽ�Ʈ Y������ ����

	private float textXSizeChange = 1f; // �ؽ�Ʈ X������ ���� ����
	private float textYSizeChange = 1f; // �ؽ�Ʈ Y������ ���� ����

	private bool isMove = false;

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
