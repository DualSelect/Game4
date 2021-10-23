using BattleJson;
using KanKikuchi.AudioManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Battle_Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Card card;
    public Image illust;
    public Text power;
    public Image color;
    public Text cost;
    public Image type;
    public Image[] level;
    public GameObject[] stateMark;
    public string area;
    public int order;
    public bool isLeft;
    public bool own;
    public bool act;
    public int lv;


    public int[] state;
    public Text notice;

    public GameObject dragParent;
    public GameObject prevParent;
    public Deck_Status status;
    private Vector2 prevPos;
    public Battle_Player player;
    public GameObject[] dropArea;
    public Battle_Drop merge;
    public Battle_Drop generate;
    private RectTransform rectTransform;
    private RectTransform rectTransformParent;
    public GameObject stateContent;
    public GameObject select;
    public GameObject panish;
    public Image[] change;
    bool dragStart = false;

    public SimpleAnimation summon;
    public SimpleAnimation levelUp;
    public SimpleAnimation levelDown;
    public SimpleAnimation actUp;
    public SimpleAnimation actDown;
    public SimpleAnimation death;
    public SimpleAnimation badState;
    public SimpleAnimation powerDown;
    public SimpleAnimation bounce;
    public SimpleAnimation powerUp;
    public SimpleAnimation vanish;
    public SimpleAnimation damage;
    public SimpleAnimation cure;
    public SimpleAnimation goodState;
    public SimpleAnimation magic;
    public SimpleAnimation set;
    public SimpleAnimation die;
    public SimpleAnimation skill;


    public IEnumerator Init(Card card,int order,bool isLeft,bool own,Battle_Player player)
    {
        rectTransform = GetComponent<RectTransform>();
        this.card = card;
        power.text = card.power1.ToString();
        cost.text = card.cost.ToString();
        this.order = order;
        this.isLeft = isLeft;
        this.own = own;
        area = "deck";
        act = true;
        this.player = player;
        merge.player = player;
        generate.player = player;
        yield return ImageUtil.ImageUtil.ColorImage(card.color, color);
        yield return ImageUtil.ImageUtil.TypeImage(card.type, type);
        yield return ImageUtil.ImageUtil.IllustImage(card.id, illust);
        player.cardCreate++;
        if (card.type == "マジック")
        {
            power.text = "-";
            level[0].gameObject.SetActive(false);
        }
        if (card.type == "アイテム")
        {
            power.gameObject.SetActive(false);
            level[0].gameObject.SetActive(false);
        }
    }
    public void ReSet()
    {
        act = true;
        illust.color = new Color(1, 1, 1);
        power.text = card.power1.ToString();
        cost.text = card.cost.ToString();
        lv = 1;
        for(int i = 0; i < state.Length; i++)
        {
            state[i] = 0;
        }
        foreach (GameObject obj in stateMark)
        {
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(-200, 0, 0);
        }
        this.level[0].gameObject.SetActive(true);
        this.level[1].gameObject.SetActive(false);
        this.level[2].gameObject.SetActive(false);
        stateContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
    }
    public void Click()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        status.StatusDisplay(card);
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        SEManager.Instance.Play(SEPath.CLICK);
        if (player.mainPhase && own && (area=="field"||(isLeft == player.isLeft && area=="hand")) )
        {
            prevParent = transform.parent.gameObject;
            transform.SetParent(dragParent.transform);
            rectTransformParent = transform.parent.GetComponent<RectTransform>();
            prevPos = transform.position;
            player.DragStart(this);
            foreach (GameObject obj in dropArea)
            {
                StartCoroutine(obj.GetComponent<Battle_Drop>().Transparent());
            }
            dragStart = true;
        }
        else
        {
            dragStart = false;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (dragStart)
        {
            Vector2 localPosition = GetLocalPosition(eventData.position);
            rectTransform.anchoredPosition = localPosition + new Vector2(960, -550);
        }
    }
    private Vector2 GetLocalPosition(Vector2 screenPosition)
    {
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransformParent,screenPosition,Camera.main,out result);
        return result;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragStart)
        {
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raycastResults);

            foreach (var hit in raycastResults)
            {
                if (hit.gameObject.CompareTag("Ray"))
                {
                    hit.gameObject.GetComponent<Battle_Drop>().OnDrop(this);
                }
            }
            StopAllCoroutines();
            player.DragEnd();
            if (transform.parent == dragParent.transform)
            {
                transform.SetParent(prevParent.transform);
                transform.position = prevPos;
            }
        }
    }
    public IEnumerator ActChange(int i)
    {
        if (i==1)
        {
            act = true;
            illust.color = new Color(1, 1, 1);
            actUp.gameObject.SetActive(true);
            actUp.Play();
            SEManager.Instance.Play(SEPath.ACTUP);
            yield return new WaitForSecondsRealtime(1.0f);
            actUp.gameObject.SetActive(false);
        }
        else
        {
            act = false;
            illust.color = new Color(0.3f, 0.3f, 0.3f);
            actDown.gameObject.SetActive(true);
            actDown.Play();
            SEManager.Instance.Play(SEPath.ACTDOWN);
            yield return new WaitForSecondsRealtime(1.0f);
            actDown.gameObject.SetActive(false);
        }
    }
    public IEnumerator StateDisplay(int stateNo,int value)
    {
        state[stateNo] = value;
        foreach (GameObject obj in stateMark)
        {
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(-200, 0, 0);
        }
        int sum = 0;
        for (int i = 0; i < state.Length; i++)
        {
            if (state[i] > 0)
            {
                stateMark[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(50 * sum, 0, 0);
                sum++;
                if (i == 16 || i == 18 || i == 24 || i == 27)
                {
                    stateMark[i].transform.GetChild(0).GetComponent<Text>().text = state[i].ToString();
                }
                if (i == 23) panish.SetActive(true);
            }
        }
        stateContent.GetComponent<RectTransform>().sizeDelta = new Vector2(50 * sum, 50);
        if (stateNo == 0 || stateNo == 23)
        {

        }
        else if( stateNo==1 || stateNo == 2 || stateNo == 3 || stateNo == 4 || stateNo == 19 || stateNo == 22 || stateNo == 27 || stateNo == 28)
        {
            if (value == 0)
            {
                goodState.gameObject.SetActive(true);
                goodState.Play();
                SEManager.Instance.Play(SEPath.STATE_UP);
                yield return new WaitForSecondsRealtime(1.0f);
                goodState.gameObject.SetActive(false);
            }
            else
            {
                badState.gameObject.SetActive(true);
                badState.Play();
                SEManager.Instance.Play(SEPath.STATE_DOWN);
                yield return new WaitForSecondsRealtime(1.0f);
                badState.gameObject.SetActive(false);
            }
        }
        else
        {
            if (value == 0)
            {
                badState.gameObject.SetActive(true);
                badState.Play();
                SEManager.Instance.Play(SEPath.STATE_DOWN);
                yield return new WaitForSecondsRealtime(1.0f);
                badState.gameObject.SetActive(false);
            }
            else
            {
                goodState.gameObject.SetActive(true);
                goodState.Play();
                SEManager.Instance.Play(SEPath.STATE_UP);
                yield return new WaitForSecondsRealtime(1.0f);
                goodState.gameObject.SetActive(false);
            }
        }
    }
    public void StateClick(int stateNo)
    {
        Debug.Log(stateNo);
        player.stateNote.Open(stateNo);
    }
    public void EnemyCard(string area)
    {
        if (area == "hand")
        {
            illust.gameObject.SetActive(false);
            power.gameObject.SetActive(false);
            color.gameObject.SetActive(false);
            cost.gameObject.SetActive(false);
            level[0].gameObject.SetActive(false);
            level[1].gameObject.SetActive(false);
            level[2].gameObject.SetActive(false);
            type.gameObject.SetActive(false);
        }
        else if (area == "equip")
        {
            illust.gameObject.SetActive(false);
            power.gameObject.SetActive(false);
            color.gameObject.SetActive(true);
            cost.gameObject.SetActive(false);
            level[0].gameObject.SetActive(false);
            level[1].gameObject.SetActive(false);
            level[2].gameObject.SetActive(false);
            type.gameObject.SetActive(false);
        }
        else
        {
            illust.gameObject.SetActive(true);
            power.gameObject.SetActive(true);
            color.gameObject.SetActive(true);
            cost.gameObject.SetActive(true);
            type.gameObject.SetActive(true);
            level[0].gameObject.SetActive(false);
            level[1].gameObject.SetActive(false);
            level[2].gameObject.SetActive(false);
            if (card.type=="ユニット"|| card.type == "進化ユニット")
            {
                level[lv-1].gameObject.SetActive(true);
            }
        }
    }
    public void Select()
    {
        player.SelectCard(order);
    }
    public IEnumerator Power(Power power)
    {
        int value = power.change;

        if (power.skill)
        {
            if (power.type == "level")
            {

            }
            else if (power.type == "damage")
            {
                if (value < 0)
                {
                    damage.gameObject.SetActive(true);
                    damage.Play();
                    SEManager.Instance.Play(SEPath.DAMAGE);
                }
                else
                {

                    cure.gameObject.SetActive(true);
                    cure.Play();
                    SEManager.Instance.Play(SEPath.CURE);
                }
            }
            else
            {
                if (value < 0)
                {
                    powerDown.gameObject.SetActive(true);
                    powerDown.Play();
                    SEManager.Instance.Play(SEPath.POWER_DOWN);
                }
                else
                {
                    powerUp.gameObject.SetActive(true);
                    powerUp.Play();
                    SEManager.Instance.Play(SEPath.POWER_UP);
                }
            }
        }



        Sprite[] sprites;
        if (power.type == "damage")
        {
            if (value < 0) sprites = player.blood;
            else sprites = player.white;
        }
        else
        {
            if (value < 0) sprites = player.cool;
            else sprites = player.heat;
        }
        if (value < 0) value *= -1;



        for (int i = 0; i < 5; i++)
        {
            change[i].sprite = sprites[value % 10];
            if (value % 10 > 0)
            {
                for (int j = 0; j <= i; j++) change[j].gameObject.SetActive(true);
            }
            value /= 10;
        }
        this.power.text = power.now.ToString();
        yield return new WaitForSecondsRealtime(1f);
        foreach(Image image in change)
        {
            image.gameObject.SetActive(false);
        }

        damage.gameObject.SetActive(false);
        cure.gameObject.SetActive(false);
        powerDown.gameObject.SetActive(false);
        powerUp.gameObject.SetActive(false);
    }
    public IEnumerator Level(Level level)
    {
        this.level[0].gameObject.SetActive(false);
        this.level[1].gameObject.SetActive(false);
        this.level[2].gameObject.SetActive(false);
        lv = level.now;
        this.level[lv - 1].gameObject.SetActive(true);
        if (level.change > 0)
        {
            levelUp.gameObject.SetActive(true);
            //levelUp.Play();
            SEManager.Instance.Play(SEPath.LEVEL_UP);
            yield return new WaitForSecondsRealtime(1.0f);
            levelUp.gameObject.SetActive(false);
        }
        else
        {
            levelDown.gameObject.SetActive(true);
            levelDown.Play();
            SEManager.Instance.Play(SEPath.LEVEL_DOWN);
            yield return new WaitForSecondsRealtime(1.0f);
            levelDown.gameObject.SetActive(false);
        }
    }
    public IEnumerator Summon(bool skill)
    {
        if (!skill)
        {
            summon.gameObject.SetActive(true);
            summon.Play();
            SEManager.Instance.Play(SEPath.SUMMON);
            yield return new WaitForSecondsRealtime(1.0f);
            summon.gameObject.SetActive(false);
        }
    }
    public IEnumerator Death(bool power)
    {
        if (!power)
        {
            death.gameObject.SetActive(true);
            death.Play();
            SEManager.Instance.Play(SEPath.DEATH);
            yield return new WaitForSecondsRealtime(1.0f);
            death.gameObject.SetActive(false);
        }
        else
        {
            die.gameObject.SetActive(true);
            die.Play();
            SEManager.Instance.Play(SEPath.DIE);
            yield return new WaitForSecondsRealtime(1.0f);
            die.gameObject.SetActive(false);
        }
    }
    public IEnumerator Vanish()
    {
        vanish.gameObject.SetActive(true);
        vanish.Play();
        SEManager.Instance.Play(SEPath.VANISH);
        yield return new WaitForSecondsRealtime(1.0f);
        vanish.gameObject.SetActive(false);
    }
    public IEnumerator Bounce()
    {
        bounce.gameObject.SetActive(true);
        bounce.Play();
        SEManager.Instance.Play(SEPath.BOUNCE);
        yield return new WaitForSecondsRealtime(1.0f);
        bounce.gameObject.SetActive(false);
    }
    public IEnumerator Set()
    {
        set.gameObject.SetActive(true);
        //set.Play();
        SEManager.Instance.Play(SEPath.CARDIN);
        yield return new WaitForSecondsRealtime(1.0f);
        set.gameObject.SetActive(false);
    }
    public IEnumerator Magic()
    {
        illust.gameObject.SetActive(true);
        magic.gameObject.SetActive(true);
        magic.Play();
        SEManager.Instance.Play(SEPath.MAGIC);
        yield return new WaitForSecondsRealtime(1.0f);
        magic.gameObject.SetActive(false);
    }
    public IEnumerator EquipDeath()
    {
        damage.gameObject.SetActive(true);
        damage.Play();
        SEManager.Instance.Play(SEPath.DAMAGE);
        yield return new WaitForSecondsRealtime(1.0f);
        damage.gameObject.SetActive(false);
    }
    public IEnumerator HandDeath()
    {
        death.gameObject.SetActive(true);
        death.Play();
        SEManager.Instance.Play(SEPath.DEATH);
        yield return new WaitForSecondsRealtime(1.0f);
        death.gameObject.SetActive(false);
    }
    public IEnumerator Skill()
    {
        skill.gameObject.SetActive(true);
        //skill.Play();
        SEManager.Instance.Play(SEPath.SKILL);
        yield return new WaitForSecondsRealtime(1.0f);
        skill.gameObject.SetActive(false);
    }
}
