using TMPro.EditorUtilities;
using UnityEngine;
using System.Linq;
using System.Collections;

abstract public class TileAttribute : MonoBehaviour
{
	private Coroutine startEffectCo;
	private Coroutine startEffectCo2;
	private WaitForSeconds se = new WaitForSeconds(0.01f);
	private WaitForSeconds se2 = new WaitForSeconds(0.014f);

	protected SpriteRenderer sprRen;
	protected SpriteRenderer sprOnRen;

	protected Sprite deSpr;	// ���� �ʾ��� �� ��������Ʈ
	protected Sprite liSpr;	// ��Ƽ� ���� �� ��������Ʈ
	public string strObj { get { return "MainTile"; } private set { } }
	public string strEle { get; protected set; }

	public bool isEffect = false;

	// Ÿ�Ͽ� �ʿ��� ��������Ʈ�� ������
	public void InitSprite(Sprite defaultSpr, Sprite lightSpr)
	{
		// �⺻ Ÿ�� ��������Ʈ�� ���� SpriteRenderer ĳ��
		sprRen = this.gameObject.GetComponent<SpriteRenderer>();
		// ����� �� ������ Ÿ�� ��������Ʈ�� ���� SpriteRendererĳ��
		for (int i = 0; i < this.transform.childCount; i++)
		{
			if (this.transform.GetChild(i).tag.Equals("TILEON"))
			{
				sprOnRen = this.transform.GetChild(i).GetComponent<SpriteRenderer>();
				break;
			}
		}

		deSpr = defaultSpr;
		liSpr = lightSpr;

		sprRen.sprite = deSpr;
		sprOnRen.sprite = liSpr;

		sprRen.material.SetTexture("_MainTex", defaultSpr.texture);

		isEffect = true;

		startEffectCo = StartCoroutine(EffectCo());
	}

	IEnumerator EffectCo()
	{
		float bloomPlus = 0.2f;
		float bloomGraphPlus = 10f;
		float xSub = 0.7f;
		float ySub = 0.7f;


		sprRen.material.SetFloat("_Xsub", xSub);
		sprRen.material.SetFloat("_Ysub", ySub);
		sprRen.material.SetFloat("_IsLight", 1f);


		for (int i = 0; i < 10; i++)
		{
			yield return se;

			bloomGraphPlus += -0.5f;
			sprRen.material.SetFloat("_BloomGraphPlus", bloomGraphPlus);
		}


		for (int i = 0; i < 10; i++)
		{
			yield return se;

			xSub -= 0.07f;
			ySub -= 0.07f;

			sprRen.material.SetFloat("_Xsub", xSub);
			sprRen.material.SetFloat("_Ysub", ySub);

			bloomGraphPlus += -0.5f;
			sprRen.material.SetFloat("_BloomGraphPlus", bloomGraphPlus);

			bloomPlus += 0.05f;

			sprRen.material.SetFloat("_BloomPlus", bloomPlus);

			if (i == 4)
			{
				startEffectCo2 = StartCoroutine(EffectCo2());
			}
		}

		bloomGraphPlus = 10f;
		sprRen.material.SetFloat("_IsLight", 0f);

		sprRen.material.SetFloat("_BloomGraphPlus", bloomGraphPlus);

		bloomPlus = 0f;
		sprRen.material.SetFloat("_BloomPlus", bloomPlus);

		StopCoroutine(startEffectCo);

		yield return null;
	}

	IEnumerator EffectCo2()
	{
		Color co = new Color(45f / 255f, 45f / 255f, 45f / 255f, 40f / 255f);

		for (int i = 0; i < 10; i++)
		{
			yield return se2;
			sprOnRen.color += co;
		}

		for (int i = 0; i < 10; i++)
		{
			yield return se2;
			sprOnRen.color -= (co);
		}

		StopCoroutine(startEffectCo2);

		yield return null;
	}

}
