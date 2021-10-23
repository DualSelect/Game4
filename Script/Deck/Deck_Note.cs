using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Deck_Note : MonoBehaviour
{
    public CardMaster cardMaster;
    public GameObject cardPrefab;
    public GameObject noteContent;
    public GameObject dragParent;
    public GameObject[] dropArea;
    public Deck_Book deck;
    public Deck_Status status;

    public Image[] colorButton;
    public bool[] colorSelect = new bool[6] { true, true, true, true, true, true };
    int per = 2;
    int min = 0;
    int max = 7;
    string type = "タイプ";
    string tribe = "種族";
    public Dropdown typeDrop;
    public Dropdown tribeDrop;

    public List<Deck_Card> cardList = new List<Deck_Card>();
    void Start()
    {
        if(PlayerPrefs.GetFloat("news", 0) == 0)
        {
            PlayerPrefs.SetString("DeckName0", "初期デッキ");
            int i = 0;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G2"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G2"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G2"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G4"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G4"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G4"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G3"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G3"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G3"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G11"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G11"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G11"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G15"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G15"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G15"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G17"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G17"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G17"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G20"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G20"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G20"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G25"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G25"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G25"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G28"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G28"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G28"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G31"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G31"); i++;
            PlayerPrefs.SetString("D" + 1 + "L" + i, "G35"); i++;
            i = 0;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R2"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R2"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R2"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R3"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R3"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R3"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R11"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R11"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R11"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R12"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R12"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R12"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R18"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R18"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R18"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R21"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R21"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R21"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R24"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R24"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R24"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R25"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R25"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R25"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R27"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R29"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R29"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R29"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R28"); i++;
            PlayerPrefs.SetString("D" + 1 + "R" + i, "R28"); i++;
        }
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        Dropdown.OptionData first = new Dropdown.OptionData();
        first.text = "種族";
        options.Add(first);
        foreach (Card card in cardMaster.CardList)
        {
            if (card.title != "")
            {
                Deck_Card cardObj = (Instantiate(cardPrefab, noteContent.transform) as GameObject).GetComponent<Deck_Card>();
                cardObj.dragParent = dragParent;
                cardObj.dropArea = dropArea;
                cardObj.status = status;
                if (card.ok == 0) cardObj.GetComponent<Image>().color = new Color(0, 0, 0);
                cardObj.Init(card);
                cardList.Add(cardObj);
                if(!options.Exists(o => o.text == card.tribe1)&& card.tribe1!="")
                {
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = card.tribe1;
                    options.Add(option);
                }
                if (!options.Exists(o => o.text == card.tribe2) && card.tribe2 != "")
                {
                    Dropdown.OptionData option = new Dropdown.OptionData();
                    option.text = card.tribe2;
                    options.Add(option);
                }
            }
        }
        tribeDrop.options = options;
        LineUp();
        deck.bookDrop.value = PlayerPrefs.GetInt("SelectDeck", 0);
        if(deck.bookDrop.value==0) deck.BookSelect(0);
    }
    public void LineUp()
    {
        cardList.Sort(Sort);

        int size = 200 * 2 / per;
        foreach (Deck_Card listCard in cardList)
        {
            listCard.transform.SetParent(noteContent.transform);
            listCard.transform.localPosition = new Vector2(-200, 0);
            listCard.transform.localScale = new Vector3(2f/per,2f/per,1);
        }
        int i = 0;
        foreach (Deck_Card listCard in cardList)
        {
            if (colorSelect[listCard.card.colorSort] && min <= listCard.card.cost && listCard.card.cost <= max)
            {
                if (type != "タイプ" && type != listCard.card.type) continue;
                if (tribe != "種族") if(tribe != listCard.card.tribe1 && tribe != listCard.card.tribe2) continue;

                listCard.transform.localPosition = new Vector2(size * (i % per) + 10,-size * (i / per));
                i++;
            }
        }
        noteContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, size * ((i / per)+1));
    }
    public void MinChange(int i)
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        min = i;
        LineUp();
    }
    public void MaxChange(int i)
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        max = 7-i;
        LineUp();
    }
    public void TypeChange()
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        type = typeDrop.captionText.text;
        LineUp();
    }
    public void TribeChange()
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        tribe = tribeDrop.captionText.text;
        LineUp();
    }
    public void ColorSwitch(int i)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        if (colorSelect[i]) colorButton[i].color = new Color(1, 1, 1, 0.5f);
        else colorButton[i].color = new Color(1, 1, 1, 1);
        colorSelect[i] = !colorSelect[i];
        LineUp();
    }
    public void PerChange(int i)
    {
        SEManager.Instance.Play(SEPath.BOOK_CHANGE);
        per = i + 2;
        LineUp();
    }
    public static int Sort(Deck_Card x , Deck_Card y)
    {
        // 第一のキー(Ageフィールド)で比較
        if (x.card.colorSort > y.card.colorSort)
        {
            return 1;
        }
        else if (x.card.colorSort < y.card.colorSort)
        {
            return -1;
        }
        else
        {
            // 第一のキーが同じだった場合は、第二のキー(IDフィールド)で比較
            if (x.card.typeSort > y.card.typeSort)
            {
                return 1;
            }
            else if (x.card.typeSort < y.card.typeSort)
            {
                return -1;
            }
            else
            {
                if (x.card.cost > y.card.cost)
                {
                    return 1;
                }
                else if (x.card.cost < y.card.cost)
                {
                    return -1;
                }
                else
                {
                    if (x.card.no > y.card.no)
                    {
                        return 1;
                    }
                    else if (x.card.no < y.card.no)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
    public void Click()
    {
        SEManager.Instance.Play(SEPath.CLICK);
    }
}
