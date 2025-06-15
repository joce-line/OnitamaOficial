using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;

public class InformacoesUser : MonoBehaviour
{
    public TextMeshProUGUI nomeDoUsuario;
    public TextMeshProUGUI moedas;

    void Start()
    {
        nomeDoUsuario.text = PlayerInfo.nomePlayer;
        moedas.text = PlayerInfo.moeda.ToString();
    }
}
