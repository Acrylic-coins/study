using UnityEngine;

public class TileDark : TileAttribute
{
	private void Awake()
	{
		strEle = "_Dark";
		type = Constant.ElementType.DARK;
	}
}
