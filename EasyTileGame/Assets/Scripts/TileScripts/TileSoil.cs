using UnityEngine;

public class TileSoil : TileAttribute
{
	private void Awake()
	{
		strEle = "_Soil";
		type = Constant.ElementType.SOIL;
	}
}
