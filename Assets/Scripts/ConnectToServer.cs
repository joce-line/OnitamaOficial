using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Conectando....");
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Já conectado, unindo ao lobby...");
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado!!!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("LobbyTest");
    }
}
