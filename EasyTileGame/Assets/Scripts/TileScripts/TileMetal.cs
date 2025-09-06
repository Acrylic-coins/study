using UnityEngine;

public class TileMetal : TileAttribute
{
	private void Awake()
	{
		strEle = "_Metal";
		type = Constant.ElementType.METAL;
	}
}
