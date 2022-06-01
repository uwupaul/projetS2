using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class ConnectionLogItem : MonoBehaviourPun
{
    public TMP_Text text;
    public void SetUp(Player player, bool connectionType)
    {
        text.text = $"{player.NickName} {(connectionType ? "entered" : "left")} the room.";
    }
}
