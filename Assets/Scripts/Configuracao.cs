using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Assets.scripts.InfoPlayer;
using System.Collections.Generic;
using System.Collections;
using System;

public class Configuracao : MonoBehaviour
{
    public TMP_Dropdown dropdownTemas;
    public Image miniaturaPreview; // <- Miniatura no Inspector
    public GameObject panelConfig, panelDadosUsu;

    private List<int> backgroundIds = new List<int>();
    private List<string> backgroundNomes = new List<string>();
    private int selectedBackgroundId;

    void Start()
    {
        if (DatabaseManager.Instance == null || string.IsNullOrEmpty(DatabaseManager.ConnectionString))
        {
            Debug.LogError("DatabaseManager não inicializado.");
            return;
        }

        CarregarBackgrounds();
    }

    private void CarregarBackgrounds()
    {
        if (dropdownTemas == null)
        {
            Debug.LogError("DropdownTemas não está atribuído no Inspector.");
            return;
        }

        dropdownTemas.ClearOptions();
        backgroundIds.Clear();
        backgroundNomes.Clear();

        var backgrounds = DatabaseManager.Instance.GetBackgroundsAtivos();

        foreach (var bg in backgrounds)
        {
            int id = Convert.ToInt32(bg["id_Background"]);
            string nome = bg["nome"].ToString();

            backgroundIds.Add(id);
            backgroundNomes.Add(nome);
        }

        dropdownTemas.AddOptions(backgroundNomes);
    }

    public void OnDropValueChanged(int index)
    {
        if (index >= 0 && index < backgroundIds.Count)
        {
            selectedBackgroundId = backgroundIds[index];
            Debug.Log("ID do background selecionado: " + selectedBackgroundId);

            string caminho = DatabaseManager.Instance.GetCaminhoDoBackground(selectedBackgroundId);

            if (!string.IsNullOrEmpty(caminho))
            {
                StartCoroutine(CarregarMiniatura(caminho));
            }
        }
        else
        {
            Debug.LogWarning("Índice inválido do dropdown. Seleção ignorada.");
        }
    }

    private IEnumerator CarregarMiniatura(string url)
    {
        if (miniaturaPreview == null)
        {
            Debug.LogWarning("MiniaturaPreview não atribuída no Inspector.");
            yield break;
        }

        if (string.IsNullOrEmpty(url) || !url.StartsWith("http"))
        {
            Sprite defaultSprite = Resources.Load<Sprite>("Backgrounds/defaultBG");
            miniaturaPreview.sprite = defaultSprite;
            yield break;
        }

        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                miniaturaPreview.sprite = sprite;
                Debug.Log("Miniatura atualizada com sucesso.");
            }
            else
            {
                Debug.LogWarning("Erro ao carregar imagem da miniatura.");
                Sprite defaultSprite = Resources.Load<Sprite>("Backgrounds/defaultBG");
                miniaturaPreview.sprite = defaultSprite;
            }
        }
    }

    public void VoltarButton()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }

    //botões de seleção
    public void ConfigButton()
    {
        panelConfig.SetActive(true);
        panelDadosUsu.SetActive(false);
    }

    public void DadoUsuButton()
    {
        panelConfig.SetActive(false);
        panelDadosUsu.SetActive(true);
    }

    public void ConfirmarButton()
    {
        int indexSelecionado = dropdownTemas.value;
        selectedBackgroundId = backgroundIds[indexSelecionado];

        Debug.Log("ID do tema selecionado (antes da verificação): " + selectedBackgroundId);

        if (!backgroundIds.Contains(selectedBackgroundId))
        {
            Debug.LogWarning("ID do tema selecionado não encontrado na lista. Aplicando fallback para ID 1.");
            selectedBackgroundId = 1;
        }

        PlayerInfo.id_Background = selectedBackgroundId;

        string updateQuery = "UPDATE usuarios SET id_Background = @bg WHERE idUsuario = @id";
        var parametros = new Dictionary<string, object>
        {
            { "@bg", selectedBackgroundId },
            { "@id", PlayerInfo.idPlayer }
        };

        if (DatabaseManager.Instance.ExecuteNonQuery(updateQuery, parametros))
        {
            Debug.Log("Background do usuário salvo com sucesso.");
        }
        else
        {
            Debug.LogError("Erro ao salvar background do usuário.");
        }

        string caminho = DatabaseManager.Instance.GetCaminhoDoBackground(selectedBackgroundId);
        PlayerInfo.caminho_Background = caminho;

        if (string.IsNullOrEmpty(caminho))
        {
            Debug.LogWarning("Caminho do background vazio, usando defaultBG.");
        }

        BackgroundController.Instance.AtualizarBackground(selectedBackgroundId, caminho);
    }
}
