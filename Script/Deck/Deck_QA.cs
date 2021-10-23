using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deck_QA : MonoBehaviour
{
    public Text text;
    public QAMaster qaMaster;
    public GameObject content;
    public GameObject prefab;

    void Start()
    {
        int i = 0;
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 110 * qaMaster.QAList.Count);
        foreach (QA qa in qaMaster.QAList)
        {
            GameObject gameObject = Instantiate(prefab, new Vector3(0, -110 * i, 0), Quaternion.identity);
            Deck_QAPrefab qaPrefab = gameObject.GetComponent<Deck_QAPrefab>();
            gameObject.transform.SetParent(content.transform, false);
            qaPrefab.text = text;
            qaPrefab.str = qa.text;
            qaPrefab.title.text = qa.title;
            i++;
        }
    }
    public void Open()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        gameObject.SetActive(false);
    }
}
