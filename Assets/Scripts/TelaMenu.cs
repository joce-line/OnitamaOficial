using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TelaMenu : MonoBehaviour
{

	void Start()
	{
		//musica de fundo        
		MusicManager.instance.playMusicGeral();
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
