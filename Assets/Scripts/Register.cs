using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data.MySqlClient;
using System;
using Assets.scripts.classes;
using System.Security.Cryptography;

public class Register : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TMP_InputField cpfInput;
    public TMP_InputField emailInput;
    public TMP_InputField senhaInput;

    private string cpf;
    private string email;
    //private int idade;
    private string nickname;
    private string senha;

  
     private string connectionString;

  void Start()
    {
        EnvReader.Load();

        string server = EnvReader.Get("DB_SERVER");
        string db = EnvReader.Get("DB_DATABASE");
        string user = EnvReader.Get("DB_USER");
        string password = EnvReader.Get("DB_PASSWORD");
        string sslmode = EnvReader.Get("DB_SSLMODE");

        connectionString = $"Server={server};Database={db};Uid={user};Pwd={password};SslMode={sslmode};";

        //Debug.Log("String de conexão configurada: " + connectionString);
    }

    public void Registrar()
    {
        //nome = nomeInput.text;
        nickname = nicknameInput.text;
        cpf = cpfInput.text;
        email = emailInput.text;
        //idade = int.Parse(idadeInput.text);
        senha = senhaInput.text;

        HashSenha encriptografar = new HashSenha(SHA512.Create());

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                // Consulta para verificar se o email esta disponivel
                string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
                MySqlCommand cmdEmail = new MySqlCommand(selectEmailQuery, conn);
                cmdEmail.Parameters.AddWithValue("@email", email);

                // ExecuteScalar � usado, quando a consulta retorna algo
                //long countEmail = (long)cmdEmail.ExecuteScalar();
                long countEmail = Convert.ToInt64(cmdEmail.ExecuteScalar());


                if (countEmail > 0) {
                    Debug.Log("Email indisponivel");
                    return;
                }

                // Consulta para verificar se o nickname esta disponivel
                string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
                MySqlCommand cmdNickname = new MySqlCommand(selectNicknameQuery, conn);
                cmdNickname.Parameters.AddWithValue("@nickname", nickname);
                
                

                //long countNick = (long)cmdNick.ExecuteScalar();
                long countNickname = Convert.ToInt64(cmdNickname.ExecuteScalar());

                if (countNickname > 0)
                {
                    Debug.Log("Nickname indisponivel");
                    return;
                }

                // string selectCpfQuery = "SELECT COUNT(*) FROM usuarios WHERE cpf = @cpf";
                // MySqlCommand cmdCpf = new MySqlCommand(selectNicknameQuery, conn);
                // cmdCpf.Parameters.AddWithValue("@cpf", cpf);

                // Inserir valores na tabela
                string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha);";
                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                //cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@nickname", nickname);
                cmd.Parameters.AddWithValue("@cpf", cpf);
                cmd.Parameters.AddWithValue("@email", email);
                //cmd.Parameters.AddWithValue("@idade", idade);
                cmd.Parameters.AddWithValue("@senha", encriptografar.CriptografarSenha(senha));

                // ExecuteNonQuery, � usado quando a consulta n�o retorna nada
                cmd.ExecuteNonQuery();

                Debug.Log("Usu�rio cadastrado!");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao registrar: " + e.Message);
            }
        }
    }
}

