using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using PlayerData;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine.UI;


[System.Serializable]
public class ProfileData
{
    public string username;
    public int level;
    public int xp;
    public int globalKill;
    public int globalDeath;

    public ProfileData() //profile par defaut
    {
        this.username = "";
        this.level = 0;
        this.xp = 0;
        this.globalDeath = 0;
        this.globalKill = 0;
    }

    public ProfileData(string u, int l, int x, int k, int d)
    {
        this.username = u;
        this.level = l;
        this.xp = x;
        this.globalDeath = d;
        this.globalKill = k;
    }
}
public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    [SerializeField] Text globalKillsField;
    [SerializeField] Text globalDeathsField;
    [SerializeField] Text KDField;
    public static ProfileData myProfile = new ProfileData();

    private PhotonView PV;
    
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

        myProfile = Data.LoadProfile(); //charger notre profile
        usernameField.text = myProfile.username;
        globalKillsField.text = "Kills : " + myProfile.globalKill;
        globalDeathsField.text = "Deaths : " + myProfile.globalDeath;
        float kd = (float) myProfile.globalKill /  myProfile.globalDeath;
        KDField.text = "K/D : " + $"{kd:0.00}";
        PhotonNetwork.NickName = myProfile.username;
    }

    void Start()
    {
        Debug.Log("Connecting to Master");
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            MenuManager.menuManager.OpenMenu("title");
            Debug.Log("was in room");
        }
        if (PhotonNetwork.IsConnected)
        {
            MenuManager.menuManager.OpenMenu("title");
            Debug.Log("Not in Room but connected");
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
        Debug.Log(usernameField.text);
        
        MenuManager.menuManager.OpenMenu("title");
        Debug.Log("Joined Lobby");
         //pour le moment A FIX !! , demande a paul
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
        
        if (string.IsNullOrEmpty(usernameField.text)) // pour les pseudos
        {
            myProfile.username = "PLAYER_" + Random.Range(100, 1000);
            PhotonNetwork.NickName = myProfile.username;
        }
        else
        {
            myProfile.username = usernameField.text;
            PhotonNetwork.NickName = myProfile.username;
        }
        
        Data.SaveProfile(myProfile); // save le profile

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
        if (string.IsNullOrEmpty(usernameField.text)) // pour les pseudos
        {
            myProfile.username = "PLAYER_" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }
        
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
        if (string.IsNullOrEmpty(usernameField.text)) // pour les pseudos
        {
            myProfile.username = "PLAYER_" + Random.Range(100, 1000);
        }
        else
        {
            myProfile.username = usernameField.text;
        }
        Data.SaveProfile(myProfile); // save le profile
        
        PhotonNetwork.LoadLevel(1); // load la scène d'index 1 (le jeu)
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}