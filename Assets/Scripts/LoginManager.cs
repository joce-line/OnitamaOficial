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

    public TextMeshProUGUI avisoCampos;
    public TextMeshProUGUI avisoDadosIncorretos;

    [SerializeField] private GameObject telaCarregamento;
    [SerializeField] private Slider barraCarregamento;
    [SerializeField] private TextMeshProUGUI textoCarregamento;

    HashSenha valida = new HashSenha(SHA512.Create());
    public void Login()
    {
        FeedbackUser aviso = new FeedbackUser();

        try
        {
            if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
            {
                Debug.LogError("DatabaseManager não inicializado.");
                return;
            }

            string username = usernameInput.text;
            string password = passwordInput.text;

            //Debug.Log($"Tentando login com usuário: {username}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                //Debug.LogError("Todos os campos são obrigatórios.");
                aviso.exibeTextoAviso(true, avisoCampos);
                return;
            }
            else
            {
                aviso.exibeTextoAviso(false, avisoCampos);
            }

            //Verifica usuario
            string userQuery = "SELECT COUNT(*) FROM usuarios WHERE nickname=@user";
            var userParams = new Dictionary<string, object> { { "@user", username } };
            long userExists = Convert.ToInt64(DatabaseManager.Instance.ExecuteScalar(userQuery, userParams));

            if (userExists == 0 || userExists == -1)
            {
                //Debug.Log("Usuário ou senha incorretos.");
                aviso.exibeTextoAviso(true, avisoDadosIncorretos);
                return;
            }
            else
            {
                aviso.exibeTextoAviso(false, avisoDadosIncorretos);
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
                //Debug.Log("Usuário ou senha incorretos.");
                aviso.exibeTextoAviso(true, avisoDadosIncorretos);
                return;
            }
            else
            {
                aviso.exibeTextoAviso(false, avisoDadosIncorretos);
            }

                string comparaSenhaBanco = result?.ToString();
            if (valida.VerificarSenha(password, comparaSenhaBanco))
            {
                Debug.Log("Login bem-sucedido!");
                PlayerInfo.nomePlayer = username;
                //SceneManager.LoadScene("Game");
                StartCoroutine(Loading("Game"));
            }
            else
            {
                //Debug.Log("Usuário ou senha incorretos.");
                aviso.exibeTextoAviso(true, avisoDadosIncorretos);
            }

        }
        catch (Exception e)
        {
            Debug.LogError($"Erro ao realizar login: {e.Message}");
        }
    }

    private IEnumerator Loading(string nomeCena)
    {
        AsyncOperation carregamento = SceneManager.LoadSceneAsync(nomeCena);

        telaCarregamento.SetActive(true);

        while (!carregamento.isDone)
        {
            float progressoCarregamento = Mathf.Clamp01(carregamento.progress / 0.9f);
            //Debug.Log(progressoCarregamento);

            barraCarregamento.value = progressoCarregamento;
            textoCarregamento.text = progressoCarregamento * 100 + "%";

            yield return null;
        }
    }

    public void OpenRegisterScene()
    {
        SceneManager.LoadScene("CriarUsuario");
    }
}
