using UnityEditor; // Unity ������ ���� API
using UnityEditor.AddressableAssets.Settings; // Addressables ���ÿ� �����ϱ� ���� API
using UnityEditor.AddressableAssets; // Addressables ���� ��� ����� ���� API
using UnityEngine; // Debug.Log, EditorUtility ���

[InitializeOnLoad] // ������ �ε� �� �ڵ����� Ŭ���� �ʱ�ȭ
public class AutoAddressableBuild
{
	static AutoAddressableBuild()
	{
		// �����Ͱ� Play ���� ��ȯ�� ������ �̺�Ʈ �߻�
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state)
	{
		// Play ���� ���� ������ ��
		if (state == PlayModeStateChange.ExitingEditMode)
		{
			// �˾�â�� ��� ����ڿ��� �����
			bool buildAddressables = EditorUtility.DisplayDialog(
				"Addressables ����", // ����
				"Play ��忡 ���� ���� Addressables �������� �����ұ��?", // ����
				"�� (Build)", // Ȯ�� ��ư �ؽ�Ʈ
				"�ƴϿ� (Skip)" // ��� ��ư �ؽ�Ʈ
			);

			if (buildAddressables) // ����ڰ� '��'�� ������ ��
			{
				Debug.Log("Addressables Build ����!");
				AddressableAssetSettings.BuildPlayerContent(); // Addressables ���� ����
				Debug.Log("Addressables Build �Ϸ�!");
			}
			else // ����ڰ� '�ƴϿ�'�� ������ ��
			{
				Debug.Log("Addressables Build�� �ǳʶ�.");
			}
		}
	}
}
