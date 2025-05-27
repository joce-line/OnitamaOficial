using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class Room : MonoBehaviourPunCallbacks
{
    public Text Name;

    public void JoinRoom()
    {
        GameObject.Find("CreateAndJoin").GetComponent<CreateAndJoin>().JoinRoomList(Name.text);
    }
}