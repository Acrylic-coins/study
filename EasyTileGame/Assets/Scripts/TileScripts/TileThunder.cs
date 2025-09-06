using UnityEngine;

public class TileThunder : TileAttribute
{
	private void Awake()
	{
		strEle = "_Thunder";
		type = Constant.ElementType.THUNDER;
	}
}
