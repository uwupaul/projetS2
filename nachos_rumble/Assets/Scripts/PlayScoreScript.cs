using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayScoreScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private Text name_text;
    [SerializeField] private Text death_text;
    [SerializeField] private Text kills_text;
    
    private PhotonView PV;
    private Player player;

    public void SetUP(Player _player)
    {
        player = _player;
        name_text.text = _player.NickName;
        kills_text.text = "0";
        death_text.text = "0";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        
        if (changedProps.ContainsKey("Kills"))
        {
            transform.Find("Kills").GetComponent<Text>().text = Convert.ToString((int)changedProps["Kills"]);
        }
        
        if (changedProps.ContainsKey("Death"))
        {
            transform.Find("Death").GetComponent<Text>().text = Convert.ToString((int)changedProps["Death"]);
        }
    }
    
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (player.Equals(otherPlayer))
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
