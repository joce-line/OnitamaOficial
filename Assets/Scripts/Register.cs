using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Assets.scripts.Classes;
using System.Security.Cryptography;
using System.Collections.Generic;

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

    public void Registrar()
    {
        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                return;
            }

            //nome = nomeInput.text;
            nickname = nicknameInput.text;
            cpf = cpfInput.text;
            email = emailInput.text;
            //idade = int.Parse(idadeInput.text);
            senha = senhaInput.text;

            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                Debug.LogError("Todos os campos são obrigatórios. Algum campo esta nulo.");
                //TODO: quando adicionar texto visivel para usuario, adicionar aqui a atualização da variavel de feedback.
                return;
            }

            HashSenha encriptografar = new HashSenha(SHA512.Create());

            #region email disponivel
            // Consulta para verificar se o email esta disponivel
            string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
            var emailParams = new Dictionary<string, object> { { "@email", email } };
            //pegando função direta do DatabaseManager
            long countEmail = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectEmailQuery, emailParams));
            if (countEmail > 0)
            {
                Debug.Log("Email indisponível.");
                //TODO: adicionar aqui variavel de feedback para usuario
                return;
            }
            #endregion

            #region nickname disponivel
            // Consulta para verificar se o nickname esta disponivel
            string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
            var nickParams = new Dictionary<string, object> { { "@nickname", nickname } };
            long countNickname = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectNicknameQuery, nickParams));

            if (countNickname > 0)
            {
                Debug.Log("Nickname indisponível.");
                //TODO: adicionar aqui variavel de feedback para usuario
                return;
            }
            #endregion

            #region insert tabela
            // Inserir valores na tabela
            string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha);";
            var insertParams = new Dictionary<string, object>
        {
            { "@nickname", nickname },
            { "@cpf", cpf },
            { "@email", email },
            { "@senha", encriptografar.CriptografarSenha(senha) }
        };

            if (DatabaseManager.Instance.ExecuteNonQuery(insertQuery, insertParams))
            {
                Debug.Log("Usuário cadastrado!");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
            }
            else
            {
                Debug.LogError("Falha ao cadastrar usuário.");
                //TODO: adicionar aqui variavel de feedback para usuario
            }

            #endregion

        }
        catch (Exception e)
        {

            Debug.LogError($"Erro ao realizar cadastro: {e.Message}");
        }
    }
}