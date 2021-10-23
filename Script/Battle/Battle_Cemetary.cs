using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_Cemetary : MonoBehaviour
{
    public GameObject window;
    public void Button()
    {
        window.SetActive(!window.activeSelf);
    }
}
