using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class PlayerScoreBoardItem : MonoBehaviour
{
    
    [SerializeField] private Text name_text;
    [SerializeField] private Text death_text;
    [SerializeField] private Text kills_text;
    
    
    public void SetUP(string username, int kills, int deaths)
    {
        name_text.text = username;
        kills_text.text = "" + kills;
        death_text.text = "" + deaths;
    }
}
