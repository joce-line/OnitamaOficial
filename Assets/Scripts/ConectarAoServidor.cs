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
        // Conecta ao servidor Photon usando as configura��es definidas no PhotonServerSettings
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        // Chamado quando a conex�o com o servidor Photon � bem-sucedida
        UnityEngine.Debug.Log("Conectado ao servidor Photon!");
        // Aqui voc� pode adicionar l�gica para entrar em uma sala ou criar uma nova sala
        PhotonNetwork.JoinLobby(); // Opcional: entra no lobby para listar salas dispon�veis
    }
    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
