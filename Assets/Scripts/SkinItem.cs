using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SkinItem : MonoBehaviour
{
    public GameObject skinItem;
    public TextMeshProUGUI nomeItem;
    public TextMeshProUGUI precoItem;
    public SpriteRenderer pawnSprite;
    public SpriteRenderer kingSprite;
    public Button comprarItem;
    private int itemId;

    public void ConfigurarItem(int idItem, string nome, int preco, string caminhoPawn, string caminhoKing, bool jaComprado)
    {
        this.itemId = idItem;
        nomeItem.text = nome;
        precoItem.text = preco.ToString();
        comprarItem.interactable = !jaComprado;

        // Carregar imagens
        StartCoroutine(LoadImage(caminhoPawn, pawnSprite));
        StartCoroutine(LoadImage(caminhoKing, kingSprite));

        // Adiciona a ação do botão aqui
        comprarItem.onClick.RemoveAllListeners();
        comprarItem.onClick.AddListener(() => Loja.GetInstance().ConfirmarCompra(itemId));
    }

    private IEnumerator LoadImage(string url, SpriteRenderer spriteRenderer)
    {
        if (string.IsNullOrEmpty(url))
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Skins/default_skin");
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Erro ao carregar imagem {url}: {request.error}");
            spriteRenderer.sprite = Resources.Load<Sprite>("Skins/default_skin");
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = sprite;
    }

}
