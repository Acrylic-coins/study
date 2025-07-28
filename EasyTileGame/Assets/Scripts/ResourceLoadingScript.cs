using UnityEngine;	// Unity �⺻ API ����Ʈ
using UnityEngine.AddressableAssets;    // Addressables ��� ���
using UnityEngine.ResourceManagement.AsyncOperations;   // �񵿱� �۾� ����
using System.Collections.Generic;
using Unity.VisualScripting;
using System;   // List, Queue �� �÷��� ���
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

	[SerializeField] private TextMeshProUGUI loadingText;
	// ���ҽ��� �䱸�ϴ� ������ �� �ֹ�
	[SerializeField] private ResourceOrder[] resourceOrders;
	// �ֹ��� ��۵Ǿ�� �ϴ� �ּ� ������Ʈ
	GameObject[] resourceOrderAddress;

	List<Dictionary<string, string>> gameObjectList = new List<Dictionary<string, string>>(); // GameObjectTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> sprList = new List<Dictionary<string, string>>(); // SpriteTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> aniList = new List<Dictionary<string, string>>(); // AnimatorTypeCode.csv�� �о���� ���� ����Ʈ
	List<Dictionary<string, string>> elementList = new List<Dictionary<string, string>>(); // ElementCode.csv�� �о���� ���� ����Ʈ

	Dictionary<string, GameObject> gameObjectResourceDic = new Dictionary<string, GameObject>(); // ���� ���� ������Ʈ ���ҽ��� �ش� ��ųʸ��� ������
	Dictionary<string, List<Sprite>> spriteResourceDic = new Dictionary<string, List<Sprite>>(); // ���� ��������Ʈ ���ҽ��� �ش� ��ųʸ��� ������
	Dictionary<string, Animator> animatorResourceDic = new Dictionary<string, Animator>(); // ���� �ִϸ����� ���ҽ��� �ش� ��ųʸ��� ������

	// Ÿ�� Ǯ�� �����ϴ� ��ųʸ�
	private Dictionary<string, TileUnit> tilePools = new Dictionary<string, TileUnit>();
	// ���߿� ���ҽ� ������ �ڵ� ����
	private List<AsyncOperationHandle<GameObject>> tileHandles = new List<AsyncOperationHandle<GameObject>>();
	// �̸� �غ��� �������� �������� �˱����� ����ü 
	private List<Constant.OrderForm> orderForm = new List<Constant.OrderForm>();

	private float loadingGage = 0f;	// ���ҽ� �ҷ����� ���൵
	private int resourceCnt = 0;	// �� �����;� �ϴ� ���ҽ�(�뷫����)
	private int compleResource = 0;	// �� ������ ���ҽ�(�������� ���Ѱ͵� ����)
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
		aniList = CSVReader.ReadCSV("AnimatorTypeCode.csv");
		elementList = CSVReader.ReadCSV("ElementCode.csv");

		resourceOrderAddress = new GameObject[resourceOrders.Length];

		for (int i = 0; i < resourceOrderAddress.Length; i++)
		{
			resourceOrderAddress[i] = resourceOrders[i].gameObject;
		}

		// �����;��� ���ҽ� ������ �밭 �����(��Ȯ���� ����)
		resourceCnt = (gameObjectList.Count + sprList.Count + aniList.Count) * elementList.Count;
		// ���� �ε���Ȳ �ʱ�ȭ
		loadingText.text = "0%";



	}

	private void Start()
	{
		InitialGameObjectResource();	// GameObject ���ҽ� ��������
		InitialSpriteResource();	// Sprite ���ҽ� ��������
		InitialAnimatorResource();	// Animator ���ҽ� ��������
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
					// ���ҽ��� �ҷ������� �ε��������� ���δ�.
					UpdateLoadingGage(1);
				};
			}
			else
			{
				// ���ҽ��� ��� �ε��������� ���δ�. (���� ���ҽ��� ������ ����)
				UpdateLoadingGage(1);
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
				AsyncOperationHandle<Sprite[]> obj = Addressables.LoadAssetAsync<Sprite[]>(str);
				// ���ҽ��� ��ųʸ��� �߰��Ѵ�.
				obj.Completed += (obj) =>
				{
					spriteResourceDic.Add(str, obj.Result.ToList());
					// ���ҽ��� �ҷ������� �ε��������� ���δ�.
					UpdateLoadingGage(1);
				};
			}
			else
			{
				// ���ҽ��� ��� �ε��������� ���δ�. (���� ���ҽ��� ������ ����)
				UpdateLoadingGage(1);
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
					// ���ҽ��� �ҷ������� �ε��������� ���δ�.
					UpdateLoadingGage(1);
				};
			}
			else
			{
				// ���ҽ��� ��� �ε��������� ���δ�. (���� ���ҽ��� ������ ����)
				UpdateLoadingGage(1);
			}
		};
	}
	// ���ҽ� �ε���Ȳ�� �����ϴ� �Լ�
	private void UpdateLoadingGage(int cnt)
	{
		compleResource += cnt;
		loadingGage = ((float)compleResource / (float)resourceCnt) * 100;

		// ���� �ε���Ȳ ����
		loadingText.text = loadingGage.ToString("F2") + "%";

		if (compleResource == resourceCnt)
		{
			InitialPrefab();
		}
	}

	private void UpdateInstantiateGage(int cnt)
	{
		compleResource += cnt;
		loadingGage = ((float)compleResource / (float)resourceOrders.Length) * 100;

		// ���� �ε���Ȳ ����
		loadingText.text = loadingGage.ToString("F2") + "%";

		if (compleResource == resourceOrders.Length)
		{
			MoveAnotherScene("MainPlayScene");
		}
	}

	// �ε��� ���ҽ����� ���� �������� �̸� ������ ���� ������(->������Ʈ Ǯ���� ����)
	private void InitialPrefab()
	{
		compleResource = 0;

		for (int i = 0; i < resourceOrders.Length; i++)
		{
			// ���ҽ� �䱸������ ������
			MoveOrder(resourceOrders[i].GetOrderForm());
			for (int j = 0; j < orderForm.Count; j++)
			{
				ResourceTypeBranch(orderForm[j], i);
			}
			UpdateInstantiateGage(1);
		}
	}
	// ��� ������ ����ü ������ �ٸ� ����ü�� �ű�� ���� �Լ�. ����ü �ȿ� ���������� list ���� �ڷ��������� �����ϴ� ���������� ����
	private void MoveOrder(List<Constant.OrderForm> info)
	{
		orderForm.Clear();

		for (int i = 0; i < info.Count; i++)
		{
			Constant.OrderForm fo = new Constant.OrderForm() { orderName = new List<string>() };
			info[i].orderName.MoveList<String>(fo.orderName);
			fo.orderType = info[i].orderType;
			fo.orderCnt = info[i].orderCnt;
			fo.resourceType = info[i].resourceType;
			fo.resourceParent = info[i].resourceParent;
			fo.resourceCode = info[i].resourceCode;

			orderForm.Add(fo);
		}
	}

	// �ʿ��� ���ҽ��� Ÿ�Ժ��� ȣ���� �Լ��� �ٲ�
	private void ResourceTypeBranch(Constant.OrderForm form, int index)
	{
		switch(form.resourceType)
		{
			case Constant.ResourceType.GAMEOBJECT:
				instantiateGameObject(form.orderName, form.orderType, form.orderCnt, index, form.resourceParent);
				break;
			case Constant.ResourceType.SPRITE:
				InstantiateSprite(form.orderName, form.orderType, form.orderCnt, index, form.resourceCode);
				break;
		}
	}
	// ���ӿ�����Ʈ�� �����ϰ��� �ϴ� ���� ������.
	private void instantiateGameObject(List<string> name, Constant.OrderType type, int cnt, int index, Transform trans)
	{
		switch(type)
		{
			case Constant.OrderType.SELECT:
				for (int i = 0; i < name.Count; i++)
				{
					if (gameObjectResourceDic.ContainsKey(name[i]))
					{
						for (int j = 0; j < cnt; j++)
						{
							var tem = Instantiate(gameObjectResourceDic[name[i]]);
							tem.transform.parent = trans;
							tem.transform.localPosition = Vector3.zero;
							tem.SetActive(false);
						}
					}
				}
				break;
			case Constant.OrderType.ALL:
				for (int i = 0; i < name.Count; i++)
				{
					var list = gameObjectResourceDic.Keys.Where(x => x.Contains(name[i])).ToList();
					for (int j = 0; j < list.Count; j++)
					{
						if (gameObjectResourceDic.ContainsKey(list[j]))
						{
							for (int k = 0; k < cnt; k++)
							{
								var tem = Instantiate(gameObjectResourceDic[list[j]]);
								tem.transform.parent = trans;
								tem.transform.localPosition = Vector3.zero;
								tem.SetActive(false);
							}
						}
					}
				}
				break;
		}
	}

	private void InstantiateSprite(List<string> name, Constant.OrderType type, int cnt, int index, int code)
	{
		switch(type)
		{
			case Constant.OrderType.SELECT:
				for (int i = 0; i < name.Count; i++)
				{
					if (spriteResourceDic.ContainsKey(name[i]))
					{
						for (int j = 0; j < cnt; j++)
						{
							var tem = spriteResourceDic[name[i]];
							resourceOrders[index].SetSpriteResource(tem[0], code);
						}
					}
				}
				break;
			case Constant.OrderType.ALL:
				for (int i = 0; i < name.Count; i++)
				{
					var list = spriteResourceDic.Keys.Where(x => x.Contains(name[i])).ToList();
					for (int j = 0; j < list.Count; j++)
					{
						if (spriteResourceDic.ContainsKey(list[j]))
						{
							for (int k = 0; k < cnt; k++)
							{
								var tem = spriteResourceDic[list[j]];
								resourceOrders[index].SetSpriteResource(tem, code);
							}
						}
					}
				}
				break;
		}
	}

	private void MoveAnotherScene(string str)
	{
		SceneManager.LoadScene(str);
	}
}
