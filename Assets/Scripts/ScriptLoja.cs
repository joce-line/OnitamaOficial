using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptLoja : MonoBehaviour
{
	public Button sairButton;

	public void OnClickSair()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
