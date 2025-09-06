using UnityEngine;

public class TilePlant : TileAttribute
{
	private void Awake()
	{
		strEle = "_Plant";
		type = Constant.ElementType.PLANT;
	}
}
