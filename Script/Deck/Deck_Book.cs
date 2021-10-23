using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_Book : MonoBehaviour
{
    public Dropdown bookDrop;
    public InputField DeckName;
    public Deck_Note note;

    public GameObject leftContent;
    public GameObject rightContent;
    public List<Deck_Card> leftList = new List<Deck_Card>();
    public List<Deck_Card> rightList = new List<Deck_Card>();
    public Text leftKinds;
    public Text leftNum;
    public Text rightKinds;
    public Text rightNum;

    bool charaLeft;
    public GameObject charaList;
    public Image charaL;
    public Image charaR;

    void Start()
    {
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        for (int i = 0; i < 30; i++) {
            Dropdown.OptionData option = new Dropdown.OptionData();
            option.text = "デッキ選択:" + (i+1) + "   " + PlayerPrefs.GetString("DeckName" + i,"新規デッキ");
            options.Add(option);
        }
        bookDrop.options = options;
    }
    public void BookSelect(int i)
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        DeckName.text = PlayerPrefs.GetString("DeckName" + i, "新規デッキ");
        PlayerPrefs.SetInt("SelectDeck",i);
        List<Deck_Card> tmpList = new List<Deck_Card>();
        foreach (Deck_Card listCard in leftList)
        {
            tmpList.Add(listCard);
        }
        foreach (Deck_Card listCard in tmpList)
        {
            listCard.area = 0;
            listCard.num = 0;
            listCard.equip.text = "";
            leftList.Remove(listCard);
            note.cardList.Add(listCard);
        }
        tmpList = new List<Deck_Card>();
        foreach (Deck_Card listCard in rightList)
        {
            tmpList.Add(listCard);
        }
        foreach (Deck_Card listCard in tmpList)
        {
            listCard.area = 0;
            listCard.num = 0;
            listCard.equip.text = "";
            rightList.Remove(listCard);
            note.cardList.Add(listCard);
        }
        for(int j = 0; j < 30; j++)
        {
            string id = PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "L" + j,"");
            if (id == "") break;
            if (!note.cardMaster.CardList.Exists(c=>c.id == id)) continue;
            Deck_Card card = note.cardList.Find(c => c.card.id == id);
            if (card != null)
            {
                card.area = 1;
                card.num = 1;
                card.equip.text = "x" + card.num;
                note.cardList.Remove(card);
                leftList.Add(card);
            }
            else
            {
                card = leftList.Find(c => c.card.id == id);
                card.num += 1;
                card.equip.text = "x" + card.num;
            }
        }
        for (int j = 0; j < 30; j++)
        {
            string id = PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "R" + j, "");
            if (id == "") break;
            if (!note.cardMaster.CardList.Exists(c => c.id == id)) continue;
            Deck_Card card = note.cardList.Find(c => c.card.id == id);
            if (card != null)
            {
                card.area = 2;
                card.num = 1;
                card.equip.text = "x" + card.num;
                note.cardList.Remove(card);
                rightList.Add(card);
            }
            else
            {
                card = rightList.Find(c => c.card.id == id);
                card.num += 1;
                card.equip.text = "x" + card.num;
            }
        }
        
        
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "L", "G-1"), charaL));
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "R", "R-2"), charaR));
        note.LineUp();
        LineUp();
    }
    public void BookNameChange(string name)
    {
        PlayerPrefs.SetString("DeckName" + bookDrop.value, name);
        bookDrop.options[bookDrop.value].text = "デッキ選択:" + (bookDrop.value+1) + "   " + PlayerPrefs.GetString("DeckName" + bookDrop.value, "新規デッキ");
    }
    public void BookReset()
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        DeckName.text = "新規デッキ";
        PlayerPrefs.SetString("DeckName" + bookDrop.value, "新規デッキ");
        bookDrop.options[bookDrop.value].text = "デッキ選択:" + (bookDrop.value + 1) + "   " + PlayerPrefs.GetString("DeckName" + bookDrop.value, "新規デッキ");

        List<Deck_Card> tmpList = new List<Deck_Card>();
        foreach (Deck_Card listCard in leftList)
        {
            tmpList.Add(listCard);
        }
        foreach (Deck_Card listCard in tmpList)
        {
            listCard.area = 0;
            listCard.num = 0;
            listCard.equip.text = "";
            leftList.Remove(listCard);
            note.cardList.Add(listCard);
        }
        tmpList = new List<Deck_Card>();
        foreach (Deck_Card listCard in rightList)
        {
            tmpList.Add(listCard);
        }
        foreach (Deck_Card listCard in tmpList)
        {
            listCard.area = 0;
            listCard.num = 0;
            listCard.equip.text = "";
            rightList.Remove(listCard);
            note.cardList.Add(listCard);
        }
        for (int j = 0; j < 30; j++)
        {
            PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "L" + j, "");
            PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "R" + j, "");
        }
        note.LineUp();
        LineUp();
    }

    public void LineUp()
    {
        leftList.Sort(Deck_Note.Sort);
        rightList.Sort(Deck_Note.Sort);

        int i = 0;
        int num = 0;
        foreach (Deck_Card listCard in leftList)
        {
            listCard.transform.SetParent(leftContent.transform);
            listCard.transform.localScale = new Vector3(0.75f, 0.75f, 1);
            listCard.transform.localPosition = new Vector2(-300+150 * (i % 4), 300-150 * (i / 4));
            for(int j = 0; j < listCard.num; j++)
            {
                PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "L" + (num + j), listCard.card.id);
            }
            i++;
            num += listCard.num;
        }

        leftKinds.text = i + "/16種まで";
        leftNum.text = num + "/30枚必須";

        for (; num < 30; num++)
        {
            PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "L" + num, "");
        }

        i = 0;
        num = 0;
        foreach (Deck_Card listCard in rightList)
        {
            listCard.transform.SetParent(rightContent.transform);
            listCard.transform.localScale = new Vector3(0.75f, 0.75f, 1);
            listCard.transform.localPosition = new Vector2(-300+150 * (i % 4),300 -150 * (i / 4));
            for (int j = 0; j < listCard.num; j++)
            {
                PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "R" + (num + j), listCard.card.id);
            }
            i++;
            num += listCard.num;
        }

        rightKinds.text = i + "/16種まで";
        rightNum.text = num + "/30枚必須";

        for (; num < 30; num++)
        {
            PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "R" + num, "");
        }
    }
    public void OpenCharaList(bool left)
    {
        charaLeft = left;
        SEManager.Instance.Play(SEPath.CLICK);
        charaList.SetActive(true);
    }
    public void CloseCharaList()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        charaList.SetActive(false);
    }
    public void CharaDecide(string id)
    {
        if (charaLeft) PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "L", id);
        else PlayerPrefs.SetString("D" + (bookDrop.value + 1) + "R", id);
        if (charaLeft) StartCoroutine( ImageUtil.ImageUtil.IllustImage(id, charaL));
        else StartCoroutine(ImageUtil.ImageUtil.IllustImage(id, charaR));
        charaList.SetActive(false);
    }
    public void BGMPlay(string id)
    {
        switch (id)
        {
            case "G-1":
                BGMManager.Instance.Play(BGMPath.G1);
                break;
            case "G-2":
                BGMManager.Instance.Play(BGMPath.G2);
                break;
            case "R-1":
                BGMManager.Instance.Play(BGMPath.R1);
                break;
            case "R-2":
                BGMManager.Instance.Play(BGMPath.R2);
                break;
            case "Y-1":
                BGMManager.Instance.Play(BGMPath.Y1);
                break;
            case "Y-2":
                BGMManager.Instance.Play(BGMPath.Y2);
                break;
            case "P-1":
                BGMManager.Instance.Play(BGMPath.P1);
                break;
            case "P-2":
                BGMManager.Instance.Play(BGMPath.P2);
                break;
        }
    }
}
