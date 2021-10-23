using KanKikuchi.AudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_Option : MonoBehaviour
{
    public GameObject[] content;
    public Button[] bgm;
    public Button[] se;


    void Start()
    {
        SoundButton();
    }
    public void Button(int i)
    {
        foreach (GameObject gameObject in content) gameObject.SetActive(false);
        content[i].SetActive(true);
    }
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
    public void BGM(int i)
    {
        PlayerPrefs.SetInt("BGM", i);
        BGMManager.Instance.ChangeBaseVolume(i*0.2f);
        SoundButton();
        SEManager.Instance.Play(SEPath.CLICK);
    }
    public void SE(int i)
    {
        PlayerPrefs.SetInt("SE", i);
        SEManager.Instance.ChangeBaseVolume(i * 0.2f);
        SoundButton();
        SEManager.Instance.Play(SEPath.CLICK);
    }
    void SoundButton()
    {
        foreach (Button button in bgm) button.interactable = true;
        foreach (Button button in se) button.interactable = true;
        bgm[PlayerPrefs.GetInt("BGM", 2)].interactable = false;
        se[PlayerPrefs.GetInt("SE", 3)].interactable = false;
    }
    public void Twitter()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        var uri = new Uri("https://twitter.com/dual_select");
        Application.OpenURL(uri.AbsoluteUri);
    }
}
