using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deck_Button : MonoBehaviour
{
    public void BattleButton()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        SceneManager.LoadScene("Battle");
    }
}
