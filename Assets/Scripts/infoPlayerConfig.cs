using UnityEngine;
using TMPro;
using Assets.scripts.InfoPlayer;

public class infoPlayerCOnfig : MonoBehaviour
{
    public TextMeshProUGUI nomeDoUsuarioConfig;
    public TextMeshProUGUI emailConfig;
    public TextMeshProUGUI moedasConfig;

    void Start()
    {
        nomeDoUsuarioConfig.text = PlayerInfo.nomePlayer;
        emailConfig.text = PlayerInfo.email;
        moedasConfig.text = PlayerInfo.moeda.ToString();
    }
}
