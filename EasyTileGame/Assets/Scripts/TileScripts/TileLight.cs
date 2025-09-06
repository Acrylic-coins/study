using UnityEngine;

public class TileLight : TileAttribute
{
	private void Awake()
	{
		strEle = "_Light";
		type = Constant.ElementType.LIGHT;
	}
}
