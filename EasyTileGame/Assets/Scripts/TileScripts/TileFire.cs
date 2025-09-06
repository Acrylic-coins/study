using UnityEngine;

public class TileFire : TileAttribute
{
	private void Awake()
	{
		strEle = "_Fire";
		type = Constant.ElementType.FIRE;
	}
}
