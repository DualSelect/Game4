using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateNote : MonoBehaviour
{
    public Scrollbar bar;

    public void Open()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(true);
    }
    public void Open(int stateNum)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(true);
        float value = 1f / 27f;
        if (stateNum < 10)
        {
            bar.value = 1 - value * stateNum;
        }
        else
        {
            bar.value = 1 - value * (stateNum-1);
        }
    }
    public void Close()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(false);
    }
}
