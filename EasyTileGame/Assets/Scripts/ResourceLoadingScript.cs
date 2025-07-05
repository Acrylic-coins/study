using UnityEngine;	// Unity 기본 API 임포트
using UnityEngine.AddressableAssets;    // Addressables 기능 사용
using UnityEngine.ResourceManagement.AsyncOperations;   // 비동기 작업 정리
using System.Collections.Generic;
using Unity.VisualScripting;
using System;   // List, Queue 등 컬렉션 사용
using System.Linq;

class TileUnit
{
	public string prefabAddress; // 이 풀에 대응하는 프리팹 주소
	public Queue<GameObject> objects = new Queue<GameObject>(); // 실제 오브젝트 풀
	public int maxPoolSize = 150; // 최대 저장할 수 있는 수

	// 추가적으로 스폰/디스폰 처리용 함수도 넣을 수 있음
}

public class ResourceLoadingScript : MonoBehaviour
{
	// 해당 오브젝트 중복 방지(싱글톤)
	public static ResourceLoadingScript Instance;

	List<Dictionary<string, string>> gameObjectList = new List<Dictionary<string, string>>(); // GameObjectTypeCode.csv를 읽어오기 위한 리스트
	List<Dictionary<string, string>> sprList = new List<Dictionary<string, string>>(); // SpriteTypeCode.csv를 읽어오기 위한 리스트
	List<Dictionary<string, string>> aniList = new List<Dictionary<string, string>>(); // AnimatorTypeCode.csv를 읽어오기 위한 리스트
	List<Dictionary<string, string>> elementList = new List<Dictionary<string, string>>(); // ElementCode.csv를 읽어오기 위한 리스트

	Dictionary<string, GameObject> gameObjectResourceDic = new Dictionary<string, GameObject>(); // 각종 게임 오브젝트 리소스를 해당 딕셔너리에 가져옴
	Dictionary<string, Sprite> spriteResourceDic = new Dictionary<string, Sprite>(); // 각종 스프라이트 리소스를 해당 딕셔너리에 가져옴
	Dictionary<string, Animator> animatorResourceDic = new Dictionary<string, Animator>(); // 각종 애니메이터 리소스를 해당 딕셔너리에 가져옴

	// 타일 풀을 관리하는 딕셔너리
	private Dictionary<string, TileUnit> tilePools = new Dictionary<string, TileUnit>();
	// 나중에 리소스 해제용 핸들 저장
	private List<AsyncOperationHandle<GameObject>> tileHandles = new List<AsyncOperationHandle<GameObject>>();

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}

		// csv파일 읽어오기
		gameObjectList = CSVReader.ReadCSV("GameObjectTypeCode.csv");
		sprList = CSVReader.ReadCSV("SpriteTypeCode.csv");
		elementList = CSVReader.ReadCSV("ElementCode.csv");

	}

	private void Start()
	{
		InitialGameObjectResource();	// GameObject 리소스 가져오기
		InitialSpriteResource();	// Sprite 리소스 가져오기
		InitialAnimatorResource();	// Animator 리소스 가져오기
	}

	private void Update()
	{
		Debug.Log(gameObjectResourceDic.Count);
	}

	// 자료형이 GameObject인 리소스를 하나씩 전부 가져온다.
	private void InitialGameObjectResource()
	{
		int objCnt = gameObjectList.Count(x => x.ContainsKey("GameObjectTypeName")); // GameObject의 종류를 의미한다.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // GameObject의 속성 개수를 의미한다.

		for (int i = 0; i < objCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadGameObjectResource(gameObjectList[i]["GameObjectTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// 해당 주소에 GameObject 리소스가 존재하는지 확인 후, 있을 때만 가져온다.
	private void LoadGameObjectResource(string str)
	{
		// 해당 주소(str)에 GameObject 리소스가 존재하는지 확인(LoadResourceLocationsAsync)한다.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count가 0 이하라면 존재하지 않는 것
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<GameObject> obj = Addressables.LoadAssetAsync<GameObject>(str);
				// 리소스를 딕셔너리에 추가한다.
				obj.Completed += (obj) =>
				{
					gameObjectResourceDic.Add(str, obj.Result);

				};
			}
		};
	}

	// 자료형이 Sprite인 리소스를 하나씩 전부 가져온다.
	private void InitialSpriteResource()
	{
		int sprCnt = sprList.Count(x => x.ContainsKey("SpriteTypeName")); // Sprite의 종류를 의미한다.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // Sprite의 속성 개수를 의미한다.

		for (int i = 0; i < sprCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadSpriteResource(sprList[i]["SpriteTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// 해당 주소에 Sprite 리소스가 존재하는지 확인 후, 있을 때만 가져온다.
	private void LoadSpriteResource(string str)
	{
		// 해당 주소(str)에 Sprite 리소스가 존재하는지 확인(LoadResourceLocationsAsync)한다.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count가 0 이하라면 존재하지 않는 것
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<Sprite> obj = Addressables.LoadAssetAsync<Sprite>(str);
				// 리소스를 딕셔너리에 추가한다.
				obj.Completed += (obj) =>
				{
					spriteResourceDic.Add(str, obj.Result);

				};
			}
		};
	}

	// 자료형이 Animator인 리소스를 하나씩 전부 가져온다.
	private void InitialAnimatorResource()
	{
		int aniCnt = aniList.Count(x => x.ContainsKey("AnimatorTypeName")); // Animator의 종류를 의미한다.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // Animator의 속성 개수를 의미한다.

		for (int i = 0; i < aniCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadAnimatorResource(aniList[i]["AnimatorTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// 해당 주소에 Animator 리소스가 존재하는지 확인 후, 있을 때만 가져온다.
	private void LoadAnimatorResource(string str)
	{
		// 해당 주소(str)에 Animator 리소스가 존재하는지 확인(LoadResourceLocationsAsync)한다.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count가 0 이하라면 존재하지 않는 것
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<Animator> obj = Addressables.LoadAssetAsync<Animator>(str);
				// 리소스를 딕셔너리에 추가한다.
				obj.Completed += (obj) =>
				{
					animatorResourceDic.Add(str, obj.Result);

				};
			}
		};
	}
}
