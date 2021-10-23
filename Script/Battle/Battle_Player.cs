using BattleJson;
using KanKikuchi.AudioManager;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_WEBGL
#else
using Firebase.Firestore;
using Firebase.Extensions;
#endif

public class Battle_Player : MonoBehaviourPunCallbacks
{
    public GameObject match;
    public GameObject matching;
    public GameObject battle;
    public GameObject menu;
    public Text menuText;
    public Text timeText;
    public Battle_Pun pun;
    public CardMaster cardMaster;
    public GameObject cardPrefab;
    List<Battle_Card> allCard = new List<Battle_Card>();
    List<Battle_Card> playerLeftDeck = new List<Battle_Card>();
    List<Battle_Card> playerLeftHand = new List<Battle_Card>();
    List<Battle_Card> playerLeftCemetary = new List<Battle_Card>();
    List<Battle_Card> playerRightDeck = new List<Battle_Card>();
    List<Battle_Card> playerRightHand = new List<Battle_Card>();
    List<Battle_Card> playerRightCemetary = new List<Battle_Card>();
    List<Battle_Card> playerField = new List<Battle_Card>();
    List<Battle_Card> playerEquip = new List<Battle_Card>();
    List<Battle_Card> enemyLeftDeck = new List<Battle_Card>();
    List<Battle_Card> enemyLeftHand = new List<Battle_Card>();
    List<Battle_Card> enemyLeftCemetary = new List<Battle_Card>();
    List<Battle_Card> enemyRightDeck = new List<Battle_Card>();
    List<Battle_Card> enemyRightHand = new List<Battle_Card>();
    List<Battle_Card> enemyRightCemetary = new List<Battle_Card>();
    List<Battle_Card> enemyField = new List<Battle_Card>();
    List<Battle_Card> enemyEquip = new List<Battle_Card>();
    public GameObject playerLeftDeckObj;
    public Text playerLeftDeckNum;
    public GameObject playerLeftHandObj;
    public GameObject playerLeftCemetaryObj;
    public Text playerLeftCemetaryNum;
    public GameObject playerRightDeckObj;
    public Text playerRightDeckNum;
    public GameObject playerRightHandObj;
    public GameObject playerRightCemetaryObj;
    public Text playerRightCemetaryNum;
    public GameObject playerFieldObj;
    public GameObject playerEquipObj;
    public GameObject[] playerLife;
    public GameObject[] playerMana;
    public GameObject enemyLeftDeckObj;
    public Text enemyLeftDeckNum;
    public GameObject enemyLeftHandObj;
    public GameObject enemyLeftCemetaryObj;
    public Text enemyLeftCemetaryNum;
    public GameObject enemyRightDeckObj;
    public Text enemyRightDeckNum;
    public GameObject enemyRightHandObj;
    public GameObject enemyRightCemetaryObj;
    public Text enemyRightCemetaryNum;
    public GameObject enemyFieldObj;
    public GameObject enemyEquipObj;
    public GameObject[] enemyLife;
    public GameObject[] enemyMana;
    public GameObject cardCreateObj;
    public StateNote stateNote;

    public List<Battle_Mulligan> mulliganLeftCard;
    public List<Battle_Mulligan> mulliganRightCard;
    public bool[] mulliganLeft;
    public bool[] mulliganRight;
    public Deck_Status status;
    public Deck_Status skill;
    public GameObject mulligan;
    List<Process> processList = new List<Process>();
    public Text turn;
    public int mana;
    public GameObject[] dropArea;
    public GameObject dragParent;
    public bool mainPhase = false;
    public bool isLeft = true;
    List<Battle_Card> dropList;
    List<Battle_Card> selectList;

    public GameObject turnEndButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject cancelButton;
    public Text message;
    public Image playerL;
    public Image playerR;
    public Image enemyL;
    public Image enemyR;
    public int cardCreate;
    public Effect_Attack attackEffect;

    public Sprite[] blood;
    public Sprite[] heat;
    public Sprite[] cool;
    public Sprite[] white;
    public SimpleAnimation[] playerBreak;
    public SimpleAnimation[] playerHeal;
    public SimpleAnimation[] enemyBreak;
    public SimpleAnimation[] enemyHeal;
    public SimpleAnimation[] playerManaUp;
    public SimpleAnimation[] playerManaDown;
    public SimpleAnimation[] enemyManaUp;
    public SimpleAnimation[] enemyManaDown;
    public SimpleAnimation[] end;
    bool secondData = false;
    public Image first;
    public Image second;
    public Battle_Vote playerVote;
    public Battle_Vote enemyVote;
    public SimpleAnimation firstSecond;
    public Image phase;
    public Image charaL;
    public Image charaR;
    public Sprite[] phaseSprite;
    public GameObject[] phasePosition;
    int enemyRp;
    bool gameEnd=false;
    public Effect_Result result;
    int time = 60;
    public bool timeClock = false;
    public Scrollbar scrollbar;

    void Start()
    {
        match.SetActive(true);
        battle.SetActive(false);
    }
    public void Initial()
    {
        StartCoroutine(PlayerIE());
    }
    IEnumerator PlayerIE()
    {
        Battle_Card card;
        Battle_Card card0;
        Battle_Card card1;
        int index;
        int index0;
        int index1;
        while (!gameEnd)
        {
            if (processList.Count > 0)
            {
                switch (processList[0].process)
                {
                    case "info":
                        InfoProcess();
                        yield return new WaitForSecondsRealtime(1.0f);
                        break;
                    case "match":
                        yield return MatchProcess(Info.ToInfo(processList[0].content));
                        break;
                    case "deck":
                        DeckProcess(Info.ToInfo(processList[0].content));
                        break;
                    case "mulligan":
                        MulliganProcess(Mulligan.ToMulligan(processList[0].content));
                        break;
                    case "turn":
                        TurnProcess(Turn.ToTurn(processList[0].content));
                        yield return new WaitForSecondsRealtime(1.0f);
                        break;
                    case "side":
                        yield return SideProcess(Side.ToSide(processList[0].content));
                        break;
                    case "mana":
                        StartCoroutine(ManaProcess(Mana.ToMana(processList[0].content)));
                        break;
                    case "life":
                        StartCoroutine(LifeProcess(Life.ToLife(processList[0].content)));
                        break;
                    case "main":
                        MainProcess(Main.ToMain(processList[0].content));
                        break;
                    case "move":
                        MoveProcess(Move.ToMove(processList[0].content));
                        break;
                    case "state":
                        StateProcess(State.ToState(processList[0].content));
                        break;
                    case "select":
                        SelectProcess(Select.ToSelect(processList[0].content));
                        break;
                    case "power":
                        PowerProcess(Power.ToPower(processList[0].content));
                        break;
                    case "level":
                        LevelProcess(Level.ToLevel(processList[0].content));
                        break;
                    case "death":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.Death(CardEffect.ToCardEffect(processList[0].content).skill));
                        break;
                    case "bounce":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.Bounce());
                        break;
                    case "vanish":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.Vanish());
                        break;
                    case "equipdeath":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.EquipDeath());
                        break;
                    case "handdeath":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.HandDeath());
                        break;
                    case "skill":
                        yield return SkillProcess(Skill.ToSkill(processList[0].content));
                        break;
                    case "attack":
                        card = allCard.Find(c => c.order == Battle.ToBattle(processList[0].content).attack);
                        if (card.own) index=playerField.FindIndex(c => c == card);
                        else index = enemyField.FindIndex(c => c == card);
                        attackEffect.AttackStart(card,index);
                        break;
                    case "block":
                        card = allCard.Find(c => c.order == Battle.ToBattle(processList[0].content).defence);
                        if (card.own) index = playerField.FindIndex(c => c == card);
                        else index = enemyField.FindIndex(c => c == card);
                        attackEffect.Block(card, index);
                        break;
                    case "break":
                        card = allCard.Find(c => c.order == Battle.ToBattle(processList[0].content).attack);
                        if (card.own) index = playerField.FindIndex(c => c == card);
                        else index = enemyField.FindIndex(c => c == card);
                        yield return attackEffect.Break(card, index);
                        break;
                    case "battle":
                        card0 = allCard.Find(c => c.order == Battle.ToBattle(processList[0].content).attack);
                        if (card0.own) index0 = playerField.FindIndex(c => c == card0);
                        else index0 = enemyField.FindIndex(c => c == card0);
                        card1 = allCard.Find(c => c.order == Battle.ToBattle(processList[0].content).defence);
                        if (card1.own) index1 = playerField.FindIndex(c => c == card1);
                        else index1 = enemyField.FindIndex(c => c == card1);
                        yield return attackEffect.Battle(card0,index0,card1,index1);
                        break;
                    case "summon":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        StartCoroutine(card0.Summon(CardEffect.ToCardEffect(processList[0].content).skill));
                        break;
                    case "magic":
                        card0 = allCard.Find(c => c.order == CardEffect.ToCardEffect(processList[0].content).order);
                        yield return card0.Magic();
                        break;
                    case "wait":
                        yield return new WaitForSecondsRealtime(1f);
                        break;
                    case "finish":
                        if (Finish.ToFinish(processList[0].content).winner == PhotonNetwork.IsMasterClient) yield return WinProcess();
                        else yield return LoseProcess();
                        break;
                }
                processList.RemoveAt(0);
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
    public void InfoProcess()
    {
        matching.SetActive(true);
        SEManager.Instance.Play(SEPath.MATCHING);
        string[] leftDeck = new string[30];
        string[] rightDeck = new string[30];
        for (int i = 0; i < 30; i++)
        {
            leftDeck[i] = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L" + i);
            rightDeck[i] = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R" + i);
        }
        Info info = new Info();
        info.player = PhotonNetwork.IsMasterClient;
        info.leftDeck = leftDeck;
        info.rightDeck = rightDeck;
        info.leftChara = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L", "G-1");
        info.rightChara = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R", "R-2");
        info.name = PlayerPrefs.GetString("Player", "新規プレイヤー");
        info.rp = PlayerPrefs.GetInt("RP", 1000);
        Answer("info", info.ToString());
    }
    public IEnumerator MatchProcess(Info info)
    {
        if (info.player == PhotonNetwork.IsMasterClient)
        {
            yield return playerVote.Init(info);
            if (info.first)
            {
                first.transform.localPosition = new Vector3(-500, 440, 0);
                firstSecond.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else second.transform.localPosition = new Vector3(-500, 440, 0);
        }
        else
        {
            yield return enemyVote.Init(info);
            if (info.first) first.transform.localPosition = new Vector3(500, 440, 0);
            else second.transform.localPosition = new Vector3(500, 440, 0);
            enemyRp = info.rp;
        }
        if (!secondData) secondData = true;
        else
        {
            match.SetActive(false);
            playerVote.Display();
            enemyVote.Display();
            firstSecond.gameObject.SetActive(true);
            SEManager.Instance.Play(SEPath.FIRST_SECOND);
            yield return new WaitForSeconds(1.5f);
            first.gameObject.SetActive(true);
            second.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            matching.SetActive(false);
            battle.SetActive(true);
        }
        PlayerPrefs.SetString("RoomNo", PhotonNetwork.CurrentRoom.Name);
        PlayerPrefs.SetInt("Battle", 1);
        if (PlayerPrefs.GetInt("Rank") == 1)
        {
            PlayerPrefs.SetInt("RpDiff", enemyRp - PlayerPrefs.GetInt("RP", 1000));
        }
    }
    public void DeckProcess(Info info)
    {
        if (PhotonNetwork.IsMasterClient == info.player)
        {
            DeckCreate(this.playerLeftDeck, info.leftDeck, true, info.player);
            DeckCreate(this.playerRightDeck, info.rightDeck, false, info.player);
        }
        else
        {
            DeckCreate(this.enemyLeftDeck, info.leftDeck, true, info.player);
            DeckCreate(this.enemyRightDeck, info.rightDeck, false, info.player);
        }
    }
    public void DeckCreate(List<Battle_Card> list, string[] deck, bool isLeft, bool player)
    {
        int enemy;
        if (player) enemy = 0;
        else enemy = 60;
        for (int i = 0; i < 30; i++)
        {
            Card card = cardMaster.CardList.Find(c => c.id == deck[i]);
            Battle_Card cardObj = (Instantiate(cardPrefab, cardCreateObj.transform) as GameObject).GetComponent<Battle_Card>();
            if(isLeft)StartCoroutine(cardObj.Init(card, i + enemy, isLeft, PhotonNetwork.IsMasterClient == player, this));
            else StartCoroutine(cardObj.Init(card, i+30 + enemy, isLeft, PhotonNetwork.IsMasterClient == player, this));
            cardObj.status = status;
            cardObj.dropArea = dropArea;
            cardObj.dragParent = dragParent;
            list.Add(cardObj);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
            allCard.Add(cardObj);
        }
    }
    public void MulliganProcess(Mulligan mulligan)
    {
        switch (PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L", "G-1"))
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

        if (PhotonNetwork.IsMasterClient == mulligan.player)
        {
            StartCoroutine(TimeClock());
            timeClock = true;

            this.mulligan.SetActive(true);
            for (int i = 0; i < 3; i++)
            {
                Card left = cardMaster.CardList.Find(c => c.id == mulligan.leftDeck[i]);
                mulliganLeftCard[i].Init(left, i, true);
                Card right = cardMaster.CardList.Find(c => c.id == mulligan.rightDeck[i]);
                mulliganRightCard[i].Init(right, i, false);
            }
        }
    }
    public void MulliganDecide()
    {
        timeClock = false;
        SEManager.Instance.Play(SEPath.CLICK);
        this.mulligan.SetActive(false);
        Mulligan mulligan = new Mulligan();
        mulligan.player = PhotonNetwork.IsMasterClient;
        mulligan.left = mulliganLeft;
        mulligan.right = mulliganRight;
        Answer("mulligan",mulligan.ToString());
    }
    void TurnProcess(Turn turn)
    {
        StartCoroutine(PhaseDisplay(0));
        string direction;
        string first;
        enemyR.color = new Color(1, 0, 0, 0.2f);
        enemyL.color = new Color(0, 0, 1, 0.2f);
        playerR.color = new Color(1, 0, 0, 0.2f);
        playerL.color = new Color(0, 0, 1, 0.2f);

        if (turn.first)
        {
            first = "先攻";
            if (PhotonNetwork.IsMasterClient == turn.player) direction = "△";
            else direction = "▽";
        }
        else
        {
            first = "後攻";
            if (PhotonNetwork.IsMasterClient == turn.player) direction = "▲";
            else direction = "▼";
        }
        this.turn.text = direction+"Turn"+turn.now+first+direction;
        if (PhotonNetwork.IsMasterClient == turn.player) time += 20 + turn.now*2;
    }
    IEnumerator ManaProcess(Mana mana)
    {
        if (PhotonNetwork.IsMasterClient == mana.player)
        {
            this.mana = mana.now;
            if (mana.change < 0)
            {
                for (int i = -mana.change; i > 0; i--)
                {
                    playerManaDown[i + mana.now - 1].gameObject.SetActive(true);
                    playerManaDown[i + mana.now - 1].Play();
                    SEManager.Instance.Play(SEPath.MANADOWN);
                }
            }
            else
            {
                for (int i = mana.change; i > 0; i--)
                {
                    playerManaUp[mana.now - i].gameObject.SetActive(true);
                    playerManaUp[mana.now - i].Play();
                    SEManager.Instance.Play(SEPath.MANAUP);
                }
            }
            for (int i = 0; i < playerMana.Length; i++)
            {
                playerMana[i].GetComponent<Image>().color = new Color(0, 0, 0);
            }
            for (int i = 0; i < mana.next; i++)
            {
                playerMana[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            for (int i = 0; i < mana.now; i++)
            {
                playerMana[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        else
        {
            if (mana.change < 0)
            {
                for (int i = -mana.change; i > 0; i--)
                {
                    enemyManaDown[i + mana.now - 1].gameObject.SetActive(true);
                    enemyManaDown[i + mana.now - 1].Play();
                    SEManager.Instance.Play(SEPath.MANADOWN);
                }
            }
            else
            {
                for (int i = mana.change; i > 0; i--)
                {
                    enemyManaUp[mana.now - i].gameObject.SetActive(true);
                    enemyManaUp[mana.now - i].Play();
                    SEManager.Instance.Play(SEPath.MANAUP);
                }
            }
            for (int i = 0; i < enemyMana.Length; i++)
            {
                enemyMana[i].GetComponent<Image>().color = new Color(0, 0, 0);
            }
            for (int i = 0; i < mana.next; i++)
            {
                enemyMana[i].GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }
            for (int i = 0; i < mana.now; i++)
            {
                enemyMana[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        yield return new WaitForSeconds(1.0f);
        foreach (SimpleAnimation simple in playerManaDown) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in playerManaUp) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in enemyManaDown) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in enemyManaUp) simple.gameObject.SetActive(false);
    }
    IEnumerator LifeProcess(Life life)
    {
        if (PhotonNetwork.IsMasterClient == life.player)
        {
            if (life.change < 0)
            {
                for(int i= -life.change; i> 0; i--)
                {
                    playerBreak[i + life.life - 1].gameObject.SetActive(true);
                    playerBreak[i + life.life -1].Play();
                    SEManager.Instance.Play(SEPath.BREAK,volumeRate:2f- (i + life.life - 1)*0.1f);
                }
            }
            else
            {
                for (int i = life.change; i > 0; i--)
                {
                    playerHeal[life.life - i].gameObject.SetActive(true);
                    playerHeal[life.life - i].Play();
                    SEManager.Instance.Play(SEPath.LIFE_UP);
                }
            }

            for (int i = 0; i < playerLife.Length; i++)
            {
                playerLife[i].GetComponent<Image>().color = new Color(0, 0, 0);
            }
            for (int i = 0; i < life.life; i++)
            {
                playerLife[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        else
        {
            if (life.change < 0)
            {
                for (int i = -life.change; i > 0; i--)
                {
                    enemyBreak[i + life.life - 1].gameObject.SetActive(true);
                    enemyBreak[i + life.life - 1].Play();
                    SEManager.Instance.Play(SEPath.BREAK, volumeRate: 2f - (i + life.life - 1) * 0.1f);
                }
            }
            else
            {
                for (int i = life.change; i > 0; i--)
                {
                    enemyHeal[life.life - i].gameObject.SetActive(true);
                    enemyHeal[life.life - i].Play();
                    SEManager.Instance.Play(SEPath.LIFE_UP);
                }
            }
            for (int i = 0; i < enemyLife.Length; i++)
            {
                enemyLife[i].GetComponent<Image>().color = new Color(0, 0, 0);
            }
            for (int i = 0; i < life.life; i++)
            {
                enemyLife[i].GetComponent<Image>().color = new Color(1, 1, 1);
            }
        }
        yield return new WaitForSeconds(1.0f);
        foreach (SimpleAnimation simple in playerBreak) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in enemyBreak) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in playerHeal) simple.gameObject.SetActive(false);
        foreach (SimpleAnimation simple in enemyHeal) simple.gameObject.SetActive(false);
    }
    IEnumerator SideProcess(Side side)
    {
        if (side.request)
        {
            StartCoroutine(PhaseDisplay(1));
            yield return ImageUtil.ImageUtil.IllustImage(side.leftChara, charaL);
            yield return ImageUtil.ImageUtil.IllustImage(side.rightChara, charaR);
            charaL.color = new Color(0, 0, 0, 0.5f);
            charaR.color = new Color(0, 0, 0, 0.5f);
            charaL.gameObject.SetActive(true);
            charaR.gameObject.SetActive(true);
            if (PhotonNetwork.IsMasterClient == side.player)
            {
                timeClock = true;
                leftButton.SetActive(true);
                rightButton.SetActive(true);
                message.text = "サイドを選択してください";
            }
            else
            {
                message.text = "相手がサイドを選択中";
            }
        }
        else
        {
            if (side.isLeft)
            {
                StartCoroutine(PhaseDisplay(2));
                charaL.color = new Color(1, 1, 1);
            }
            else
            {
                StartCoroutine(PhaseDisplay(3));
                charaR.color = new Color(1, 1, 1);
            }

            yield return new WaitForSecondsRealtime(1.0f);
            charaL.gameObject.SetActive(false);
            charaR.gameObject.SetActive(false);

            if (PhotonNetwork.IsMasterClient == side.player)
            {
                isLeft = side.isLeft;
                if (side.isLeft)
                {
                    playerL.color = new Color(0, 0, 1, 1);
                    if (!BGMManager.Instance.GetCurrentAudioNames().Exists(b => b == side.leftChara))
                    {
                        switch (side.leftChara)
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
                else
                {
                    playerR.color = new Color(1, 0, 0, 1);
                    if (!BGMManager.Instance.GetCurrentAudioNames().Exists(b => b == side.rightChara))
                    {
                        switch (side.rightChara)
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
            }
            else
            {
                if (side.isLeft)
                {
                    enemyL.color = new Color(0, 0, 1, 1);
                }
                else
                {
                    enemyR.color = new Color(1, 0, 0, 1);
                }
            }
        }
    }
    void MainProcess(Main main)
    {
        StartCoroutine(PhaseDisplay(4));
        attackEffect.AttackEnd();
        if(PhotonNetwork.IsMasterClient == main.player)
        {
            Battle_Card card = playerField.Find(c => c.state[3] > 0 && c.act && c.state[0]== 0 && c.state[19] == 0);
            if (card != null)
            {
                Act act = new Act();
                act.act = "fieldattack";
                act.player = PhotonNetwork.IsMasterClient;
                act.order = card.order;
                act.isLeft = card.isLeft;
                Answer("act", act.ToString());
            }
            else
            {
                timeClock = true;
                mainPhase = true;
                turnEndButton.SetActive(true);
                message.text = "行動を実行してください";
            }
        }
        else
        {
            message.text = "相手が行動を思考中";
        }
    }
    void MoveProcess(Move move)
    {
        int[] order = move.order;
        string prev = move.prev;
        string after = move.after;

        foreach (int o in order)
        {
            List<Battle_Card> prevArea = null;
            Battle_Card card = allCard.Find(c => c.order == o);
            if (!card.own)card.EnemyCard(after);
            switch (prev)
            {
                case "hand":
                    if (card.own)
                    {
                        if (card.isLeft) prevArea = playerLeftHand;
                        else prevArea = playerRightHand;
                    }
                    else
                    {
                        if (card.isLeft) prevArea = enemyLeftHand;
                        else prevArea = enemyRightHand;
                    }
                    break;
                case "deck":
                    if (card.own)
                    {
                        if (card.isLeft) prevArea = playerLeftDeck;
                        else prevArea = playerRightDeck;
                    }
                    else
                    {
                        if (card.isLeft) prevArea = enemyLeftDeck;
                        else prevArea = enemyRightDeck;
                    }
                    break;
                case "cemetary":
                    if (card.own)
                    {
                        if (card.isLeft) prevArea = playerLeftCemetary;
                        else prevArea = playerRightCemetary;
                    }
                    else
                    {
                        if (card.isLeft) prevArea = enemyLeftCemetary;
                        else prevArea = enemyRightCemetary;
                    }
                    break;
                case "field":
                    if (card.own)
                    {
                        prevArea = playerField;
                    }
                    else
                    {
                        prevArea = enemyField;
                    }
                    break;
                case "equip":
                    if (card.own)
                    {
                        prevArea = playerEquip;
                    }
                    else
                    {
                        prevArea = enemyEquip;
                    }
                    break;
            }
            List<Battle_Card> afterArea = null;
            switch (after)
            {
                case "hand":
                    if (card.own)
                    {
                        if (card.isLeft)
                        {
                            afterArea = playerLeftHand;
                            if (playerLeftHand.Count == 6) afterArea = playerLeftCemetary;
                        }
                        else
                        {
                            afterArea = playerRightHand;
                            if (playerRightHand.Count == 6) afterArea = playerRightCemetary;
                        }
                    }
                    else
                    {
                        if (card.isLeft)
                        {
                            afterArea = enemyLeftHand;
                            if (enemyLeftHand.Count == 6) afterArea = enemyLeftCemetary;
                        }
                        else
                        {
                            afterArea = enemyRightHand;
                            if (enemyRightHand.Count == 6) afterArea = enemyRightCemetary;
                        }
                    }
                    break;
                case "deck":
                    if (card.own)
                    {
                        if (card.isLeft) afterArea = playerLeftDeck;
                        else afterArea = playerRightDeck;
                    }
                    else
                    {
                        if (card.isLeft) afterArea = enemyLeftDeck;
                        else afterArea = enemyRightDeck;
                    }
                    break;
                case "cemetary":
                    if (card.own)
                    {
                        if (card.isLeft) afterArea = playerLeftCemetary;
                        else afterArea = playerRightCemetary;
                    }
                    else
                    {
                        if (card.isLeft) afterArea = enemyLeftCemetary;
                        else afterArea = enemyRightCemetary;
                    }
                    break;
                case "field":
                    if (card.own)
                    {
                        afterArea = playerField;
                    }
                    else
                    {
                        afterArea = enemyField;
                    }
                    break;
                case "equip":
                    if (card.own)
                    {
                        afterArea = playerEquip;
                    }
                    else
                    {
                        afterArea = enemyEquip;
                    }
                    break;
            }
            card.area = after;
            prevArea.Remove(card);
            afterArea.Add(card);
            if (prev == "hand" && after == "field")
            {

            }
            else
            {
                card.ReSet();
            }
            if (after == "hand"||after=="equip")
            {
                StartCoroutine(card.Set());
            }
            if (after != "cemetary") card.panish.SetActive(false);
        }
        switch (prev)
        {
            case "hand":
                HandAlignment();
                break;
            case "deck":
                DeckAlignment();
                break;
            case "cemetary":
                CemetaryAlignment();
                break;
            case "field":
                FieldAlignment();
                break;
            case "equip":
                EquipAlignment();
                break;
        }
        switch (after)
        {
            case "hand":
                HandAlignment();
                CemetaryAlignment();
                break;
            case "deck":
                DeckAlignment();
                break;
            case "cemetary":
                CemetaryAlignment();
                break;
            case "field":
                FieldAlignment();
                break;
            case "equip":
                EquipAlignment();
                break;
        }
        int attack = -1;
        int block = -1;
        if (attackEffect.attackCard != null)
        {
            if (attackEffect.attackCard.own) attack = playerField.FindIndex(c => c == attackEffect.attackCard);
            else attack = enemyField.FindIndex(c => c == attackEffect.attackCard);
        }
        if (attackEffect.blockCard != null)
        {
            if (attackEffect.blockCard.own) block = playerField.FindIndex(c => c == attackEffect.blockCard);
            else block = enemyField.FindIndex(c => c == attackEffect.blockCard);
        }
        attackEffect.Move(attack, block);
    }
    void StateProcess(State state)
    {
        Battle_Card card = allCard.Find(c => c.order == state.order);
        if (state.stateNo < 0)
        {
            StartCoroutine(card.ActChange(state.value));
        }
        else
        {
            StartCoroutine(card.StateDisplay(state.stateNo, state.value));
        }
    }
    void SelectProcess(Select select)
    {

        selectList = new List<Battle_Card>();
        if (PhotonNetwork.IsMasterClient == select.player)
        {
            StartCoroutine(PhaseDisplay(5));
            foreach (int order in select.orderList)
            {
                Battle_Card card = allCard.Find(c => c.order == order);
                selectList.Add(card);
            }
            Battle_Card forced = selectList.Find(c => c.state[26] > 0 && !c.own && c.state[19]==0);

            if (forced == null)
            {
                timeClock = true;
                time += 3;
                message.text = select.message;
                if (select.cancel)
                {
                    cancelButton.SetActive(true);
                }
                foreach (Battle_Card card in selectList) card.select.SetActive(true);
            }
            else
            {
                Select answer = new Select
                {
                    player = PhotonNetwork.IsMasterClient,
                    cancel = false,
                    orderList = new int[] { forced.order }
                };
                Answer("select", answer.ToString());
            }
        }
        else
        {
            message.text = "相手が選択を思考中";
        }
    }
    void PowerProcess(Power power)
    {
        Battle_Card card = allCard.Find(c => c.order == power.order);
        StartCoroutine( card.Power(power));
    }
    void LevelProcess(Level level)
    {
        Battle_Card card = allCard.Find(c => c.order == level.order);
        StartCoroutine(card.Level(level));
    }
    IEnumerator SkillProcess(Skill skill)
    {
        if (skill.start)
        {
            Battle_Card card = allCard.Find(c => c.order == skill.order);
            StartCoroutine(card.Skill());
            this.skill.SkillDisplay(card, skill.value);
            yield return new WaitForSecondsRealtime(0.4f);
        }
        else
        {
            yield return new WaitForSecondsRealtime(0.1f);
            this.skill.gameObject.SetActive(false);
        }
    }
    IEnumerator WinProcess()
    {

        if (PlayerPrefs.GetInt("Battle") == 1)
        {
            gameEnd = true;
            PlayerPrefs.SetInt("Battle", 0);
            if (PlayerPrefs.GetInt("Rank") == 1)
            {
                int rpDiff = PlayerPrefs.GetInt("RpDiff");
                rpDiff /= 20;
                int rpChange = 20 + rpDiff;
                int rp = PlayerPrefs.GetInt("RP",1000);
                if (rp < 1200)
                {
                    if (rpChange < 0) rpChange = 0;
                }
                else if (rp < 1500)
                {
                    rpChange += 2;
                }
                else if (rp < 1800)
                {
                    rpChange += 0;
                }
                else if (rp < 2000)
                {
                    rpChange -= 2;
                }
                else
                {
                    rpChange -= 4;
                }
                if (rpChange <= 0) rpChange = 1;
                int rpResult = rp + rpChange;
                PlayerPrefs.SetInt("RP", rpResult);
                if (rpResult > PlayerPrefs.GetInt("MaxRP")) PlayerPrefs.SetInt("MaxRP", rpResult);

#if UNITY_WEBGL
#else
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                DocumentReference docRef = db.Collection("WinLose").Document(PlayerPrefs.GetString("RoomNo") + "Win");
                DateTime dateTime = System.DateTime.Now;
                string left = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L" + 0); ;
                string right = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R" + 0); ;
                for (int i = 1; i < 30; i++)
                {
                    left += "," + PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L" + i);
                    right += "," + PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R" + i);
                }
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                        { "Ver", PhotonNetwork.AppVersion },
                        { "Day", dateTime.ToShortDatePattern() },
                        { "Name", PlayerPrefs.GetString("Player","新規プレイヤー") },
                        { "Rate", PlayerPrefs.GetInt("RP",1000)},
                        { "Win", true },
                        { "Left", left },
                        { "Right", right }
                };
                docRef.SetAsync(user).ContinueWithOnMainThread(task => {
                    Debug.Log("Added data to the alovelace document in the users collection.");
                });
#endif

                BGMManager.Instance.Stop();
                foreach (SimpleAnimation anime in end) anime.transform.localPosition = new Vector3(0, 300, -1);
                end[0].gameObject.SetActive(true);
                end[0].Play();
                for (int i = 0; i < 5; i++)
                {
                    SEManager.Instance.Play(SEPath.BREAK);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
                end[1].gameObject.SetActive(true);
                end[1].Play();
                SEManager.Instance.Play(SEPath.END);
                yield return new WaitForSecondsRealtime(1.0f);

                result.gameObject.SetActive(true);
                yield return result.Win(rp, rpChange, rpResult);
            }
            else
            {
                gameEnd = true;
                BGMManager.Instance.Stop();
                foreach (SimpleAnimation anime in end) anime.transform.localPosition = new Vector3(0, 300, -1);
                end[0].gameObject.SetActive(true);
                end[0].Play();
                for (int i = 0; i < 5; i++)
                {
                    SEManager.Instance.Play(SEPath.BREAK);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
                end[1].gameObject.SetActive(true);
                end[1].Play();
                SEManager.Instance.Play(SEPath.END);
                yield return new WaitForSecondsRealtime(1.0f);

                result.gameObject.SetActive(true);
                result.FreeWin();
            }
        }
    }
    IEnumerator LoseProcess()
    {
        if (PlayerPrefs.GetInt("Battle") == 1)
        {
            gameEnd = true;
            PlayerPrefs.SetInt("Battle", 0);
            if (PlayerPrefs.GetInt("Rank") == 1)
            {

                int rpDiff = PlayerPrefs.GetInt("RpDiff");
                rpDiff /= 20;
                int rpChange = -20 + rpDiff;
                int rp = PlayerPrefs.GetInt("RP", 1000);
                if (rp < 1200)
                {
                    if (rpChange < 0) rpChange = 0;
                }
                else if (rp < 1500)
                {
                    rpChange += 2;
                }
                else if (rp < 1800)
                {
                    rpChange += 0;
                }
                else if (rp < 2000)
                {
                    rpChange -= 2;
                }
                else
                {
                    rpChange -= 4;
                }
                if (rpChange < -39) rpChange = -39;
                int rpResult = rp + rpChange;
                PlayerPrefs.SetInt("RP", rpResult);
                if (rpResult > PlayerPrefs.GetInt("MaxRP")) PlayerPrefs.SetInt("MaxRP", rpResult);
#if UNITY_WEBGL
#else
                FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
                DocumentReference docRef = db.Collection("WinLose").Document(PlayerPrefs.GetString("RoomNo") + "Lose");
                DateTime dateTime = System.DateTime.Now;
                string left = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L" + 0); ;
                string right = PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R" + 0); ;
                for (int i = 1; i < 30; i++)
                {
                    left += "," + PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "L" + i);
                    right += "," + PlayerPrefs.GetString("D" + (PlayerPrefs.GetInt("SelectDeck", 0) + 1) + "R" + i);
                }
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                        { "Ver", PhotonNetwork.AppVersion },
                        { "Day", dateTime.ToShortDatePattern() },
                        { "Name", PlayerPrefs.GetString("Player","新規プレイヤー") },
                        { "Rate", PlayerPrefs.GetInt("RP",1000)},
                        { "Win", false },
                        { "Left", left },
                        { "Right", right }
                };
                docRef.SetAsync(user).ContinueWithOnMainThread(task => {
                    Debug.Log("Added data to the alovelace document in the users collection.");
                });
#endif
                BGMManager.Instance.Stop();
                foreach (SimpleAnimation anime in end) anime.transform.localPosition = new Vector3(0, -300, -1);
                end[0].gameObject.SetActive(true);
                end[0].Play();
                for (int i = 0; i < 5; i++)
                {
                    SEManager.Instance.Play(SEPath.BREAK);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
                end[1].gameObject.SetActive(true);
                end[1].Play();
                SEManager.Instance.Play(SEPath.END);
                yield return new WaitForSecondsRealtime(1.0f);



                result.gameObject.SetActive(true);
                yield return result.Lose(rp, rpChange, rpResult);
            }
            else
            {
                BGMManager.Instance.Stop();
                foreach (SimpleAnimation anime in end) anime.transform.localPosition = new Vector3(0, -300, -1);
                end[0].gameObject.SetActive(true);
                end[0].Play();
                for (int i = 0; i < 5; i++)
                {
                    SEManager.Instance.Play(SEPath.BREAK);
                    yield return new WaitForSecondsRealtime(0.2f);
                }
                end[1].gameObject.SetActive(true);
                end[1].Play();
                SEManager.Instance.Play(SEPath.END);
                yield return new WaitForSecondsRealtime(1.0f);

                result.gameObject.SetActive(true);
                result.FreeLose();
            }
        }
    }

    public void Receive(string process, string content)
    {
        Process p = new Process(process, content);
        processList.Add(p);
        //Debug.Log("player:" + process +":"+content);
    }
    class Process
    {
        public string process;
        public string content;
        public Process(string p, string c)
        {
            process = p;
            content = c;
        }
    }
    void AllAlignment()
    {
        DeckAlignment();
        HandAlignment();
        CemetaryAlignment();
        FieldAlignment();
        EquipAlignment();
    }
    void DeckAlignment()
    {
        playerLeftDeck.Sort((a, b) => a.order - b.order);
        playerRightDeck.Sort((a, b) => a.order - b.order);
        enemyLeftDeck.Sort((a, b) => a.order - b.order);
        enemyRightDeck.Sort((a, b) => a.order - b.order);

        for (int i = 0; i < playerLeftDeck.Count; i++)
        {
            Battle_Card cardObj = playerLeftDeck[i];
            cardObj.transform.SetParent(playerLeftDeckObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < playerRightDeck.Count; i++)
        {
            Battle_Card cardObj = playerRightDeck[i];
            cardObj.transform.SetParent(playerRightDeckObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < enemyLeftDeck.Count; i++)
        {
            Battle_Card cardObj = enemyLeftDeck[i];
            cardObj.transform.SetParent(enemyLeftDeckObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < enemyRightDeck.Count; i++)
        {
            Battle_Card cardObj = enemyRightDeck[i];
            cardObj.transform.SetParent(enemyRightDeckObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        playerLeftDeckNum.text = "山札\n" + playerLeftDeck.Count + "枚";
        playerRightDeckNum.text = "山札\n" + playerRightDeck.Count + "枚";
        enemyLeftDeckNum.text = "山札\n" + enemyLeftDeck.Count + "枚";
        enemyRightDeckNum.text = "山札\n" + enemyRightDeck.Count + "枚";
    }
    void HandAlignment()
    {
        for (int i = 0; i < playerLeftHand.Count; i++)
        {
            Battle_Card cardObj = playerLeftHand[i];
            cardObj.transform.SetParent(playerLeftHandObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * i , 0);
        }
        for (int i = 0; i < playerRightHand.Count; i++)
        {
            Battle_Card cardObj = playerRightHand[i];
            cardObj.transform.SetParent(playerRightHandObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * i , 0);
        }
        for (int i = 0; i < enemyLeftHand.Count; i++)
        {
            Battle_Card cardObj = enemyLeftHand[i];
            cardObj.transform.SetParent(enemyLeftHandObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * i , 0);
        }
        for (int i = 0; i < enemyRightHand.Count; i++)
        {
            Battle_Card cardObj = enemyRightHand[i];
            cardObj.transform.SetParent(enemyRightHandObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition =  new Vector2(200 * i + 400, 0);
        }
    }
    void CemetaryAlignment()
    {
        for (int i = 0; i < playerLeftCemetary.Count; i++)
        {
            Battle_Card cardObj = playerLeftCemetary[i];
            cardObj.transform.SetParent(playerLeftCemetaryObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < playerRightCemetary.Count; i++)
        {
            Battle_Card cardObj = playerRightCemetary[i];
            cardObj.transform.SetParent(playerRightCemetaryObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < enemyLeftCemetary.Count; i++)
        {
            Battle_Card cardObj = enemyLeftCemetary[i];
            cardObj.transform.SetParent(enemyLeftCemetaryObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        for (int i = 0; i < enemyRightCemetary.Count; i++)
        {
            Battle_Card cardObj = enemyRightCemetary[i];
            cardObj.transform.SetParent(enemyRightCemetaryObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 6), -200 * (i / 6));
        }
        playerLeftCemetaryNum.text = "墓地\n" + (playerLeftCemetary.Count - playerLeftCemetary.FindAll(c=>c.panish.activeSelf).Count) + "枚";
        playerRightCemetaryNum.text = "墓地\n" + (playerRightCemetary.Count - playerRightCemetary.FindAll(c => c.panish.activeSelf).Count) + "枚";
        enemyLeftCemetaryNum.text = "墓地\n" + (enemyLeftCemetary.Count - enemyLeftCemetary.FindAll(c => c.panish.activeSelf).Count) + "枚";
        enemyRightCemetaryNum.text = "墓地\n" + (enemyRightCemetary.Count - enemyRightCemetary.FindAll(c => c.panish.activeSelf).Count) + "枚";
    }
    void FieldAlignment()
    {
        for (int i = 0; i < playerField.Count; i++)
        {
            Battle_Card cardObj = playerField[i];
            cardObj.transform.SetParent(playerFieldObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * i, 0);
        }
        for (int i = 5; i > 5 - enemyField.Count; i--)
        {
            Battle_Card cardObj = enemyField[5-i];
            cardObj.transform.SetParent(enemyFieldObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(800-200 * (5-i), 0);
        }
    }
    void EquipAlignment()
    {
        for (int i = 0; i < playerEquip.Count; i++)
        {
            Battle_Card cardObj = playerEquip[i];
            cardObj.transform.SetParent(playerEquipObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i%2), -200 * (i/2));
        }
        for (int i = 0; i < enemyEquip.Count; i++)
        {
            Battle_Card cardObj = enemyEquip[i];
            cardObj.transform.SetParent(enemyEquipObj.transform);
            cardObj.transform.localScale = new Vector3(1, 1, 1);
            cardObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(200 * (i % 2), -200 * (i / 2));
        }
    }
    public void DragStart(Battle_Card card)
    {
        if (mainPhase)
        {
            dropList = new List<Battle_Card>();
            if(card.area == "hand")
            {
                dropArea[1].SetActive(true);
                dropArea[3].SetActive(true);
            }
            if (card.area == "hand" && card.card.type == "ユニット")
            {
                if (playerField.Count < 5)
                {
                    if (card.card.cost <= mana) dropArea[0].SetActive(true);
                    else
                    {
                        foreach (Battle_Card battle in playerEquip)
                        {
                            if (battle.card.type != "アイテム" && card.card.color == battle.card.color)
                            {
                                if (card.card.cost <= mana + 1) dropArea[0].SetActive(true);
                            }
                        }
                    }
                }
            }
            if (card.area == "hand" && card.card.type == "マジック")
            {
                if (card.card.cost <= mana) dropArea[0].SetActive(true);
                else
                {
                    foreach (Battle_Card battle in playerEquip)
                    {
                        if (battle.card.type != "アイテム" && card.card.color == battle.card.color)
                        {
                            if (card.card.cost <= mana + 1) dropArea[0].SetActive(true);
                        }
                    }
                }
            }
            if (card.area == "hand" && (card.card.type == "ユニット" || card.card.type == "進化ユニット") && card.card.skill1!= "合札禁止" && card.card.skill2 != "合札禁止" && card.card.skill3 != "合札禁止")
            {
                if (card.isLeft)
                {
                    foreach (Battle_Card battle in playerLeftHand)
                    {
                        if (card.card.title == battle.card.title && card.order != battle.order)
                        {
                            dropList.Add(battle);
                            battle.merge.gameObject.SetActive(true);
                        }
                    }
                }
                else
                {
                    foreach (Battle_Card battle in playerRightHand)
                    {
                        if (card.card.title == battle.card.title && card.order != battle.order)
                        {
                            dropList.Add(battle);
                            battle.merge.gameObject.SetActive(true);
                        }
                    }
                }
            }
            if (card.area == "hand" && card.card.type == "進化ユニット")
            {
                bool b = false;
                if (card.card.cost <= mana) b=true;
                else
                {
                    foreach (Battle_Card battle in playerEquip)
                    {
                        if (battle.card.type != "アイテム" && card.card.color == battle.card.color)
                        {
                            if (card.card.cost <= mana + 1) b=true;
                        }
                    }
                }

                if (b)
                {
                    foreach (Battle_Card battle in playerField)
                    {
                        if (card.card.color == battle.card.color)
                        {
                            dropList.Add(battle);
                            battle.generate.gameObject.SetActive(true);
                        }
                    }
                }
            }
            if (card.area == "field")
            {
                dropArea[3].SetActive(true);
                if (card.act)
                {
                    if (card.state[1] == 0)
                    {
                        if(card.state[0]==0 || (card.state[0]!=0 && card.state[14] == 1 && card.state[19] == 0))
                        {
                            dropArea[2].SetActive(true);
                        }
                    }
                }
            }
        }
    }
    public void DragEnd()
    {
        if (mainPhase)
        {
            foreach (GameObject obj in dropArea)
            {
                obj.SetActive(false);
            }
            foreach (Battle_Card obj in dropList)
            {
                obj.merge.gameObject.SetActive(false);
                obj.generate.gameObject.SetActive(false);
            }
        }
    }
    public void LeftRightButton(bool isLeft)
    {
        timeClock = false;
        SEManager.Instance.Play(SEPath.CLICK);
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        Side side = new Side();
        side.player = PhotonNetwork.IsMasterClient;
        side.isLeft = isLeft;
        Answer("side", side.ToString());
    }
    public void TurnEndButton()
    {
        timeClock = false;
        SEManager.Instance.Play(SEPath.CLICK);
        mainPhase = false;
        turnEndButton.SetActive(false);
        Act act = new Act();
        act.player = PhotonNetwork.IsMasterClient;
        act.act = "end";
        Answer("act", act.ToString());
    }
    public void SelectCard(int order)
    {
        timeClock = false;
        SEManager.Instance.Play(SEPath.CLICK);
        cancelButton.SetActive(false);
        foreach(Battle_Card card in selectList)card.select.SetActive(false);
        Select select = new Select
        {
            player = PhotonNetwork.IsMasterClient,
            cancel = false,
            orderList = new int[] { order }
        };
        Answer("select", select.ToString());
    }
    public void CancelButton()
    {
        timeClock = false;
        SEManager.Instance.Play(SEPath.CLICK);
        cancelButton.SetActive(false);
        foreach (Battle_Card card in selectList) card.select.SetActive(false);
        Select select = new Select();
        select.player = PhotonNetwork.IsMasterClient;
        select.cancel = true;
        Answer("select", select.ToString());
    }
    IEnumerator PhaseDisplay(int j)
    {
        phase.sprite = phaseSprite[j];
        phase.transform.localPosition = phasePosition[0].transform.localPosition;
        phase.gameObject.SetActive(true);
        SEManager.Instance.Play(SEPath.PHASE);
        for (int i = 0; i < 20; i++)
        {
            phase.transform.localPosition = Vector3.MoveTowards(phase.transform.localPosition, phasePosition[1].transform.localPosition, 100);
            yield return new WaitForSeconds(0.001f);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 20; i++)
        {
            phase.transform.localPosition = Vector3.MoveTowards(phase.transform.localPosition, phasePosition[2].transform.localPosition, 100);
            yield return new WaitForSeconds(0.001f);
        }
        phase.gameObject.SetActive(false);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        StartCoroutine(WinProcess());
    }
    public override void OnLeftRoom()
    {
        StartCoroutine(LoseProcess());
    }
    public void Retire()
    {
        menu.SetActive(false);
        PhotonNetwork.LeaveRoom();
        StartCoroutine(LoseProcess());
    }
    public void MenuButton()
    {
        if (menu.activeSelf)
        {
            menuText.text = "メニュー";
            menu.SetActive(false);
        }
        else
        {
            menuText.text = "閉じる";
            menu.SetActive(true);
        }
    }
    public IEnumerator TimeClock()
    {
        while (!gameEnd)
        {
            yield return new WaitForSeconds(1.0f);
            if (timeClock)
            {
                time--;
                if (time > 99) time = 99;
                if (time < 10)
                {
                    timeText.color = new Color(1, 0, 0);
                    SEManager.Instance.Play(SEPath.COUNT_DOWN);
                }
                else
                {
                    timeText.color = new Color(0.2f, 0.2f, 0.2f);
                }
                timeText.text = time.ToString() + "秒";
                if (time < 0) Retire();
            }
        }
    }

    public void Answer(string process, string content)
    {
        pun.Answer(process, content);
    }
    public void LeftScroll()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        scrollbar.value = 0;
    }
    public void RightScroll()
    {
        SEManager.Instance.Play(SEPath.CLICK);
        scrollbar.value = 1;
    }
}
public static class DateTimeExtensions
{
    /// <summary>
    /// yyyy/MM/dd HH:mm:ss 形式の文字列に変換して返します
    /// </summary>
    public static string ToPattern(this DateTime self)
    {
        return self.ToString("yyyy/MM/dd HH:mm:ss");
    }

    /// <summary>
    /// yyyy/MM/dd 形式の文字列に変換して返します
    /// </summary>
    public static string ToShortDatePattern(this DateTime self)
    {
        return self.ToString("yyyy/MM/dd");
    }

    /// <summary>
    /// yyyy年M月d日 形式の文字列に変換して返します
    /// </summary>
    public static string ToLongDatePattern(this DateTime self)
    {
        return self.ToString("yyyy年M月d日");
    }

    /// <summary>
    /// yyyy年M月d日 HH:mm:ss 形式の文字列に変換して返します
    /// </summary>
    public static string ToFullDateTimePattern(this DateTime self)
    {
        return self.ToString("yyyy年M月d日 HH:mm:ss");
    }

    /// <summary>
    /// HH:mm 形式の文字列に変換して返します
    /// </summary>
    public static string ToShortTimePattern(this DateTime self)
    {
        return self.ToString("HH:mm");
    }

    /// <summary>
    /// HH:mm:ss 形式の文字列に変換して返します
    /// </summary>
    public static string ToLongTimePattern(this DateTime self)
    {
        return self.ToString("HH:mm:ss");
    }
}