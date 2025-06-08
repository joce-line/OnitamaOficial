using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptTelaMenu : MonoBehaviour
{
	//teste de musica de fundo
    public GameObject musicBackGround;
    public MusicMananger musicMananger;

	void Start()
	{
		//musica de fundo
        musicBackGround = GameObject.FindGameObjectWithTag("MusicMananger");
        musicMananger = musicBackGround.GetComponent<MusicMananger>();
        musicMananger.playMusicGeral();
	}

	public void gameStartButton()
	{
		SceneManager.LoadScene("Game");
	}

	public void lojaButton()
	{
		SceneManager.LoadScene("Loja");
	}

	public void confgButton()
	{
		SceneManager.LoadScene("Configuracoes");
	}

	public void creditoButton()
	{
		SceneManager.LoadScene("Creditos");
	}

	public void sairButton()
	{
		//implementar rotina de logout aqui
	}
}
