using UnityEngine;
using Photon.Pun;

public class BoardRotation : MonoBehaviourPunCallbacks
{    void Start()
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
        {
            transform.Rotate(0, 0, 180);
        }
    }
}
