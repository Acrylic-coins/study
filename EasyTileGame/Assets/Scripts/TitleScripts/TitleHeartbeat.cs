using UnityEngine;

public class TitleHeartbeat : MonoBehaviour
{
	TitleLogoAnime animeScript;

	public void InitAnimeScript(TitleLogoAnime scr)
	{
		animeScript = scr;
	}

	public void ThinkCGHeartbeatLogo()
	{
		animeScript.ThinkCGHeartbeatLogo();
	}
	public void ActiveFalseHeartbeatLogo()
	{
		animeScript.ActiveFalseHeartbeatLogo();
	}
}
