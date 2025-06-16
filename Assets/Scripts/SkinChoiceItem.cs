using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SkinChoiceItem : MonoBehaviour
{
    public GameObject skinItem;
    public TextMeshProUGUI nomeItem;
    public Image pawnImage;
    public Image kingImage;
    public Button selecionarItem;
    public Image selectionBorder;
    private int itemId;
    private string caminhoPawn;
    private string caminhoKing;
    private bool isSelected;
    public int ItemId => itemId;
    public string CaminhoPawn => caminhoPawn;
    public string CaminhoKing => caminhoKing;
    public bool IsSelected => isSelected;

    public void ConfigurarItem(int idItem, string nome, string caminhoPawn, string caminhoKing, bool jaSelecionado)
    {
        this.itemId = idItem;
        this.caminhoPawn = caminhoPawn;
        this.caminhoKing = caminhoKing;
        nomeItem.text = nome;
        selecionarItem.interactable = !jaSelecionado;
        isSelected = false;
        selectionBorder.gameObject.SetActive(false);

        // Carregar imagens
        StartCoroutine(LoadImage(caminhoPawn, pawnImage));
        StartCoroutine(LoadImage(caminhoKing, kingImage));

        // Adiciona a ação do botão aqui
        selecionarItem.onClick.RemoveAllListeners();
        selecionarItem.onClick.AddListener(() =>
        {
            LobbyManager.GetInstance().ConfirmarEscolha(itemId, !isSelected);

            selectionBorder.gameObject.SetActive(true);
        });
    }

    public void SetSelected(bool isSelected)
    {
        this.isSelected = isSelected;
        selectionBorder.gameObject.SetActive(isSelected);
    }

    private IEnumerator LoadImage(string url, Image image)
    {
        if (string.IsNullOrEmpty(url))
        {
            image.sprite = Resources.Load<Sprite>("Skins/default_skin");
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Erro ao carregar imagem {url}: {request.error}");
            image.sprite = Resources.Load<Sprite>("Skins/default_skin");
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
    }
}
