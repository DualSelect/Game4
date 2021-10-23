using BattleJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Battle_Drop : MonoBehaviour
{
    public string area;
    public Battle_Player player;
    public Battle_Card target;
    public void OnDrop(Battle_Card card)
    {
        Debug.Log(area);
        Debug.Log(card.card.title);
        player.DragEnd();

        player.timeClock = false;
        player.mainPhase = false;
        player.turnEndButton.SetActive(false);
        if (area == "delete")
        {
            Act act = new Act();
            if (card.area == "hand") act.act = "handcemetary";
            if (card.area == "field") act.act = "fieldcemetary";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            player.pun.Answer("act", act.ToString());
        }
        if (area == "equip")
        {
            Act act = new Act();
            act.act = "handequip";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            player.pun.Answer("act", act.ToString());
        }
        if (area == "field")
        {
            Act act = new Act();
            act.act = "handfield";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            player.pun.Answer("act", act.ToString());
        }
        if (area == "attack")
        {
            Act act = new Act();
            act.act = "fieldattack";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            player.pun.Answer("act", act.ToString());
        }
        if (area == "merge")
        {
            Act act = new Act();
            act.act = "merge";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            act.orderTarget = target.order;
            player.pun.Answer("act", act.ToString());
        }
        if (area == "generate")
        {
            Act act = new Act();
            act.act = "generate";
            act.player = PhotonNetwork.IsMasterClient;
            act.order = card.order;
            act.isLeft = card.isLeft;
            act.orderTarget = target.order;
            player.pun.Answer("act", act.ToString());
        }
    }
    public IEnumerator Transparent()
    {
        while (true)
        {
            for (int i = 25; i > 13; i--)
            {
                gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0, i / 25f);
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 13; i < 25; i++)
            {
                gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0, i / 25f);
                yield return new WaitForSeconds(0.1f);

            }
        }
    }
}