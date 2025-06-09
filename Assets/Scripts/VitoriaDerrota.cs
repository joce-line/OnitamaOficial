using Assets.scripts.InfoPlayer;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VitoriaDerrota : MonoBehaviour, IOnEventCallback
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

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            object[] data = (object[])photonEvent.CustomData;
            int vencedor = (int)data[0];
            int perdedor = (int)data[1];

            if (PhotonNetwork.LocalPlayer.ActorNumber == vencedor)
            {
                painelVitoria.SetActive(true);
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == perdedor)
            {
                painelDerrota.SetActive(true);
            }

            FindAnyObjectByType<InputManager>().enabled = false;
        }
    }
}