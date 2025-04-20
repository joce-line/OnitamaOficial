using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.scripts.classes;
using System.Security.Cryptography;
using Assets.scripts.infoPlayer;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    private string connectionString;

    void Start()
    {
        // Garante que o arquivo .env seja carregado antes de acessar as variáveis
        EnvReader.Load();

        string server = EnvReader.Get("DB_SERVER");
        string db = EnvReader.Get("DB_DATABASE");
        string user = EnvReader.Get("DB_USER");
        string password = EnvReader.Get("DB_PASSWORD");
        string sslmode = EnvReader.Get("DB_SSLMODE");

        connectionString = $"Server={server};Database={db};Uid={user};Pwd={password};SslMode={sslmode};";

        //Debug.Log("String de conexão configurada: " + connectionString);
    }

     HashSenha valida = new HashSenha(SHA512.Create());
    public void Login()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        Debug.Log($"Tentando login com usuário: {username}");

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                string selectSenhaQuery = "SELECT senha FROM usuarios WHERE nickname = @nome";
                MySqlCommand cmdSenha = new MySqlCommand(selectSenhaQuery, conn);
                cmdSenha.Parameters.AddWithValue("@nome", username);

                object result = cmdSenha.ExecuteScalar();
                string comparaSenhaBanco = result?.ToString();

                if (comparaSenhaBanco == null)
                {
                    Debug.Log("Usuário não encontrado.");
                    return;
                }

                string query = "SELECT COUNT(*) FROM usuarios WHERE nickname=@user";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@user", username);

                int userExists = Convert.ToInt32(cmd.ExecuteScalar());

                if (userExists > 0 && valida.VerificarSenha(password, comparaSenhaBanco))
                {
                    Debug.Log("Login bem-sucedido!");
                    SceneManager.LoadScene("GameScene");
                    playerInfo.nomePlayer = username;
                }
                else
                {
                    Debug.Log("Usuário ou senha incorretos.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Erro de conexão: " + e.Message);
            }
        }
    }

    public void OpenRegisterScene()
    {
        SceneManager.LoadScene("RegisterScene");
    }
}
