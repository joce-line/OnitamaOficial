using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Assets.scripts.InfoPlayer;

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
        PhotonNetwork.CreateRoom(input_Create.text, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true });
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(input_Join.text);
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entrou na salinha");
    }

    public bool IsRoomFull()
    {
        Debug.Log("IsRoomFull Player Count = " + PhotonNetwork.CurrentRoom.PlayerCount);
        Debug.Log("IsRoomFull Max players = " + PhotonNetwork.CurrentRoom.MaxPlayers);
        return PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;
    }
}
