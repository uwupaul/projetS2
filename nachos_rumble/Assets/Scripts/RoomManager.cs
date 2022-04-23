using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        if (Instance && Instance != this)// verifie si une autre roomManager exists
        {
            Destroy(gameObject); // il ne peut y en avoir que un 
            return;
        }
        DontDestroyOnLoad(gameObject); // il est le seul.
        Instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.buildIndex == 1) // on est dans la scene
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero,
                Quaternion.identity);
            // on instancie le playerManager qui nous servira à instantier le playerController et à le gérer, etc.
            
            InstantiateAI(); // fait spawn une IA (par personne?)
        }
    }

    void InstantiateAI()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "AIManager"), Vector3.zero,
            Quaternion.identity);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Deaths"))
            Debug.Log($"{targetPlayer.NickName} died {(int)changedProps["Deaths"]} times.");
    }
}
