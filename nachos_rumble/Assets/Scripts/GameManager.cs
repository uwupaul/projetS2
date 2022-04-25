using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public int scoreToWin = 10;
    Player GameWinner;
    public static GameManager Instance;

    void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre GameManager exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        DontDestroyOnLoad(gameObject); // il est le seul.
        Instance = this;
        PV = GetComponent<PhotonView>();
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine)
            return;
        
        if (changedProps.ContainsKey("Kills"))
        {
            Debug.Log($"{targetPlayer} has {(int)changedProps["Kills"]} eliminations.");
            
            if ((int) changedProps["Kills"] >= scoreToWin)
            {
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
        // faire un écran de fin de partie donnant le nom du vainqueur de la partie
        // attendre 10 secondes puis faire que tlm quitte la game avec une fonction RPC lancée uniquement par le master de la room
        // (voir comment c'est fait dans le settings menu)
        
        //faire que les joueurs ne peuvent plus ni bouger ni tirer, etc

        DisplayEndScreen(winner);

        for (int i = 5; i > 0; i--)
        {
            Debug.Log($"Fin de la partie dans : {i}..");
            yield return new WaitForSecondsRealtime(1);
        }

        QuitRoom();
    }

    void DisplayEndScreen(Player winner)
    {
        Debug.Log($"{winner} has won the game !!");
    }
    
    void QuitRoom()
    {
        Destroy(RoomManager.Instance.gameObject);
        PhotonNetwork.LeaveRoom();
        // il faut remettre la souris et le curseur
    }
    
    public override void OnLeftRoom()
    {
        Debug.Log($"SetingsMenu : OnLeftRoom");
        SettingsMenu.EnableMouse();
        PhotonNetwork.LoadLevel(0);
        Destroy(gameObject);
    }
}