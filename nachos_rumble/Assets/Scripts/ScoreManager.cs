using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject playerScoreBoardItem;
    
    [SerializeField] private Transform playerScoreBoardList;
    
    void OnEnable()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject itemGO = (GameObject) Instantiate(playerScoreBoardItem, playerScoreBoardList);
            PlayerScoreBoardItem item = itemGO.GetComponent<PlayerScoreBoardItem>();
            
            if (item != null)
            {
                item.SetUP(player.NickName,(int) player.CustomProperties["K"], (int) player.CustomProperties["D"]);
            }
        }
    }
    
    private void OnDisable()
    {
        foreach (Transform child in playerScoreBoardList)
        {
            Destroy(child.gameObject);
        }
    }
}
