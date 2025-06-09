using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TelaTitulo : MonoBehaviour
{
    public void Continuar()
    {
        SceneManager.LoadScene("Login");
    }
}

