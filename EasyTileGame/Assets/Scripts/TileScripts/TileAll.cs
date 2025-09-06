using UnityEngine;

public class TileAll : TileAttribute
{
	private void Awake()
	{
		strEle = "All";
		type = Constant.ElementType.ALL;
	}
}
