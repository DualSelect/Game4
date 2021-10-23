using BattleJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_Vote : MonoBehaviour
{
    public Text playerName;
    public Text rp;
    public Image charaL;
    public Image charaR;
    public GameObject vote;
    public GameObject macthing;
    public GameObject[] rankColor;
    public IEnumerator Init(Info info)
    {
        macthing.SetActive(false);
        playerName.text = info.name;
        rp.text = "RP"+info.rp.ToString();
        yield return ImageUtil.ImageUtil.IllustImage(info.leftChara, charaL);
        yield return ImageUtil.ImageUtil.IllustImage(info.rightChara, charaR);
        if (info.rp >= 1200) rankColor[0].SetActive(true);
        if (info.rp >= 1500) rankColor[1].SetActive(true);
        if (info.rp >= 1800) rankColor[2].SetActive(true);
        if (info.rp >= 2000) rankColor[3].SetActive(true);
    }
    public void Display()
    {
        vote.SetActive(true);
    }
}
