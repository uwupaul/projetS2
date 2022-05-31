using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScoreManager : MonoBehaviour
{
    public GameObject playerScoreListPrefab;
    
    [SerializeField] private Transform PlayerScoreEntry;
    
    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Instantiate(playerScoreListPrefab, PlayerScoreEntry).GetComponent<PlayScoreScript>().SetUP(player);
        }
    }
}
