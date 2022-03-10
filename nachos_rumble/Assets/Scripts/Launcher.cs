using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher launcher;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    private void Awake()
    {
        launcher = this;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.menuManager.OpenMenu("title");
        }
        if (PhotonNetwork.IsConnected)
        {
            MenuManager.menuManager.OpenMenu("title");
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true; // pour load le jeu
    }

    public override void OnJoinedLobby()
    {
        MenuManager.menuManager.OpenMenu("title");
        Debug.Log("Joined Lobby");
        PhotonNetwork.NickName = "Player" + Random.Range(0, 1000).ToString("0000"); //pour le moment
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.menuManager.OpenMenu("loading");
        }
    }

    public override void OnJoinedRoom()
    {
        MenuManager.menuManager.OpenMenu("room");
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        
        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // set le bouton startGame à l'host
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed:" + message;
        MenuManager.menuManager.OpenMenu("error");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.menuManager.OpenMenu("loading");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.menuManager.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.menuManager.OpenMenu("title");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        foreach (RoomInfo info in roomList)
        {
            if (!info.RemovedFromList)
                Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(info);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); // load la scène d'index 1 (le jeu)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}