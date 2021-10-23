using KanKikuchi.AudioManager;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Battle_Header : MonoBehaviour
{
    public Dropdown bookDrop;
    public Image[] rankColor;
    public Text rateText;
    public GameObject rate;
    void Start()
    {
        int rate = PlayerPrefs.GetInt("RP", 1000);
        int maxRate = PlayerPrefs.GetInt("MaxRP", 1000);
        if (rate >= 1200) rankColor[0].color = new Color(1, 1, 1, 1);
        if (rate >= 1500) rankColor[1].color = new Color(1, 1, 1, 1);
        if (rate >= 1800) rankColor[2].color = new Color(1, 1, 1, 1);
        if (rate >= 2000) rankColor[3].color = new Color(1, 1, 1, 1);
        rateText.text = "現在:" + rate + "rp" + "\n" + "最高:" + maxRate + "rp";
        BGMManager.Instance.Play(BGMPath.MINSTREL2_EPIC_ORCHE);
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < 30; i++)
        {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = "デッキ選択:" + (i + 1) + "   " + PlayerPrefs.GetString("DeckName" + i, "新規デッキ");
            options.Add(option);
        }
        bookDrop.options = options;
        bookDrop.value = PlayerPrefs.GetInt("SelectDeck", 0);
    }
    public void BookSelect(int i)
    {
        PlayerPrefs.SetInt("SelectDeck", i);
    }
    public void DeckButton()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Deck");
    }
    public void RateButton()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        rate.SetActive(true);
    }
}