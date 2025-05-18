using TMPro;
using UnityEngine;
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


    public void SetData(int id, string nome, int preco)
    {
        itemId = id;
        nomeItem.text = nome;
        precoItem.text = $"{preco}";

        pawnSprite.sprite = Resources.Load<Sprite>("Sprites/default_skin");
        kingSprite.sprite = Resources.Load<Sprite>("Sprites/default_skin");

        comprarItem.onClick.RemoveAllListeners();
        //comprarItem.onClick.AddListener(() => ScriptLoja.GetInstance().ComprarItem(itemId));
        comprarItem.onClick.AddListener(() => Loja.GetInstance().ConfirmarCompra(itemId));

    }

    public void ConfigurarItem(int idItem, string nome, int preco, bool jaComprado)
    {
        this.itemId = idItem;
        nomeItem.text = nome;
        precoItem.text = preco.ToString();
        comprarItem.interactable = !jaComprado;

        // Adiciona a ação do botão aqui
        comprarItem.onClick.RemoveAllListeners();
        comprarItem.onClick.AddListener(() => Loja.GetInstance().ConfirmarCompra(itemId));
    }

}
