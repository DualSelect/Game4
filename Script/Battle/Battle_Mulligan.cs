using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Battle_Mulligan : MonoBehaviour
{
    public Card card;
    public Image illust;
    public Text power;
    public Image color;
    public Text cost;
    public Image type;
    public Text equip;
    public Image level;
    public int area;
    public int order;
    public bool isLeft;
    public Battle_Player player;

    public Deck_Status status;
    public GameObject cheakMark;

    public void Init(Card card, int order, bool isLeft)
    {
        this.card = card;
        power.text = card.power1.ToString();
        cost.text = card.cost.ToString();
        this.order = order;
        this.isLeft = isLeft;
        StartCoroutine(ImageUtil.ImageUtil.ColorImage(card.color, color));
        StartCoroutine(ImageUtil.ImageUtil.TypeImage(card.type, type));
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(card.id, illust));
    }
    public void MulliganButton()
    {
        SEManager.Instance.Play(SEPath.CARDIN);
        if (isLeft)
        {
            if (player.mulliganLeft[order])
            {
                player.mulliganLeft[order] = false;
                cheakMark.SetActive(false);
            }
            else
            {
                player.mulliganLeft[order] = true;
                cheakMark.SetActive(true);
            }
        }
        else
        {
            if (player.mulliganRight[order])
            {
                player.mulliganRight[order] = false;
                cheakMark.SetActive(false);
            }
            else
            {
                player.mulliganRight[order] = true;
                cheakMark.SetActive(true);
            }
        }
    }

    public void Click()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        status.StatusDisplay(card);
    }
}
