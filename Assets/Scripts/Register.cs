using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using MySql.Data.MySqlClient;
using System;
using Assets.scripts.classes;
using System.Security.Cryptography;

public class Register : MonoBehaviour
{
    public TMP_InputField nomeInput;
    public TMP_InputField emailInput;
    public TMP_InputField idadeInput;
    public TMP_InputField nickInput;
    public TMP_InputField senhaInput;

    private string nome;
    private string email;
    private int idade;
    private string nick;
    private string senha;

     //private string connectionString = "Server=localhost;Database=jogo;User=root;Password=12345;SslMode=None;";
     private string connectionString;

    // void Start()
    // {
    //    connectionString = $"Server={EnvReader.LoadEnv("DB_SERVER")};" +
    //                       $"Database={EnvReader.LoadEnv("DB_DATABASE")};" +
    //                       $"Uid={EnvReader.LoadEnv("DB_USER")};" +
    //                       $"Pwd={EnvReader.LoadEnv("DB_PASSWORD")};" +
    //                       $"SslMode={EnvReader.LoadEnv("DB_SSLMODE")};";
    // }


    public void Registrar()
    {
        nome = nomeInput.text;
        email = emailInput.text;
        idade = int.Parse(idadeInput.text);
        nick = nickInput.text;
        senha = senhaInput.text;

        HashSenha encriptografar = new HashSenha(SHA512.Create());

        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            try
            {
                conn.Open();

                // Consulta para verificar se o email esta disponivel
                string selectEmailQuery = "SELECT COUNT(*) FROM clientes WHERE email = @email";
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
                string selectNicknameQuery = "SELECT COUNT(*) FROM clientes WHERE nickname = @nickname";
                MySqlCommand cmdNick = new MySqlCommand(selectNicknameQuery, conn);
                cmdNick.Parameters.AddWithValue("@nickname", nick);

                //long countNick = (long)cmdNick.ExecuteScalar();
                long countNick = Convert.ToInt64(cmdNick.ExecuteScalar());

                if (countNick > 0)
                {
                    Debug.Log("Nickname indisponivel");
                    return;
                }

                // Inserir valores na tabela
                string insertQuery = "INSERT INTO clientes (nome, email, idade, nickname, senha) VALUES (@nome, @email, @idade, @nickname, @senha);";
                MySqlCommand cmd = new MySqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@nome", nome);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@idade", idade);
                cmd.Parameters.AddWithValue("@nickname", nick);
                cmd.Parameters.AddWithValue("@senha", encriptografar.CriptografarSenha(senha));

                // ExecuteNonQuery, � usado quando a consulta n�o retorna nada
                cmd.ExecuteNonQuery();

                Debug.Log("Usu�rio cadastrado!");
                UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
            }
            catch (Exception e)
            {
                Debug.LogError("Erro ao registrar: " + e.Message);
            }
        }
    }
}

