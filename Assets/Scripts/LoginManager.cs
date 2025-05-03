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

    [SerializeField] private GameObject telaLogin;
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

            Debug.Log($"Tentando login com usuário: {username}");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                Debug.LogError("Todos os campos são obrigatórios.");
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
                Debug.Log("Usuário ou senha incorretos.");
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
                Debug.Log("Usuário ou senha incorretos.");
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
                Debug.Log("Usuário ou senha incorretos.");
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
        telaLogin.SetActive(false);
        telaCarregamento.SetActive(true);

        AsyncOperation carregamento = SceneManager.LoadSceneAsync(nomeCena);
        carregamento.allowSceneActivation = false;

        float progressoVisual = 0f;

        while (progressoVisual < 1f)
        {
            // Avança até o progresso real (no máximo 0.9)
            float progressoReal = Mathf.Clamp01(carregamento.progress / 0.9f);

            // Simula a barra crescendo suavemente até o progresso real
            if (progressoVisual < progressoReal)
            {
                progressoVisual += Time.deltaTime * 0.3f; // velocidade de crescimento
                progressoVisual = Mathf.Min(progressoVisual, progressoReal); // não passar do real
            }
            // Quando o progresso real chegar em 0.9, completamos o 1.0 visualmente
            else if (carregamento.progress >= 0.9f)
            {
                progressoVisual += Time.deltaTime * 0.3f;
            }

            barraCarregamento.value = progressoVisual;
            textoCarregamento.text = Mathf.RoundToInt(progressoVisual * 100f) + "%";

            yield return null;
        }

        // Espera um instante antes de ativar
        yield return new WaitForSeconds(0.5f);

        carregamento.allowSceneActivation = true;
    }

    public void OpenRegisterScene()
    {
        SceneManager.LoadScene("CriarUsuario");
    }
}
