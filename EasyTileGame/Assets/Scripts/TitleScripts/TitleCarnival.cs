using UnityEngine;

public class TitleCarnival : MonoBehaviour
{
	TitleLogoAnime animeScript;
	public void InitAnimeScript(TitleLogoAnime scr)
	{
		animeScript = scr;
	}
	public void ThinkCGCarnivalLogo()
	{
		animeScript.ThinkCGCarnivalLogo();
	}
	public void ActiveFalseCarnivalLogo()
	{
		animeScript.ActiveFalseCarnivalLogo();
	}
}
