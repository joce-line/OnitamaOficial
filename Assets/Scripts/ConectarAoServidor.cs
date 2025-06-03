using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConectarAoServidor : MonoBehaviourPunCallbacks
{
    void Start()
    {
        UnityEngine.Debug.Log("Conectando...");
        // Conecta ao servidor Photon usando as configurações definidas no PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        // Chamado quando a conexão com o servidor Photon é bem-sucedida
        UnityEngine.Debug.Log("Conectado ao servidor Photon!");
        // Aqui você pode adicionar lógica para entrar em uma sala ou criar uma nova sala
        PhotonNetwork.JoinLobby(); // Opcional: entra no lobby para listar salas disponíveis
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
