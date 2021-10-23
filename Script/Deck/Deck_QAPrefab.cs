using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_QAPrefab : MonoBehaviour
{
    public Text title;
    public Text text;
    public string str;
    public void Button()
    {
        text.text = str;
    }
}
