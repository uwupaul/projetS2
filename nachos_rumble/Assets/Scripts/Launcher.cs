using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Launcher : MonoBehaviourPunCallbacks
{
    public InputField usernameField;
    [SerializeField] Text globalKillsField;
    [SerializeField] Text globalDeathsField;
    [SerializeField] Text KDField;
    [SerializeField] TextMeshProUGUI AINumber;
    
    public static Launcher launcher;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] GameObject NoUsernamePopup;
    [SerializeField] GameObject AIManagement;

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
        
        launcher = this;

        PlayerData.Instance.LoadProfile(); //charger notre profileData
        
        float kd = PlayerData.Instance.globalDeaths == 0 ? PlayerData.Instance.globalKills :
            (float) PlayerData.Instance.globalKills /  PlayerData.Instance.globalDeaths;
        
        usernameField.text = PlayerData.Instance.username;
        globalKillsField.text = "Kills : " + PlayerData.Instance.globalKills;
        globalDeathsField.text = "Deaths : " + PlayerData.Instance.globalDeaths;
        KDField.text = "K/D : " + $"{kd:0.00}";
        
        PhotonNetwork.NickName = PlayerData.Instance.username;
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
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text);
            MenuManager.menuManager.OpenMenu("loading");
        }
    }

    public void ChangeUsername(string username)
    {
        PlayerData.Instance.username = username;
        PhotonNetwork.NickName = username;
        PlayerData.Instance.SaveProfile();
    }
    
    public void UsernameCheck(string menu)
    {
        ChangeUsername(usernameField.text);
        
        if (string.IsNullOrEmpty(PlayerData.Instance.username))
            NoUsernamePopup.SetActive(true);
        else
            MenuManager.menuManager.OpenMenu(menu);
    }
    
    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.menuManager.OpenMenu("room");

        foreach (Transform child in playerListContent)
        {
            // On détruit tout ce qui est déjà présent dans la liste de joueur (pour reset)
            Destroy(child.gameObject);
        }
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // On reconstruit la liste de joueur
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }
        
        startGameButton.SetActive(PhotonNetwork.IsMasterClient); // set le bouton startGame à l'host
        AIManagement.SetActive(PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient) {
            Hashtable h = new Hashtable {{"AI", 0}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
            
            // Pour créer la properties au cas ou on ne veut pas d'IA dans la room
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        AIManagement.SetActive(PhotonNetwork.IsMasterClient);
        
        SetAINumberText((int)PhotonNetwork.CurrentRoom.CustomProperties["AI"]);
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

    #region AI Management

        public void SetAINumberText(int n)
        {
            AINumber.text = n.ToString();
        }
        public void IncreaseAICount()
        {
            int n = Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["AI"]);
            
            if (n >= 4)
                return;
            
            SetAINumberText(n+1);
            Hashtable h = new Hashtable {{"AI", n + 1}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
        }
        
        public void DecreaseAICount()
        {
            int n = Convert.ToInt32(PhotonNetwork.CurrentRoom.CustomProperties["AI"]);
            
            if (n <= 0)
                return;
            
            SetAINumberText(n-1);
            Hashtable h = new Hashtable {{"AI", n - 1}};
            PhotonNetwork.CurrentRoom.SetCustomProperties(h);
        }

    #endregion
}