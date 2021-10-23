using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck_Kiyaku : MonoBehaviour
{

    public GameObject kiyaku;
    public GameObject tuto;
    void Start()
    {
        if (PlayerPrefs.GetInt("kiyaku", 0) == 0)
        {
            kiyaku.SetActive(true);
        }
        if (PlayerPrefs.GetInt("kiyaku", 0) <= 1)
        {
            tuto.SetActive(true);
        }
    }

    public void Close()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        PlayerPrefs.SetInt("kiyaku", 1);
        kiyaku.SetActive(false);
    }
    public void TutoClose()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        PlayerPrefs.SetInt("kiyaku", 2);
        tuto.SetActive(false);
    }
}
