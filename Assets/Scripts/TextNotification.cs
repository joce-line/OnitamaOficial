using TMPro;
using UnityEngine;

public class FeedbackUser
{
    public void ExibeTextoAviso(bool aviso, TextMeshProUGUI elemento)
    {
        elemento.gameObject.SetActive(aviso);
    }

    public void ExibePouUpAviso(bool aviso, GameObject elemento)
    {
        elemento.gameObject.SetActive(aviso);
    }
}