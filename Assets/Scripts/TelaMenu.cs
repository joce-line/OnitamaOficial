using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.scripts.InfoPlayer;

public class TelaMenu : MonoBehaviour
{
	public GameObject painelADMButton;
	void Start()
	{
		if (PlayerInfo.role == "admin")
		{
			painelADMButton.SetActive(true);
		}
		else
		{
			painelADMButton.SetActive(false);
		}
	}
	public void AbrirPainelADM()
{
    Application.OpenURL("https://telacrudonitama202501.onrender.com");
}


	public void GameStartButton()
	{
		SceneManager.LoadScene("LoadingLobby");
	}

	public void LojaButton()
	{
		SceneManager.LoadScene("Loja");
	}

	public void ConfgButton()
	{
		SceneManager.LoadScene("Configuracoes");
	}

	public void CreditoButton()
	{
		SceneManager.LoadScene("Creditos");
	}
	public void RankingButton()
	{
		SceneManager.LoadScene("Ranking");
	}

	public void SairButton()
	{
        Application.Quit();
    }
}
