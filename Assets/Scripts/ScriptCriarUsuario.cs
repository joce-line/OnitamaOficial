using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScriptCriarUsuario : MonoBehaviour
{
    public Button create;
    private bool ok = true;

    public void OnCreateButtonClicked()
    {
        if (ok == true)
        {
            SceneManager.LoadScene("Login");
        }
    }
}
