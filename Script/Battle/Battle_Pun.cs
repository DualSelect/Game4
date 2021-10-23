using Photon.Pun;
using UnityEngine;

public class Battle_Pun : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
    Battle_Rule rule;
    Battle_Player player;

    void IPunInstantiateMagicCallback.OnPhotonInstantiate(PhotonMessageInfo info)
    {
        rule = GameObject.Find("BattleRule").GetComponent<Battle_Rule>();
        rule.pun = this;
        player = GameObject.Find("BattlePlayer").GetComponent<Battle_Player>();
        player.pun = this;
        if (PhotonNetwork.IsMasterClient)
        {
            rule.Initial();
        }
        player.Initial();
    }
 
    public void ProcessRequest(string process, string content)
    {
        photonView.RPC(nameof(ProcessRequestPRC), RpcTarget.All, process,content);
    }
    [PunRPC]
    void ProcessRequestPRC(string process, string content)
    {
        this.player.Receive(process, content);
    }
    public void Answer(string process, string content)
    {
        photonView.RPC(nameof(AnswerPRC), RpcTarget.MasterClient, PhotonNetwork.IsMasterClient, process, content);
    }
    [PunRPC]
    void AnswerPRC(bool player,string process, string content)
    {
        rule.Receive(player, process, content);
    }

}

