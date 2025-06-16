using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Assets.scripts.InfoPlayer;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_Create;
    public TMP_InputField input_Join;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        PhotonNetwork.AutomaticallySyncScene = true;
        if (!string.IsNullOrEmpty(PlayerInfo.nomePlayer))
        {
            PhotonNetwork.NickName = PlayerInfo.nomePlayer;
        }
        else
        {
            PhotonNetwork.NickName = $"Jogador_{Random.Range(1000, 9999)}";
            Debug.LogWarning("nomePlayer não definido, usando nome aleatório.");
        }
    }

    public void CreateRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(WaitForConnectionAndCreateRoom(input_Create.text));
        }
        else
        {
            PhotonNetwork.CreateRoom(input_Create.text, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true });
        }
    }

    public void JoinRoom()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            StartCoroutine(WaitForConnectionAndJoinRoom(input_Join.text));
        }
        else
        {
            PhotonNetwork.JoinRoom(input_Join.text);
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} entrou na sala");
        PhotonNetwork.LoadLevel("Lobby");
    }

    public void JoinRoomList(string RoomName)
    {
        PhotonNetwork.JoinRoom(RoomName);
    }

    private IEnumerator WaitForConnectionAndCreateRoom(string roomName)
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true });
    }

    private IEnumerator WaitForConnectionAndJoinRoom(string roomName)
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entrou na salinha");
    }

    public bool IsRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    public void Voltar()
    {
        PhotonNetwork.LeaveRoom();
        StartCoroutine(GameManager.instance.WaitForLeaveRoomAndDisconnect());
    }
}
