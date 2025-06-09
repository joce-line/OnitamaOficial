using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject telaCarregamento;
    public TextMeshProUGUI textoCarregamento;
    public void CarregarCena(string nomeCena, GameObject telaAntiga)
    {
        StartCoroutine(CarregarCenaAssincrona(nomeCena, telaAntiga));
    }

    private IEnumerator CarregarCenaAssincrona(string nomeCena, GameObject telaAntiga)
    {
        if (telaAntiga != null)
            telaAntiga.SetActive(false);

        if (telaCarregamento != null)
            telaCarregamento.SetActive(true);

        AsyncOperation carregamento = SceneManager.LoadSceneAsync(nomeCena);
        carregamento.allowSceneActivation = false;

        float progressoVisual = 0f;
        while (!carregamento.isDone)
        {
            float progressoReal = carregamento.progress;

            if (progressoVisual < progressoReal)
            {
                progressoVisual += Time.deltaTime * 0.5f;
            }

            textoCarregamento.text = Mathf.RoundToInt(progressoVisual * 100f) + "%";

            if (progressoVisual >= 0.9f)
            {
                textoCarregamento.text = "Carregando...";

                carregamento.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}