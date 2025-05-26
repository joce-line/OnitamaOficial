using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptTelaConfiguração : MonoBehaviour
{
	public GameObject panelConfig,panelDadosUsu;

	//botões de seleção
	public void configButton()
	{
		panelConfig.SetActive(true);
		panelDadosUsu.SetActive(false);
	}

	public void dadoUsuButton()
	{
		panelConfig.SetActive(false);
		panelDadosUsu.SetActive(true);
	}

	//navegação 
	public void voltarButton()
	{
		SceneManager.LoadScene("MenuPrincipal");
	}
}
