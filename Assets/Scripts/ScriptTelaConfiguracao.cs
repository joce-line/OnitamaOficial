using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptTelaConfiguração : MonoBehaviour
{
	//volta para a tela de menu
	public void voltarButton()
	{
		SceneManager.LoadScene("MenuPrincipal");
	}
}
