using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptJogarNovamente : MonoBehaviour
{
	public Button jogarButton;

	public void OnClickJogar()
	{
		SceneManager.LoadScene("Game");
	}
}
