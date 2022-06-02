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
        string time = $"[{date.Hour}h{date.Minute}m{date.Second}s]";
        text.text = $"{time} {player.NickName} {(connectionType ? "entered" : "left")} the room.";
    }
}
