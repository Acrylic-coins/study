using UnityEditor; // Unity 에디터 관련 API
using UnityEditor.AddressableAssets.Settings; // Addressables 세팅에 접근하기 위한 API
using UnityEditor.AddressableAssets; // Addressables 빌드 기능 사용을 위한 API
using UnityEngine; // Debug.Log, EditorUtility 사용

[InitializeOnLoad] // 에디터 로드 시 자동으로 클래스 초기화
public class AutoAddressableBuild
{
	static AutoAddressableBuild()
	{
		// 에디터가 Play 모드로 전환될 때마다 이벤트 발생
		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state)
	{
		// Play 모드로 들어가기 직전일 때
		if (state == PlayModeStateChange.ExitingEditMode)
		{
			// 팝업창을 띄워 사용자에게 물어본다
			bool buildAddressables = EditorUtility.DisplayDialog(
				"Addressables 빌드", // 제목
				"Play 모드에 들어가기 전에 Addressables 콘텐츠를 빌드할까요?", // 내용
				"네 (Build)", // 확인 버튼 텍스트
				"아니오 (Skip)" // 취소 버튼 텍스트
			);

			if (buildAddressables) // 사용자가 '네'를 눌렀을 때
			{
				Debug.Log("Addressables Build 시작!");
				AddressableAssetSettings.BuildPlayerContent(); // Addressables 빌드 실행
				Debug.Log("Addressables Build 완료!");
			}
			else // 사용자가 '아니오'를 눌렀을 때
			{
				Debug.Log("Addressables Build를 건너뜀.");
			}
		}
	}
}
