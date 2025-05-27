using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_Create;
    public TMP_InputField input_Join;

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
        PhotonNetwork.LoadLevel("Game");
    }

    public void JoinRoomList(string RoomName)
    {
        PhotonNetwork.JoinRoom(RoomName);
    }
}
