using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_Status : MonoBehaviour
{
    public Image window;
    public Card card;
    public Image illust;
    public Text type;
    public Text title;
    public Image color;
    public Text cost;
    public Text skill1;
    public Text skill2;
    public Text skill3;
    public Text tribe1;
    public Text tribe2;
    public Text power1;
    public Text power2;
    public Text power3;
    public Text text;
    public Text text0;
    public Text text1;
    public Text text2;
    public StateNote state;

    public void StatusDisplay(Card card)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        this.card = card;
        type.text = card.type;
        title.text = card.title;
        cost.text = card.cost.ToString();
        skill1.text = card.skill1;
        skill2.text = card.skill2;
        skill3.text = card.skill3;
        if (skill1.text == "") skill1.gameObject.SetActive(false);
        else skill1.gameObject.SetActive(true);
        if (skill2.text == "") skill2.gameObject.SetActive(false);
        else skill2.gameObject.SetActive(true);
        if (skill3.text == "") skill3.gameObject.SetActive(false);
        else skill3.gameObject.SetActive(true);
        tribe1.text = card.tribe1;
        tribe2.text = card.tribe2;
        if (card.type == "ƒ†ƒjƒbƒg" || card.type == "i‰»ƒ†ƒjƒbƒg")
        {
            power1.text = card.power1.ToString();
            power2.text = card.power2.ToString();
            power3.text = card.power3.ToString();
            power1.gameObject.SetActive(true);
            power2.gameObject.SetActive(true);
            power3.gameObject.SetActive(true);
        }
        else
        {
            power1.gameObject.SetActive(false);
            power2.gameObject.SetActive(false);
            power3.gameObject.SetActive(false);
        }
        text.text = card.text;
        text0.text = "";
        text1.text = "";
        text2.text = "";
        if (card.textList.Length > 0) text0.text = card.textList[0];
        if (card.textList.Length > 1) text1.text = card.textList[1];
        if (card.textList.Length > 2) text2.text = card.textList[2];

        switch (card.color)
        {
            case "—Î":
                title.color = new Color(0, 1, 0);
                break;
            case "Ô":
                title.color = new Color(1, 0, 0);
                break;
            case "‰©":
                title.color = new Color(1, 1, 0);
                break;
            case "•":
                title.color = new Color(0.5f, 0, 1);
                break;
            case "”’":
                title.color = new Color(1, 1, 1);
                break;
        }
        gameObject.SetActive(true);
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(card.id, illust));
        StartCoroutine(ImageUtil.ImageUtil.ColorImage(card.color, color));
    }
    public void Close()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(false);
    }

    public void SkillDisplay(Battle_Card card,int i)
    {
        this.card = card.card;
        text.text = card.card.textList[i];
        if (card.own) window.color = new Color(0, 0, 0.1f, 0.7f);
        else window.color = new Color(0.1f, 0, 0, 0.7f);
        gameObject.SetActive(true);
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(card.card.id, illust));
    }
    public void StateButton(int i)
    {
        if (i == 0) state.Open(StateToNum(card.skill1));
        if (i == 1) state.Open(StateToNum(card.skill2));
        if (i == 2) state.Open(StateToNum(card.skill3));
    }
    int StateToNum(string state)
    {
        int num = -1;
        switch (state)
        {
            case "í“¬€”õ":
                num = 0;
                break;
            case "UŒ‚‹Ö~":
                num = 1;
                break;
            case "–hŒä‹Ö~":
                num = 2;
                break;
            case "‹­§UŒ‚":
                num = 3;
                break;
            case "‹­§–hŒä":
                num = 4;
                break;
            case "Á–Å–³Œø":
                num = 5;
                break;
            case "”j‰ó–³Œø":
                num = 6;
                break;
            case "‡D‹Ö~":
                num = 7;
                break;
            case "‹­§”­“®":
                num = 8;
                break;
            case "ŠÑ’Ê":
                num = 10;
                break;
            case "Šæ‹­":
                num = 11;
                break;
            case "‘ÌŠ²":
                num = 12;
                break;
            case "M”O":
                num = 13;
                break;
            case "‘¬U":
                num = 14;
                break;
            case "Ä‹N":
                num = 15;
                break;
            case "Z“§1":
                num = 16;
                break;
            case "Z“§2":
                num = 16;
                break;
            case "Z“§3":
                num = 16;
                break;
            case "Z“§4":
                num = 16;
                break;
            case "Z“§5":
                num = 16;
                break;
            case "‰B–§":
                num = 17;
                break;
            case "‹‘å2":
                num = 18;
                break;
            case "‹‘å3":
                num = 18;
                break;
            case "‹‘å4":
                num = 18;
                break;
            case "‹‘å5":
                num = 18;
                break;
            case "’¾–Ù":
                num = 19;
                break;
            case "‰ÁŒì":
                num = 20;
                break;
            case "¶æÑ":
                num = 21;
                break;
            case "‰Î":
                num = 22;
                break;
            case "Á–Å":
                num = 23;
                break;
            case "dŒ‚1":
                num = 24;
                break;
            case "dŒ‚2":
                num = 24;
                break;
            case "dŒ‚3":
                num = 24;
                break;
            case "‹­P":
                num = 25;
                break;
            case "’§”­":
                num = 26;
                break;
            case "–Ò“Å1":
                num = 27;
                break;
            case "–Ò“Å2":
                num = 27;
                break;
            case "–Ò“Å3":
                num = 27;
                break;
            case "ô”›":
                num = 28;
                break;
        }
        return num;
    }
}
