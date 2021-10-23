using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_Drop : MonoBehaviour
{
    public int area;
    public int num;
    public Deck_Note note;
    public Deck_Book book;


    public void OnDrop(Deck_Card card)
    {
        if (num == -1) return;
        if (card.area == 0)
        {
            note.cardList.Remove(card);
        }
        if (card.area == 1)
        {
            book.leftList.Remove(card);
        }
        if (card.area == 2)
        {
            book.rightList.Remove(card);
        }
        if (area == 0)
        {
            note.cardList.Add(card);
            card.area = 0;
        }
        if (area == 1)
        {
            book.leftList.Add(card);
            card.area = 1;
        }
        if (area == 2)
        {
            book.rightList.Add(card);
            card.area = 2;
        }
        card.num = num;
        card.equip.text = "";
        if (num > 0)
        {
            card.equip.text = "x"+num;
        }

        note.LineUp();
        book.LineUp();

    }
    public IEnumerator Transparent()
    {
        while (true)
        {
            for(int i = 25; i > 13; i--)
            {
                gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0,i/25f);
                yield return new WaitForSeconds(0.1f);
            }
            for (int i = 13; i < 25; i++)
            {
                gameObject.GetComponent<Image>().color = new Color(0.6f, 0.6f, 0, i / 25f);
                yield return new WaitForSeconds(0.1f);

            }
        }
    }
}
