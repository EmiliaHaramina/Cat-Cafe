using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CatCafeNetworkManager : MonoBehaviourPunCallbacks {

    public GameObject userTable;

    public override void OnJoinedRoom() {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            Debug.Log("Initialized cats because both players joined.");
        } else {
            Debug.Log("You are the first player to join.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("A new player joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 2) {
            Debug.Log(PhotonNetwork.CurrentRoom.ToStringFull());
        }
    }

}
