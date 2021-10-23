using KanKikuchi.AudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck_Tutorial : MonoBehaviour
{
    public void Close()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(false);
    }
    public void Open()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(true);
    }
    public void Deck()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        var uri = new Uri("https://youtu.be/t5xKFFXhgPA");
        Application.OpenURL(uri.AbsoluteUri);
    }
    public void Battle()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        var uri = new Uri("https://youtu.be/yKPupmkRPQY");
        Application.OpenURL(uri.AbsoluteUri);
    }
    public void Sample()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        var uri = new Uri("https://youtu.be/b7EbY4X16r8");
        Application.OpenURL(uri.AbsoluteUri);
    }
}
