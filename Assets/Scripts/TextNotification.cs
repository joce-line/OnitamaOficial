using TMPro;
using UnityEngine;

public class FeedbackUser : MonoBehaviour
{
    public void exibeTextoAviso(bool aviso, TextMeshProUGUI elemento)
    {
        elemento.gameObject.SetActive(aviso);
    }

    public void exibePouUpAviso(bool aviso, GameObject elemento)
    {
        elemento.gameObject.SetActive(aviso);
    }
}