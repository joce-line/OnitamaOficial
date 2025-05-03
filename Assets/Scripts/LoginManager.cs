using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.scripts.Classes;
using System.Security.Cryptography;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    //thiago mostrar Erro
    public TextMeshProUGUI errorMessage;
    public GameObject errorPanel;

    HashSenha valida = new HashSenha(SHA512.Create());
    public void Login()
    {
        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                showError();
                return;
            }

            string username = usernameInput.text;
            string password = passwordInput.text;

            //Debug.Log($"Tentando login com usuário: {username}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogError("Todos os campos são obrigatórios.");
                //TODO: quando adicionar texto visivel para usuario, adicionar aqui a atualização da variavel de feedback.
                showError();
                return;
            }

            //Verifica usuario
            string userQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname=@user";
            var userParams = new Dictionary<string, object> { { "@user", username } };
            long userExists = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(userQuery, userParams));

            if (userExists == 0 || userExists == -1)
            {
                Debug.Log("Usuário ou senha incorretos.");
                //TODO: adicionar aqui variavel de feedback para usuario
                showError();
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
                //TODO: adicionar aqui variavel de feedback para usuario
                showError();
                return;
            }

            string comparaSenhaBanco = result?.ToString();
            if (valida.VerificarSenha(password, comparaSenhaBanco))
            {
                Debug.Log("Login bem-sucedido!");
                PlayerInfo.nomePlayer = username;
                SceneManager.LoadScene("MenuPrincipal");
            }
            else
            {
                Debug.Log("Usuário ou senha incorretos.");
                //TODO: adicionar aqui variavel de feedback para usuario
                showError();
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
    public void showError()
    {
        errorPanel.SetActive(true);
        errorMessage.text = "Usuário ou senha incorretos.";
    }
    
    //esconde o painel de erro e limpa a menssagem
    public void closeError()
    {
        errorPanel.SetActive(false);
        errorMessage.text = null;
    }
}
