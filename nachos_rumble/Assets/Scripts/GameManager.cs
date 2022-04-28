using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public int scoreToWin = 5;
    Player GameWinner;
    public static GameManager Instance;
    private bool gameEnded = false;
    
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
        //DontDestroyOnLoad(gameObject); // il est le seul.
        Instance = this;
        
        PV = GetComponent<PhotonView>();

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 240;
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine || gameEnded)
            return;
        
        if (changedProps.ContainsKey("Kills"))
        {
            Debug.Log($"{targetPlayer} has {(int)changedProps["Kills"]} eliminations.");
            
            if ((int) changedProps["Kills"] >= scoreToWin)
            {
                gameEnded = true;
                GameWinner = targetPlayer;
                PV.RPC("RPC_EndGame", RpcTarget.All, GameWinner);
            }
        }
        
        if (changedProps.ContainsKey("Deaths"))
            Debug.Log($"{targetPlayer} has {(int)changedProps["Deaths"]} deaths.");
    }

    [PunRPC]
    void RPC_EndGame(Player winner)
    {
        StartCoroutine(EndGame(winner)); 
    }


    IEnumerator EndGame(Player winner)
    {
        DisplayEndScreen(winner);

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
        Debug.Log($"{winner} has won the game !!");
    }
    
    void QuitRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
    }
    
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(0);
        Debug.Log($"SetingsMenu : OnLeftRoom");
        SettingsMenu.EnableMouse();
        Destroy(gameObject);
    }
}