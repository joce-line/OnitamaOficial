using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusPlayerItem : MonoBehaviour
{
    public TextMeshProUGUI txtNome;
    public TextMeshProUGUI txtStatus;
    private int actorNumber;

    public void ConfigurarPlayer(int actorNumber, string nome, string status)
    {
        this.actorNumber = actorNumber;
        txtNome.text = nome;
        txtStatus.text = status;

    }

    public int GetActorNumber()
    {
        return actorNumber;
    }
}
