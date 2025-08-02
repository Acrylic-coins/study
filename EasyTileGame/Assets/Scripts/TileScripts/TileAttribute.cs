using TMPro.EditorUtilities;
using UnityEngine;

abstract public class TileAttribute : MonoBehaviour
{
	protected SpriteRenderer sprRen;

	protected Sprite deSpr;	// ���� �ʾ��� �� ��������Ʈ
	protected Sprite liSpr;	// ��Ƽ� ���� �� ��������Ʈ
	public string strObj { get { return "MainTile"; } private set { } }
	public string strEle { get; protected set; }

	public bool isEffect = false;

	// Ÿ�Ͽ� �ʿ��� ��������Ʈ�� ������
	public void InitSprite(Sprite defaultSpr, Sprite lightSpr)
	{
		sprRen = this.gameObject.GetComponent<SpriteRenderer>();

		deSpr = defaultSpr;
		liSpr = lightSpr;

		sprRen.sprite = deSpr;

		isEffect = true;
	}

}
