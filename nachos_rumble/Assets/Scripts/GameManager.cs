using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public int scoreToWin;
    Player GameWinner;
    private bool gameEnded;
    
    public static GameManager Instance;

    #region EndScreen
        [SerializeField] GameObject EndScreen;
        [SerializeField] Text TitleText;
        [SerializeField] Text subTitleText;
    #endregion

    void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre GameManager exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        Instance = this;
        
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        Debug.Log($"PV of GameManager is :{PV.Owner is null}");

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
        
        Hashtable H1 = new Hashtable();
        Hashtable H2 = new Hashtable();

        H1.Add("K", 0);
        H2.Add("D", 0);

        PhotonNetwork.LocalPlayer.SetCustomProperties(H1);
        PhotonNetwork.LocalPlayer.SetCustomProperties(H2);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine || gameEnded)
            return;
        
        if (changedProps.ContainsKey("K"))
        {
            if ((int) changedProps["K"] >= scoreToWin)
            {
                gameEnded = true;
                GameWinner = targetPlayer;
                PV.RPC("RPC_EndGame", RpcTarget.All, GameWinner);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (gameEnded && PV.Owner.IsMasterClient)
            PhotonNetwork.CloseConnection(newPlayer); // pour le kick
        
        // En théorie ca serait mieux de créer une properties de Room 'gameEnded', et si elle est True, la room disparait
        // dans la liste de room des menus
    }

    [PunRPC]
    void RPC_EndGame(Player winner)
    {
        StartCoroutine(EndGame(winner)); 
    }
    
    IEnumerator EndGame(Player winner)
    {
        DisplayEndScreen(winner);

        Debug.Log(PlayerData.Instance.globalDeaths);
        Debug.Log(PlayerData.Instance.globalKills);
        
        PlayerData.Instance.SaveProfile();

        for (int i = 8; i > 0; i--)
        {
            subTitleText.text = $"Fin de la partie dans : {i}...";
            yield return new WaitForSecondsRealtime(1);
        }

        QuitRoom();
    }

    void DisplayEndScreen(Player winner)
    {
        // set le text du EndScreen ('{Nickname} won the game !')
        // puis SetActive le EndScreen

        TitleText.text = $"{winner.NickName} won the game !";
        EndScreen.SetActive(true);
    }
    
    void QuitRoom()
    {
        //Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        RoomManager.Instance.transform.SetParent(GameObject.Find("RoomManagerParentToLeaveRoomProperly").transform);
        // on set le parent du RoomManager comme un objet qui lui est détruit OnSceneLoaded
        
        PhotonNetwork.LoadLevel(0);
        SettingsMenu.EnableMouse();
        Destroy(gameObject);
    }
}