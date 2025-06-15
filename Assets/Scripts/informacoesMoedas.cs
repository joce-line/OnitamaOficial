using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;

public class InformacoesMoedas : MonoBehaviour
{
    public TextMeshProUGUI moedas;

    void Start()
    {
        moedas.text = PlayerInfo.moeda.ToString();
    }

    public void AtualizaMoedas()
    {
        moedas.text = PlayerInfo.moeda.ToString();
    }
}
