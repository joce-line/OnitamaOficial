using Assets.scripts.InfoPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VitoriaDerrota : MonoBehaviour
{
    public GameObject painelVitoria;
    public GameObject painelDerrota;

    void Start()
    {
        // Logica pega id do estatico para saber vencedor/perdedor
        //TODO: mudar apos implementação multiplayer
        int vencedor = DadosJogo.vencedor;
        int perdedor = DadosJogo.perdedor;

        if (vencedor == 1)
        {
            painelVitoria.SetActive(true);
            painelDerrota.SetActive(false);
        }
        else if (vencedor == 2)
        {
            painelVitoria.SetActive(false);
            painelDerrota.SetActive(true);
        }
    }

    //TODO: modificar apos logica de paginas
    public void Voltar()
    {
        SceneManager.LoadScene("Game");
    }

    //TODO: modificar apos logica de paginas
    public void Sair()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}