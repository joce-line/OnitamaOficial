using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptLoja : MonoBehaviour
{
	//paineis de compras na loja
	public GameObject LojaItem,LojaBackground,LojaPoderes,LojaCoin;


	//função de lojas
	public void atvLojaItem()
	{
		LojaItem.SetActive(true);
		LojaBackground.SetActive(false);
		LojaPoderes.SetActive(false);
		LojaCoin.SetActive(false);
	}

	public void atvLojaBackGround()
	{
		LojaItem.SetActive(false);
		LojaBackground.SetActive(true);
		LojaPoderes.SetActive(false);
		LojaCoin.SetActive(false);
	}

	public void atvLojaPoderes()
	{
		LojaItem.SetActive(false);
		LojaBackground.SetActive(false);
		LojaPoderes.SetActive(true);
		LojaCoin.SetActive(false);
	}

	public void atvLojaCoins()
	{
		LojaItem.SetActive(false);
		LojaBackground.SetActive(false);
		LojaPoderes.SetActive(false);
		LojaCoin.SetActive(true);
	}
	//função de navegação
	public void OnClickSair()
	{
		SceneManager.LoadScene("MenuPrincipal");
	}
}
