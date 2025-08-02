using UnityEngine;

abstract public class TileAttribute : MonoBehaviour
{
	public string strObj { get { return "MainTile"; } private set { } }
	public string strEle { get; protected set; }

}
