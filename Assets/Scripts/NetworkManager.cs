using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class DefaultRoom {
    public string Name;
    public int sceneIndex;
    public int maxPlayer;
}

public class NetworkManager : MonoBehaviourPunCallbacks {

    public List<DefaultRoom> defaultRooms;
    public GameObject lobbies;
    public GameObject main;

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
        main.SetActive(false);
        Debug.Log("Trying to connect to the server...");
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to server.");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        base.OnJoinedLobby();
        Debug.Log("WE JOINED THE LOBBY");
        lobbies.SetActive(true);
    }

    public void InitializeRoom(int defaultRoomIndex) {
        DefaultRoom roomSettings = defaultRooms[defaultRoomIndex];

        //LOAD SCENE
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(roomSettings.sceneIndex);

        // CREATE THE ROOM
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) roomSettings.maxPlayer;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom(roomSettings.Name, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined a room.");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("A new player joined the room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
