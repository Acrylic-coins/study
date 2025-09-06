using UnityEngine;

public class TileIce : TileAttribute
{
	private void Awake()
	{
		strEle = "_Ice";
		type = Constant.ElementType.ICE;
	}
}
