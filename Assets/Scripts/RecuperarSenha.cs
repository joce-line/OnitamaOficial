using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Assets.scripts.Classes;

public class RecuperarSenha : MonoBehaviour
{
    [Header("Campos de Input")]
    public TMP_InputField cpfInput;
    public TMP_InputField novaSenhaInput;


    [Header("Canvas")]
    public GameObject canvasLogin;
    public GameObject canvasRecuperarSenha;

    [Header("Painel de erro")]
    public GameObject errorPanel;

    private HashSenha encriptador = new HashSenha(SHA512.Create());

    public void EnviarNovaSenha()
    {
        string cpf = cpfInput.text;
        string novaSenha = novaSenhaInput.text;

        try
        {
            if (string.IsNullOrEmpty(cpf) || string.IsNullOrEmpty(novaSenha))
            {
                showError();
                return;
            }


            string senhaCriptografada = encriptador.CriptografarSenha(novaSenha);

            string updateQuery = "UPDATE usuarios SET senha = @novaSenha WHERE cpf = @cpf";
            var parametros = new Dictionary<string, object>
            {
                { "@novaSenha", senhaCriptografada },
                { "@cpf", cpf }
            };

            bool sucesso = DatabaseManager.Instance.ExecuteNonQuery(updateQuery, parametros);

            if (sucesso)
            {
                Debug.Log("Senha atualizada com sucesso.");
                AlternarParaLogin();
            }
            else
            {
                Debug.Log("Erro ao atualizar a senha.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao atualizar a senha: {e.Message}");
        }
    }

    public void showError()
    {
        errorPanel.SetActive(true);
    }


    public void closeError()
    {
        errorPanel.SetActive(false);
    }

    public void AlternarParaLogin()
    {
        canvasRecuperarSenha.SetActive(false);
        canvasLogin.SetActive(true);
    }

    public void AlternarParaRecuperarSenha()
    {
        canvasLogin.SetActive(false);
        canvasRecuperarSenha.SetActive(true);
    }

    private bool SenhaValida(string senha)
    {
        bool senhaInvalida = senha.Length < 8 ||
                            !System.Text.RegularExpressions.Regex.IsMatch(senha, @"[A-Za-z]") ||
                            !System.Text.RegularExpressions.Regex.IsMatch(senha, @"\d") ||
                            !System.Text.RegularExpressions.Regex.IsMatch(senha, @"[\W_]");

        return !senhaInvalida;
    }

}
