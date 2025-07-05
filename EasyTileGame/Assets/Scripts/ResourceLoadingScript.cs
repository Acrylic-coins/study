using UnityEngine;	// Unity �⺻ API ����Ʈ
using UnityEngine.AddressableAssets;    // Addressables ��� ���
using UnityEngine.ResourceManagement.AsyncOperations;   // �񵿱� �۾� ����
using System.Collections.Generic;
using Unity.VisualScripting;
using System;   // List, Queue �� �÷��� ���
using System.Linq;

class TileUnit
{
	public string prefabAddress; // �� Ǯ�� �����ϴ� ������ �ּ�
	public Queue<GameObject> objects = new Queue<GameObject>(); // ���� ������Ʈ Ǯ
	public int maxPoolSize = 150; // �ִ� ������ �� �ִ� ��

	// �߰������� ����/���� ó���� �Լ��� ���� �� ����
}

public class ResourceLoadingScript : MonoBehaviour
{
	// �ش� ������Ʈ �ߺ� ����(�̱���)
	public static ResourceLoadingScript Instance;

	List<Dictionary<string, string>> gameObjectList = new List<Dictionary<string, string>>(); // GameObjectTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> sprList = new List<Dictionary<string, string>>(); // SpriteTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> aniList = new List<Dictionary<string, string>>(); // AnimatorTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> elementList = new List<Dictionary<string, string>>(); // ElementCode.csv�� �о���� ���� ����Ʈ

	Dictionary<string, GameObject> gameObjectResourceDic = new Dictionary<string, GameObject>(); // ���� ���� ������Ʈ ���ҽ��� �ش� ��ųʸ��� ������
	Dictionary<string, Sprite> spriteResourceDic = new Dictionary<string, Sprite>(); // ���� ��������Ʈ ���ҽ��� �ش� ��ųʸ��� ������
	Dictionary<string, Animator> animatorResourceDic = new Dictionary<string, Animator>(); // ���� �ִϸ����� ���ҽ��� �ش� ��ųʸ��� ������

	// Ÿ�� Ǯ�� �����ϴ� ��ųʸ�
	private Dictionary<string, TileUnit> tilePools = new Dictionary<string, TileUnit>();
	// ���߿� ���ҽ� ������ �ڵ� ����
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

		// csv���� �о����
		gameObjectList = CSVReader.ReadCSV("GameObjectTypeCode.csv");
		sprList = CSVReader.ReadCSV("SpriteTypeCode.csv");
		elementList = CSVReader.ReadCSV("ElementCode.csv");

	}

	private void Start()
	{
		InitialGameObjectResource();	// GameObject ���ҽ� ��������
		InitialSpriteResource();	// Sprite ���ҽ� ��������
		InitialAnimatorResource();	// Animator ���ҽ� ��������
	}

	private void Update()
	{
		Debug.Log(gameObjectResourceDic.Count);
	}

	// �ڷ����� GameObject�� ���ҽ��� �ϳ��� ���� �����´�.
	private void InitialGameObjectResource()
	{
		int objCnt = gameObjectList.Count(x => x.ContainsKey("GameObjectTypeName")); // GameObject�� ������ �ǹ��Ѵ�.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // GameObject�� �Ӽ� ������ �ǹ��Ѵ�.

		for (int i = 0; i < objCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadGameObjectResource(gameObjectList[i]["GameObjectTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// �ش� �ּҿ� GameObject ���ҽ��� �����ϴ��� Ȯ�� ��, ���� ���� �����´�.
	private void LoadGameObjectResource(string str)
	{
		// �ش� �ּ�(str)�� GameObject ���ҽ��� �����ϴ��� Ȯ��(LoadResourceLocationsAsync)�Ѵ�.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count�� 0 ���϶�� �������� �ʴ� ��
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<GameObject> obj = Addressables.LoadAssetAsync<GameObject>(str);
				// ���ҽ��� ��ųʸ��� �߰��Ѵ�.
				obj.Completed += (obj) =>
				{
					gameObjectResourceDic.Add(str, obj.Result);

				};
			}
		};
	}

	// �ڷ����� Sprite�� ���ҽ��� �ϳ��� ���� �����´�.
	private void InitialSpriteResource()
	{
		int sprCnt = sprList.Count(x => x.ContainsKey("SpriteTypeName")); // Sprite�� ������ �ǹ��Ѵ�.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // Sprite�� �Ӽ� ������ �ǹ��Ѵ�.

		for (int i = 0; i < sprCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadSpriteResource(sprList[i]["SpriteTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// �ش� �ּҿ� Sprite ���ҽ��� �����ϴ��� Ȯ�� ��, ���� ���� �����´�.
	private void LoadSpriteResource(string str)
	{
		// �ش� �ּ�(str)�� Sprite ���ҽ��� �����ϴ��� Ȯ��(LoadResourceLocationsAsync)�Ѵ�.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count�� 0 ���϶�� �������� �ʴ� ��
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<Sprite> obj = Addressables.LoadAssetAsync<Sprite>(str);
				// ���ҽ��� ��ųʸ��� �߰��Ѵ�.
				obj.Completed += (obj) =>
				{
					spriteResourceDic.Add(str, obj.Result);

				};
			}
		};
	}

	// �ڷ����� Animator�� ���ҽ��� �ϳ��� ���� �����´�.
	private void InitialAnimatorResource()
	{
		int aniCnt = aniList.Count(x => x.ContainsKey("AnimatorTypeName")); // Animator�� ������ �ǹ��Ѵ�.
		int eleCnt = elementList.Count(x => x.ContainsKey("ElementName")); // Animator�� �Ӽ� ������ �ǹ��Ѵ�.

		for (int i = 0; i < aniCnt; i++)
		{
			for (int j = 0; j < eleCnt; j++)
			{
				LoadAnimatorResource(aniList[i]["AnimatorTypeName"] + "_" + elementList[j]["ElementName"]);
			}
		}
	}

	// �ش� �ּҿ� Animator ���ҽ��� �����ϴ��� Ȯ�� ��, ���� ���� �����´�.
	private void LoadAnimatorResource(string str)
	{
		// �ش� �ּ�(str)�� Animator ���ҽ��� �����ϴ��� Ȯ��(LoadResourceLocationsAsync)�Ѵ�.
		Addressables.LoadResourceLocationsAsync(str).Completed += handle =>
		{
			// Count�� 0 ���϶�� �������� �ʴ� ��
			if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result.Count > 0)
			{
				AsyncOperationHandle<Animator> obj = Addressables.LoadAssetAsync<Animator>(str);
				// ���ҽ��� ��ųʸ��� �߰��Ѵ�.
				obj.Completed += (obj) =>
				{
					animatorResourceDic.Add(str, obj.Result);

				};
			}
		};
	}
}
