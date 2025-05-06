using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.scripts.Classes;
using System.Security.Cryptography;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;


public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    //thiago mostrar Erro
    public TextMeshProUGUI errorMessage;
    public GameObject errorPanel;

    [SerializeField] private GameObject telaLogin;
    [SerializeField] private SceneLoader sceneLoader;

    HashSenha valida = new HashSenha(SHA512.Create());
    public void Login()
    {

        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                return;
            }

            string username = usernameInput.text;
            string password = passwordInput.text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogError("Todos os campos são obrigatórios.");
                showError("Todos os campos são obrigatórios.");

                return;
            }

            //Verifica usuario
            string userQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname=@user";
            var userParams = new Dictionary<string, object> { { "@user", username } };
            long userExists = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(userQuery, userParams));

            if (userExists == 0 || userExists == -1)
            {
                Debug.Log("Usuário ou senha incorretos.");
                showError("Usuário ou senha incorretos.");
                return;
            }

            // Verificar senha
            string selectSenhaQuery = "SELECT senha FROM usuarios WHERE nickname = @nome";
            var senhaParams = new Dictionary<string, object> { { "@nome", username } };
            object result = DatabaseManager.Instance.ExecuteScalar(selectSenhaQuery, senhaParams);

            if (result != null && result.ToString() == "-1")
            {
                Debug.Log("Falha no banco de dados");
                return;
            }

            if (result == null)
            {
                Debug.Log("Usuário ou senha incorretos.");
                showError("Usuário ou senha incorretos.");
                return;
            }

            string comparaSenhaBanco = result?.ToString();
            if (valida.VerificarSenha(password, comparaSenhaBanco))
            {
                Debug.Log("Login bem-sucedido!");
                PlayerInfo.nomePlayer = username;

                sceneLoader.CarregarCena("MenuPrincipal", telaLogin);
            }
            else
            {
                Debug.Log("Usuário ou senha incorretos.");
                showError("Usuário ou senha incorretos.");
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao realizar login: {e.Message}");
        }
    }

    public void OpenRegisterScene()
    {
        SceneManager.LoadScene("CriarUsuario");
    }

    //ativa o painel de erro e mostra a menssagem
    public void showError(string mensagem)
    {
        errorPanel.SetActive(true);
        errorMessage.text = mensagem;
    }

    //esconde o painel de erro e limpa a menssagem
    public void closeError()
    {
        errorPanel.SetActive(false);
        errorMessage.text = null;
    }
}
