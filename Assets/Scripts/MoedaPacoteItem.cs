using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MoedaPacoteItem : MonoBehaviour
{
    public Image iconePacote;
    public TextMeshProUGUI txtQuantidade;
    public TextMeshProUGUI txtPreco;
    public Button botaoComprar;

    private int quantidadeMoedas;

    public void ConfigurarMoeda(int moedas, string quantidade, float preco, Sprite icone)
    {
        quantidadeMoedas = moedas;
        txtQuantidade.text = quantidade;
        txtPreco.text = $"R$ {preco:F2}";
        iconePacote.sprite = icone;

        botaoComprar.onClick.RemoveAllListeners();
        botaoComprar.onClick.AddListener(() => Loja.GetInstance().ConfirmarCompra(moedas));
    }

}
