using KanKikuchi.AudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_News : MonoBehaviour
{
    public Text date;
    public Text title;
    string url;
    public GameObject button;

    public void Init(News news)
    {
        date.text = news.date + "/Ver" + news.ver;
        title.text = news.title;
        url = news.link;
        if (url != "") button.SetActive(true);
    }
    public void Link()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        var uri = new Uri(url);
        Application.OpenURL(uri.AbsoluteUri);
    }
}
