using TMPro.EditorUtilities;
using UnityEngine;

abstract public class TileAttribute : MonoBehaviour
{
	protected SpriteRenderer sprRen;

	protected Sprite deSpr;	// 밟지 않았을 때 스프라이트
	protected Sprite liSpr;	// 밟아서 빛날 때 스프라이트
	public string strObj { get { return "MainTile"; } private set { } }
	public string strEle { get; protected set; }

	public bool isEffect = false;

	// 타일에 필요한 스프라이트를 갱신함
	public void InitSprite(Sprite defaultSpr, Sprite lightSpr)
	{
		sprRen = this.gameObject.GetComponent<SpriteRenderer>();

		deSpr = defaultSpr;
		liSpr = lightSpr;

		sprRen.sprite = deSpr;

		isEffect = true;
	}

}
