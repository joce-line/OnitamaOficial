using TMPro;
using UnityEngine;

public class FeedbackUser : MonoBehaviour
{
    public void exibeTextoAviso(bool aviso, TextMeshProUGUI elemento)
    {
        elemento.gameObject.SetActive(aviso);
    }
}