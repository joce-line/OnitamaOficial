using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Text.RegularExpressions;
using Assets.scripts.Classes;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Collections;

public class Register : MonoBehaviour
{
    public TMP_InputField nicknameInput;
    public TMP_InputField cpfInput;
    public TMP_InputField emailInput;
    public TMP_InputField senhaInput;

    public TextMeshProUGUI textoAvisoCPF;
    public TextMeshProUGUI textoAvisoCamposObrigatorios;
    public TextMeshProUGUI textoAvisoSenhaRequisitos;
    public TextMeshProUGUI textoAvisoEmail;
    public TextMeshProUGUI textoAvisoCpfCadastrado;
    public TextMeshProUGUI textoAvisoEmailCadastrado;
    public TextMeshProUGUI textoAvisoNickCadastrado;
    public TextMeshProUGUI textoAvisoErroCadastro;

    private string cpf;
    private string email;
    private string nickname;
    private string senha;

    public void Registrar()
    {
        FeedbackUser aviso = new FeedbackUser();
        bool temErro = false;

        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                return;
            }

            nickname = nicknameInput.text;
            cpf = cpfInput.text;
            email = emailInput.text;
            senha = senhaInput.text;

            // Campos obrigatórios
            if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                Debug.LogError("Todos os campos são obrigatórios.");
                aviso.ExibeTextoAviso(true, textoAvisoCamposObrigatorios);
                temErro = true;
            }
            else
            {
                aviso.ExibeTextoAviso(false, textoAvisoCamposObrigatorios);
            }

            // Se há erro nos campos obrigatórios, não valida as outras condições
            //if (temErro) return;

            // Validações adicionais (senha, email, CPF)
            if (!ValidarCPF(cpf) && !string.IsNullOrEmpty(cpf))
            {
                Debug.LogError("CPF inválido.");
                aviso.ExibeTextoAviso(true, textoAvisoCPF);
                temErro = true;
            }
            else aviso.ExibeTextoAviso(false, textoAvisoCPF);

            if (!email.Contains("@") && !string.IsNullOrEmpty(email))
            {
                Debug.LogError("email inválido.");
                aviso.ExibeTextoAviso(true, textoAvisoEmail);
                temErro = true;
            }
            else aviso.ExibeTextoAviso(false, textoAvisoEmail);

            bool senhaInvalida = senha.Length < 8 ||
                                 !Regex.IsMatch(senha, @"[A-Za-z]") ||
                                 !Regex.IsMatch(senha, @"\d") ||
                                 !Regex.IsMatch(senha, @"[\W_]");

            if (senhaInvalida && !string.IsNullOrEmpty(senha))
            {
                Debug.LogError("Senha invalida.");
                aviso.ExibeTextoAviso(true, textoAvisoSenhaRequisitos);
                temErro = true;
            }
            else
            {
                aviso.ExibeTextoAviso(false, textoAvisoSenhaRequisitos);
            }

            //if (temErro) return;

            // CPF já cadastrado
            string selectCpfQuery = "SELECT COUNT(*) FROM usuarios WHERE cpf = @cpf";
            var cpfParams = new Dictionary<string, object> { { "@cpf", cpf } };
            if (Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectCpfQuery, cpfParams)) > 0)
            {
                Debug.Log("CPF já cadastrado.");
                aviso.ExibeTextoAviso(true, textoAvisoCpfCadastrado);
                temErro = true;
            }
            else aviso.ExibeTextoAviso(false, textoAvisoCpfCadastrado);

            // Email já cadastrado
            string selectEmailQuery = "SELECT COUNT(*) FROM usuarios WHERE email = @email";
            var emailParams = new Dictionary<string, object> { { "@email", email } };
            if (Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectEmailQuery, emailParams)) > 0)
            {
                Debug.Log("Email indisponível.");
                Debug.Log(selectEmailQuery);  // Verifique o debug aqui para saber se a consulta está sendo executada
                aviso.ExibeTextoAviso(true, textoAvisoEmailCadastrado);
                temErro = true;
            }
            else aviso.ExibeTextoAviso(false, textoAvisoEmailCadastrado);

            // Nickname já cadastrado
            string selectNicknameQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname = @nickname";
            var nickParams = new Dictionary<string, object> { { "@nickname", nickname } };
            if (Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(selectNicknameQuery, nickParams)) > 0)
            {
                Debug.Log("Nickname indisponível.");
                aviso.ExibeTextoAviso(true, textoAvisoNickCadastrado);
                temErro = true;
            }
            else aviso.ExibeTextoAviso(false, textoAvisoNickCadastrado);

            if (temErro) return;

            // Criptografar e inserir
            HashSenha encriptografar = new HashSenha(SHA512.Create());

            string insertQuery = "INSERT INTO usuarios (nickname, cpf, email, senha) VALUES (@nickname, @cpf, @email, @senha)";
            var insertParams = new Dictionary<string, object>
            {
                { "@nickname", nickname },
                { "@cpf", cpf },
                { "@email", email },
                { "@senha", encriptografar.CriptografarSenha(senha) }
            };

            if (DatabaseManager.Instance.ExecuteNonQuery(insertQuery, insertParams))
            {
                Debug.Log("Usuário cadastrado com sucesso.");
                SceneManager.LoadScene("Login");
            }
            else
            {
                Debug.LogError("Erro no cadastro.");
                aviso.ExibeTextoAviso(true, textoAvisoErroCadastro);
                temErro = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro no cadastro: {e.Message}");
        }
    }

    private bool ValidarCPF(string cpf)
    {
        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11 || !long.TryParse(cpf, out _)) return false;

        string[] invalidos = {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };
        if (Array.Exists(invalidos, element => element == cpf)) return false;

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int digito1 = soma % 11;
        digito1 = digito1 < 2 ? 0 : 11 - digito1;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        int digito2 = soma % 11;
        digito2 = digito2 < 2 ? 0 : 11 - digito2;

        return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
    }

    //navega para a pagina anterior
    public void OpenLoginScene()
    {
        SceneManager.LoadScene("Login");
    }

    private IEnumerator OcultarAvisoTemporarioTexto(TextMeshProUGUI aviso, float tempo)
    {
        aviso.gameObject.SetActive(true);
        yield return new WaitForSeconds(tempo);
        aviso.gameObject.SetActive(false);
    }
}
