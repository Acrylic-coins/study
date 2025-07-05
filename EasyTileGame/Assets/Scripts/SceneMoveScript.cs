using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMoveScript : MonoBehaviour
{
	private void OnEnable()
	{
		SceneManager.LoadScene("MainPlayScene");
	}
}
