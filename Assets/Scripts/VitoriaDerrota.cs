using Assets.scripts.InfoPlayer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VitoriaDerrota : MonoBehaviour
{
    public GameObject painelVitoria;
    public GameObject painelDerrota;
    public static VitoriaDerrota instance;

    public static VitoriaDerrota GetInstance()
    {
        if (instance == null)
        {
            instance = (VitoriaDerrota)FindFirstObjectByType(typeof(VitoriaDerrota));
        }
        return instance;
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = (VitoriaDerrota)FindFirstObjectByType(typeof(VitoriaDerrota));
            if (instance == null)
            {
                instance = this;
            }
        }
    }

    public void FimDeJogo()
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
}