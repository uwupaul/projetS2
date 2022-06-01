using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ConnectionLogItem : MonoBehaviourPun
{
    public TMP_Text text;
    public void SetUp(Player player, bool connectionType)
    {
        DateTime date = DateTime.Now;
        string time = $"{date.Hour}:{date.Minute}:{date.Second}";
        text.text = $"{time} : {player.NickName} {(connectionType ? "entered" : "left")} the room.";
    }
}
