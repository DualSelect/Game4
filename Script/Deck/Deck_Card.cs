using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using KanKikuchi.AudioManager;

public class Deck_Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public Image illust;
    public Text power;
    public Image color;
    public Text cost;
    public Image type;
    public Text equip;
    public int num;
    public int area;

    public GameObject dragParent;
    public GameObject prevParent;
    public GameObject[] dropArea;
    public Deck_Status status;
    private Vector2 prevPos;

    private RectTransform rectTransform;
    private RectTransform rectTransformParent;
    public void Init(Card card)
    {
        rectTransform = GetComponent<RectTransform>();
        this.card = card;
        power.text = card.power1.ToString();
        cost.text = card.cost.ToString();
        if (card.type == "マジック")
        {
            power.text = "-";
        }
        if (card.type == "アイテム")
        {
            power.gameObject.SetActive(false);
        }
        StartCoroutine(ImageUtil.ImageUtil.ColorImage(card.color, color));
        StartCoroutine(ImageUtil.ImageUtil.TypeImage(card.type, type));
        StartCoroutine(ImageUtil.ImageUtil.IllustImage(card.id, illust));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        prevParent = transform.parent.gameObject;
        transform.SetParent(dragParent.transform);
        rectTransformParent = transform.parent.GetComponent<RectTransform>();
        prevPos = transform.position;
        foreach (GameObject obj in dropArea)
        {
            obj.SetActive(true);
            StartCoroutine(obj.GetComponent<Deck_Drop>().Transparent());
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        //transform.position = eventData.position +new Vector2(0,-10);
        Vector2 localPosition = GetLocalPosition(eventData.position);
        rectTransform.anchoredPosition = localPosition + new Vector2(80, -80);
    }
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformParent, screenPosition, Camera.main, out result);
        return result;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        StopAllCoroutines();
        foreach (var hit in raycastResults)
        {
            if (hit.gameObject.CompareTag("Ray"))
            {
                hit.gameObject.GetComponent<Deck_Drop>().OnDrop(this);
                StartCoroutine(Se(hit.gameObject.GetComponent<Deck_Drop>().num));
            }
        }



        foreach (GameObject obj in dropArea)
        {
            obj.SetActive(false);
        }
        if (transform.parent == dragParent.transform)
        {
            transform.SetParent(prevParent.transform);
            transform.position = prevPos;
        }
    }
    public void Click()
    {

        status.StatusDisplay(card);
    }
    IEnumerator Se(int num)
    {
        for (int i = 0; i < num; i++)
        {
            SEManager.Instance.Play(SEPath.CARDIN);
            yield return new WaitForSecondsRealtime(0.3f);
        }
        if (num == 0) SEManager.Instance.Play(SEPath.CARDOUT);
        if (num == -1) SEManager.Instance.Play(SEPath.CANCEL);
    }
}
