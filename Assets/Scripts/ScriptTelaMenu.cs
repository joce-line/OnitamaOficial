using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptTelaMenu : MonoBehaviour
{
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

   public void sairButton()
   {
	   //implementar rotina de logout aqui
   }
}
