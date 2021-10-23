using KanKikuchi.AudioManager;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Effect_Result : MonoBehaviour
{
    public SimpleAnimation win;
    public SimpleAnimation lose;
    public SimpleAnimation rankUp;
    public Text rp;
    public Text result;
    public Text total;
    public GameObject[] prev;
    public GameObject[] next;

    public IEnumerator Win(int rp,int result,int total)
    {
        BGMManager.Instance.Play(BGMPath.MINSTREL2_EPIC_ORCHE);
        int prevRank;
        int nextRank;
        win.gameObject.SetActive(true);
        SEManager.Instance.Play(SEPath.WIN);
        win.Play();
        yield return new WaitForSecondsRealtime(1f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.rp.text = rp.ToString();
        if (rp < 1200)
        {
            prevRank = 0;
        }else if(rp < 1500)
        {
            prevRank = 1;
            prev[0].SetActive(true);
        }
        else if (rp < 1800)
        {
            prevRank = 2;
            prev[1].SetActive(true);
        }
        else if (rp < 200)
        {
            prevRank = 3;
            prev[2].SetActive(true);
        }
        else
        {
            prevRank = 4;
            prev[3].SetActive(true);
        }
        yield return new WaitForSecondsRealtime(0.3f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.result.text = result.ToString();
        yield return new WaitForSecondsRealtime(0.3f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.total.text = total.ToString();
        if (rp < 1200)
        {
            nextRank = 0;
        }else if(rp < 1500)
        {
            nextRank = 1;
            next[0].SetActive(true);
        }
        else if (rp < 1800)
        {
            nextRank = 2;
            next[1].SetActive(true);
        }
        else if (rp < 200)
        {
            nextRank = 3;
            next[2].SetActive(true);
        }
        else
        {
            nextRank = 4;
            next[3].SetActive(true);
        }
        yield return new WaitForSecondsRealtime(0.3f);
        if (nextRank > prevRank)
        {
            SEManager.Instance.Play(SEPath.WIN);
            rankUp.Play();
        }
    }
    public IEnumerator Lose(int rp, int result, int total)
    {
        BGMManager.Instance.Play(BGMPath.MINSTREL2_HARP);
        int prevRank;
        int nextRank;
        lose.gameObject.SetActive(true);
        lose.Play();
        yield return new WaitForSecondsRealtime(1f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.rp.text = rp.ToString();
        if (rp < 1200)
        {
            prevRank = 0;
        }
        else if (rp < 1500)
        {
            prevRank = 1;
            prev[0].SetActive(true);
        }
        else if (rp < 1800)
        {
            prevRank = 2;
            prev[1].SetActive(true);
        }
        else if (rp < 200)
        {
            prevRank = 3;
            prev[2].SetActive(true);
        }
        else
        {
            prevRank = 4;
            prev[3].SetActive(true);
        }
        yield return new WaitForSecondsRealtime(0.3f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.result.text = result.ToString();
        yield return new WaitForSecondsRealtime(0.3f);
        SEManager.Instance.Play(SEPath.CARDIN);
        this.total.text = total.ToString();
        if (rp < 1200)
        {
            nextRank = 0;
        }
        else if (rp < 1500)
        {
            nextRank = 1;
            next[0].SetActive(true);
        }
        else if (rp < 1800)
        {
            nextRank = 2;
            next[1].SetActive(true);
        }
        else if (rp < 200)
        {
            nextRank = 3;
            next[2].SetActive(true);
        }
        else
        {
            nextRank = 4;
            next[3].SetActive(true);
        }
        yield return new WaitForSecondsRealtime(0.3f);
        if (nextRank > prevRank)
        {
            SEManager.Instance.Play(SEPath.WIN);
            rankUp.Play();
        }
    }
    public void FreeWin()
    {
        BGMManager.Instance.Play(BGMPath.MINSTREL2_EPIC_ORCHE);
        SEManager.Instance.Play(SEPath.WIN);
        win.gameObject.SetActive(true);
        win.Play();
        this.rp.text = "ïœçXÇ»Çµ";
    }
    public void FreeLose()
    {
        BGMManager.Instance.Play(BGMPath.MINSTREL2_HARP);
        lose.gameObject.SetActive(true);
        lose.Play();
        this.rp.text = "ïœçXÇ»Çµ";
    }
    public void EndButton()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Deck");
    }
}
