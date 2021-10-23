using BattleJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Battle_Rule : MonoBehaviour
{
    List<Rule_Card> allCard = new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerLeftDeck = new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerLeftHand= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerLeftCemetary= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerRightDeck= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerRightHand= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerRightCemetary= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerField= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> playerEquip= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyLeftDeck= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyLeftHand= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyLeftCemetary= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyRightDeck= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyRightHand= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyRightCemetary= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyField= new List<Rule_Card>();
    [SerializeReference] List<Rule_Card> enemyEquip= new List<Rule_Card>();
    List<Event> eventList = new List<Event>();
    string playerCharaL;
    string playerCharaR;
    string enemyCharaL;
    string enemyCharaR;

    int turn;
    bool firstPlayer;
    bool first;
    int playerLife = 7;
    int enemyLife = 7;
    int playerMana = 0;
    int enemyMana = 0;
    int playerManaNext = 0;
    int enemyManaNext = 0;
    public Battle_Pun pun;
    public CardMaster cardMaster;

    bool answerPlayer;
    string process;
    string content;
    string processPlayer;
    string contentPlayer;
    string processEnemy;
    string contentEnemy;
    bool eventProcess = false;
    int eventNo = 0;
    public void Initial()
    {
        StartCoroutine(RuleIE());
    }
    IEnumerator RuleIE()
    {
        firstPlayer = UnityEngine.Random.Range(0, 2) == 0;
        yield return PlayerInfo();
        yield return Mulligan();



        turn = 0;
        Draw(true, 3, 3);
        Draw(false, 3, 3);
        Mana(firstPlayer, 0, 1);
        Mana(!firstPlayer, 0, 2);
        while (turn<10)
        {
            turn++;
            //先攻
            first = true;
            yield return Turn(firstPlayer, true, turn);
            yield return Side(firstPlayer);
            yield return Main(firstPlayer);


            //後攻
            first = false;
            yield return Turn(!firstPlayer, false, turn);
            yield return Side(!firstPlayer);
            yield return Main(!firstPlayer);

            yield return new WaitForSecondsRealtime(0.1f);
        }
        bool winner;
        if (firstPlayer)
        {
            if (playerLife > enemyLife) winner = true;
            else winner = false;
        }
        else
        {
            if (playerLife < enemyLife) winner = false;
            else winner = true;
        }
        Finish finish = new Finish()
        {
            winner = winner
        };
        ProcessRequest("finish", finish.ToString());
    }
    IEnumerator PlayerInfo()
    {
        ProcessRequest("info", "");
        yield return AnswerWaitW("info");
        Info playerInfo = Info.ToInfo(contentPlayer);
        Info enemyInfo = Info.ToInfo(contentEnemy);
        playerInfo.first = playerInfo.player == firstPlayer;
        enemyInfo.first = enemyInfo.player == firstPlayer;
        ProcessRequest("match", playerInfo.ToString());
        ProcessRequest("match", enemyInfo.ToString());
        DeckCreate(playerInfo);
        DeckCreate(enemyInfo);
        ProcessRequest("deck", playerInfo.ToString());
        ProcessRequest("deck", enemyInfo.ToString());
    }
    public void DeckCreate(Info info)
    {
        int enemy;
        if (info.player) enemy = 0;
        else enemy =60;
        List<int> ramdom = new List<int>
        {
            0,
            1,
            2
        };
        ramdom = ramdom.OrderBy(a => Guid.NewGuid()).ToList();
        for (int i = 0; i < 30; i++)
        {
            Card card = cardMaster.CardList.Find(c => c.id == info.leftDeck[i]);
            Rule_Card rule_Card = new Rule_Card();
            rule_Card.Initial(card, i+enemy, true,info.player);
            if (info.player) playerLeftDeck.Add(rule_Card);
            else enemyLeftDeck.Add(rule_Card);
        }
        List<int> suffleLeft = new List<int>();
        List<int> tmp0 = new List<int>();
        List<int> tmp1 = new List<int>();
        List<int> tmp2 = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            if (i % 3 == 0) tmp0.Add(i + enemy);
            if (i % 3 == 1) tmp1.Add(i + enemy);
            if (i % 3 == 2) tmp2.Add(i + enemy);
        }
        tmp0 = tmp0.OrderBy(a => Guid.NewGuid()).ToList();
        tmp1 = tmp1.OrderBy(a => Guid.NewGuid()).ToList();
        tmp2 = tmp2.OrderBy(a => Guid.NewGuid()).ToList();
        for (int i = 0; i < 3; i++)
        {
            if (ramdom[i] == 0) suffleLeft.AddRange(tmp0);
            if (ramdom[i] == 1) suffleLeft.AddRange(tmp1);
            if (ramdom[i] == 2) suffleLeft.AddRange(tmp2);
        }
        for (int i = 0; i < 30; i++)
        {
            Rule_Card rule_Card;
            if (info.player) rule_Card = playerLeftDeck.Find(c => c.order == suffleLeft[i]);
            else rule_Card = enemyLeftDeck.Find(c => c.order == suffleLeft[i]);
            if (info.player)
            {
                playerLeftDeck.Remove(rule_Card);
                playerLeftDeck.Add(rule_Card);
            }
            else
            {
                enemyLeftDeck.Remove(rule_Card);
                enemyLeftDeck.Add(rule_Card);
            }
            allCard.Add(rule_Card);
        }
        for (int i = 0; i < 30; i++)
        {
            Card card = cardMaster.CardList.Find(c => c.id == info.rightDeck[i]);
            Rule_Card rule_Card = new Rule_Card();
            rule_Card.Initial(card, i+30 + enemy, false, info.player);
            if (info.player) playerRightDeck.Add(rule_Card);
            else enemyRightDeck.Add(rule_Card);
        }
        ramdom = ramdom.OrderBy(a => Guid.NewGuid()).ToList();
        List<int> suffleRight = new List<int>();
        tmp0 = new List<int>();
        tmp1 = new List<int>();
        tmp2 = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            if (i % 3 == 0) tmp0.Add(i + 30 + enemy);
            if (i % 3 == 1) tmp1.Add(i + 30 + enemy);
            if (i % 3 == 2) tmp2.Add(i + 30 + enemy);
        }
        tmp0 = tmp0.OrderBy(a => Guid.NewGuid()).ToList();
        tmp1 = tmp1.OrderBy(a => Guid.NewGuid()).ToList();
        tmp2 = tmp2.OrderBy(a => Guid.NewGuid()).ToList();
        for (int i = 0; i < 3; i++)
        {
            if (ramdom[i] == 0) suffleRight.AddRange(tmp0);
            if (ramdom[i] == 1) suffleRight.AddRange(tmp1);
            if (ramdom[i] == 2) suffleRight.AddRange(tmp2);
        }
        for (int i = 0; i < 30; i++)
        {
            Rule_Card rule_Card;
            if (info.player) rule_Card = playerRightDeck.Find(c => c.order == suffleRight[i]);
            else rule_Card = enemyRightDeck.Find(c => c.order == suffleRight[i]);
            if (info.player)
            {
                playerRightDeck.Remove(rule_Card);
                playerRightDeck.Add(rule_Card);
            }
            else
            {
                enemyRightDeck.Remove(rule_Card);
                enemyRightDeck.Add(rule_Card);
            }
            allCard.Add(rule_Card);
        }
        if (info.player)
        {
            playerCharaL = info.leftChara;
            playerCharaR = info.rightChara;
        }
        else
        {
            enemyCharaL = info.leftChara;
            enemyCharaR = info.rightChara;
        }
    }
    IEnumerator Mulligan()
    {
        Mulligan mulliganPlayer = new Mulligan
        {
            player = true,
            leftDeck = new string[] { playerLeftDeck[0].id, playerLeftDeck[1].id, playerLeftDeck[2].id },
            rightDeck = new string[] { playerRightDeck[0].id, playerRightDeck[1].id, playerRightDeck[2].id }
        };
        Mulligan mulliganEnemy = new Mulligan
        {
            player = false,
            leftDeck = new string[] { enemyLeftDeck[0].id, enemyLeftDeck[1].id, enemyLeftDeck[2].id },
            rightDeck = new string[] { enemyRightDeck[0].id, enemyRightDeck[1].id, enemyRightDeck[2].id }
        };
        ProcessRequest("mulligan", mulliganEnemy.ToString());
        ProcessRequest("mulligan", mulliganPlayer.ToString());
        yield return AnswerWaitW("mulligan");
        mulliganPlayer = BattleJson.Mulligan.ToMulligan(contentPlayer);
        mulliganEnemy = BattleJson.Mulligan.ToMulligan(contentEnemy);
        for(int i = 2; i >= 0; i--)
        {
            if (mulliganPlayer.left[i])
            {
                Rule_Card card = playerLeftDeck[i];
                playerLeftDeck.Remove(card);
                playerLeftDeck.Add(card);
            }
            if (mulliganPlayer.right[i])
            {
                Rule_Card card = playerRightDeck[i];
                playerRightDeck.Remove(card);
                playerRightDeck.Add(card);
            }
            if (mulliganEnemy.left[i])
            {
                Rule_Card card = enemyLeftDeck[i];
                enemyLeftDeck.Remove(card);
                enemyLeftDeck.Add(card);
            }
            if (mulliganEnemy.right[i])
            {
                Rule_Card card = enemyRightDeck[i];
                enemyRightDeck.Remove(card);
                enemyRightDeck.Add(card);
            }
        }

        
    }

    IEnumerator Main(bool player)
    {
        bool end = true;
        while (end)
        {
            Main main = new Main
            {
                player = player
            };
            ProcessRequest("main", main.ToString());
            yield return AnswerWait(player, "act");
            Act act = BattleJson.Act.ToAct(content);
            Rule_Card card = allCard.Find(c => c.order == act.order); ;
            Rule_Card target = allCard.Find(c => c.order == act.orderTarget);
            Rule_Card reduce = null;
            Event ev;

            switch (act.act)
            {
                case "handcemetary":
                    Move(card.ToList(), "hand", "cemetary");
                    break;
                case "handfield":
                    if (card.card.cost > 0)
                    {
                        if (act.player)
                        {
                            foreach (Rule_Card rule in playerEquip)
                            {
                                if (rule.card.type != "アイテム" && card.card.color == rule.card.color)
                                {
                                    reduce = rule;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Rule_Card rule in enemyEquip)
                            {
                                if (rule.card.type != "アイテム" && card.card.color == rule.card.color)
                                {
                                    reduce = rule;
                                    break;
                                }
                            }
                        }
                    }
                    if (card.card.type == "ユニット")
                    {
                        if (reduce != null)
                        {
                            Move(reduce.ToList(), "equip", "cemetary");
                            ManaAdd(act.player, 1 - card.card.cost);
                            Move(card.ToList(), "hand", "field");
                            yield return Summon(card,false);
                        }
                        else
                        {
                            ManaAdd(act.player, -card.card.cost);
                            Move(card.ToList(), "hand", "field");
                            yield return Summon(card, false);
                        }
                    }
                    else
                    {
                        CardEffect effect = new CardEffect()
                        {
                            order = card.order
                        };
                        ProcessRequest("magic", effect.ToString());
                        if (reduce != null)
                        {
                            Move(reduce.ToList(), "equip", "cemetary");
                            ManaAdd(act.player, 1 - card.card.cost);
                            Move(card.ToList(), "hand", "cemetary");
                            yield return Magic(card);
                        }
                        else
                        {
                            ManaAdd(act.player, -card.card.cost);
                            Move(card.ToList(), "hand", "cemetary");
                            yield return Magic(card);
                        }
                    }
                    break;
                case "handequip":
                    if (act.player)
                    {
                        if (playerEquip.Count == 4) Move(playerEquip[0].ToList(), "equip", "cemetary");
                    }
                    else
                    {
                        if (enemyEquip.Count == 4) Move(enemyEquip[0].ToList(), "equip", "cemetary");
                    }
                    Move(card.ToList(), "hand", "equip");
                    break;
                case "fieldcemetary":
                    Move(card.ToList(), "field", "cemetary");
                    break;
                case "fieldattack":
                    yield return Act(card.ToList(), 0, false, card.player);
                    Battle battle = new Battle();
                    battle.attack = card.order;
                    ProcessRequest("attack", battle.ToString());
                    ev = new Event()
                    {
                        type = "attack",
                        player = card.player,
                        targetCard = card.ToList()
                    };
                    eventList.Add(ev);
                    yield return EventStart();
                    if (!TargetList(card.player, "player", "field").Exists(c => c == card)) break;

                    List<Rule_Card> selectList = new List<Rule_Card>();
                    if (act.player)
                    {
                        foreach (Rule_Card rule in enemyField)
                        {
                            if (rule.act) selectList.Add(rule);
                        }
                    }
                    else
                    {
                        foreach (Rule_Card rule in playerField)
                        {
                            if (rule.act) selectList.Add(rule);
                        }
                    }
                    selectList.RemoveAll(c => c.StateValue(2) > 0);
                    if (card.StateValue(16) > 0) selectList.RemoveAll(c => c.card.cost >= card.StateValue(16));
                    if (card.StateValue(18) > 0) selectList.RemoveAll(c => c.card.cost <= card.StateValue(18));

                    if (selectList.Count == 0)
                    {
                        ProcessRequest("break", battle.ToString());
                        ev = new Event
                        {
                            type = "break",
                            player = card.player,
                            targetCard = card.ToList()
                        };
                        eventList.Add(ev);
                        yield return Life(!act.player, -1);
                        if (card.StateValue(24) > 0) yield return Life(!act.player, -card.StateValue(24));
                    }
                    else
                    {
                        Rule_Card forced = selectList.Find(c => c.StateValue(4) > 0);
                        if (forced != null)
                        {
                            battle.defence = forced.order;
                            ProcessRequest("block", battle.ToString());
                            ev = new Event()
                            {
                                type = "dffence",
                                targetCard = forced.ToList()
                            };
                            eventList.Add(ev);
                            yield return EventStart();
                            yield return Battle(card, forced);
                        }
                        else
                        {
                            Select(!act.player, selectList, true, "防御するユニットを選んでください");
                            yield return AnswerWait(!act.player, "select");
                            Select select = BattleJson.Select.ToSelect(content);
                            if (select.cancel)
                            {
                                ProcessRequest("break", battle.ToString());
                                ev = new Event
                                {
                                    type = "break",
                                    player = card.player,
                                    targetCard = card.ToList()
                                };
                                eventList.Add(ev);
                                yield return Life(!act.player, -1);
                                if (card.StateValue(24) > 0) yield return Life(!act.player, -card.StateValue(24));
                            }
                            else
                            {
                                Rule_Card selected = allCard.Find(c => c.order == select.orderList[0]);
                                battle.defence = selected.order;
                                ProcessRequest("block", battle.ToString());
                                ev = new Event()
                                {
                                    type = "dffence",
                                    targetCard = selected.ToList()
                                };
                                eventList.Add(ev);
                                yield return EventStart();
                                yield return Battle(card, selected);
                            }
                        }
                    }
                    break;
                case "merge":
                    Move(card.ToList(), "hand", "cemetary");
                    if (card.isLeft) Draw(act.player, 1, 0);
                    else Draw(act.player, 0, 1);
                    yield return Level(target.ToList(), 1);
                    break;
                case "generate":
                    if (card.card.cost > 0)
                    {
                        if (act.player)
                        {
                            foreach (Rule_Card rule in playerEquip)
                            {
                                if (rule.card.type != "アイテム" && card.card.color == rule.card.color)
                                {
                                    reduce = rule;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Rule_Card rule in enemyEquip)
                            {
                                if (rule.card.type != "アイテム" && card.card.color == rule.card.color)
                                {
                                    reduce = rule;
                                    break;
                                }
                            }
                        }
                    }
                    if (reduce != null)
                    {
                        bool targetAct = target.act;
                        Move(reduce.ToList(), "equip", "cemetary");
                        ManaAdd(act.player, 1 - card.card.cost);
                        Move(target.ToList(), "field", "cemetary");
                        Move(card.ToList(), "hand", "field");
                        if (!targetAct) yield return Act(card.ToList(), 0, false, false);
                        yield return Summon(card, false);
                    }
                    else
                    {
                        bool targetAct = target.act;
                        ManaAdd(act.player, -card.card.cost);
                        Move(target.ToList(), "field", "cemetary");
                        Move(card.ToList(), "hand", "field");
                        if (!targetAct) yield return Act(card.ToList(), 0, false, false);
                        yield return Summon(card, false);
                    }
                    break;
                case "end":
                    yield return End(act.player);
                    end = false;
                    break;
            }
        }
    }

    void Draw(bool player, bool isLeft, int i)
    {
        if (isLeft) Draw(player, i, 0);
        else Draw(player, 0, i);
    }
    void Draw(bool player, int left, int right)
    {
        List<Rule_Card> cardList = new List<Rule_Card>();
        List<Rule_Card> hand;
        if (player) hand = playerLeftHand;
        else hand = enemyLeftHand;
        while (left + hand.Count > 6)
        {
            left--;
        }
        if (player) hand = playerRightHand;
        else hand = enemyRightHand;
        while (right + hand.Count > 6)
        {
            right--;
        }
        List<Rule_Card> deck;
        if (player) deck = playerLeftDeck;
        else deck = enemyLeftDeck;
        if (deck.Count < left)
        {
            List<Rule_Card> cemetary;
            if (player) cemetary = playerLeftCemetary;
            else cemetary = enemyLeftCemetary;

            cemetary = cemetary.OrderBy(a => Guid.NewGuid()).ToList();
            Move(cemetary, "cemetary", "deck");
        }
        if (player) deck = playerRightDeck;
        else deck = enemyRightDeck;
        if (deck.Count < right)
        {
            List<Rule_Card> cemetary;
            if (player) cemetary = playerRightCemetary;
            else cemetary = enemyRightCemetary;

            cemetary = cemetary.OrderBy(a => Guid.NewGuid()).ToList();
            Move(cemetary, "cemetary", "deck");
        }


        if (player)
        {
            for (int i = 0; i < left; i++) cardList.Add(playerLeftDeck[i]);
            for (int i = 0; i < right; i++) cardList.Add(playerRightDeck[i]);
        }
        else
        {
            for (int i = 0; i < left; i++) cardList.Add(enemyLeftDeck[i]);
            for (int i = 0; i < right; i++) cardList.Add(enemyRightDeck[i]);
        }
        Draw(cardList);
    }
    void Draw(List<Rule_Card> cardList)
    {
        Move(cardList, "deck", "hand");
    }
    void NextManaAdd(bool player, int change)
    {
        if (player) Mana(player, playerMana, playerManaNext + change);
        else Mana(player, enemyMana, enemyManaNext + change);
    }
    void ManaAdd(bool player, int change)
    {
        if (player) Mana(player, playerMana + change, playerManaNext);
        else Mana(player, enemyMana + change, enemyManaNext);
    }
    void Mana(bool player,int now ,int next)
    {
        if (now > 8) now = 8;
        if (now < 0) now = 0;
        if (next > 8) next = 8;
        if (next < 0) next = 0;


        int change;
        if (player)
        {
            change = now - playerMana;
            playerMana = now;
            playerManaNext = next;
        }
        else
        {
            change = now - enemyMana;
            enemyMana = now;
            enemyManaNext = next;
        }

        Mana mana = new Mana
        {
            change =change,
            player = player,
            now = now,
            next = next
        };
        ProcessRequest("mana", mana.ToString());
    }
    IEnumerator Turn(bool player,bool first,int now)
    {
        Turn turn = new Turn
        {
            player = player,
            first = first,
            now = now
        };
        ProcessRequest("turn", turn.ToString());

        int nextMana = now + 1;
        if (nextMana > 7) nextMana = 7;
        if (player) Mana(player, playerManaNext, nextMana);
        else Mana(player, enemyManaNext, nextMana);
        if (now != 1 || !first) Draw(player, 1, 1);
        List<Rule_Card> actList = TargetList(player,"player","field");
        actList.RemoveAll(c => c.StateValue(28) > 0);
        yield return Act(actList, 1,false,false);
    }
    IEnumerator State(List<Rule_Card> cardList, int stateNo, int value, bool turn)
    {
        if (stateNo == 7 || stateNo == 8) yield break;
        foreach (Rule_Card card in cardList)
        {
            if (card.StateValue(20) > 0 && value > 0) continue;
            int prevValue = card.state[stateNo] + card.turnState[stateNo];

            if (!turn)
            {
                if(stateNo == 24 || stateNo == 27)
                {
                    card.state[stateNo] += value;
                }
                else if (stateNo == 16 || stateNo == 18)
                {
                    if (card.state[stateNo] == 0 && card.turnState[stateNo] == 0 && value > 0) card.state[stateNo] = value;
                    if (value == 0) card.state[stateNo] = value;
                }
                else
                {
                    card.state[stateNo] = value;
                }
            }
            else
            {
                if (stateNo == 24 || stateNo == 27)
                {
                    card.turnState[stateNo] += value;
                }
                else if (stateNo == 16 || stateNo == 18)
                {
                    if (card.state[stateNo] == 0 && card.turnState[stateNo] == 0 && value >0) card.turnState[stateNo] = value;
                    if (value==0) card.turnState[stateNo] = value;
                }
                else
                {
                    card.turnState[stateNo] = value;
                }
            }

            int afterValue = card.state[stateNo] + card.turnState[stateNo];

            if (prevValue != afterValue)
            {
                State state = new State
                {
                    order = card.order,
                    stateNo = stateNo,
                    value = afterValue
                };
                ProcessRequest("state", state.ToString());

                if (stateNo == 19)
                {
                    Event ev = new Event
                    {
                        type = "state",
                        targetCard = card.ToList(),
                        value = value,
                        state = stateNo
                    };
                    eventList.Add(ev);
                    yield return EventStart();
                }
            }
        }
    }
    void StateNoStart(List<Rule_Card> cardList, int stateNo, int value, bool turn)
    {
        if (stateNo == 7 || stateNo == 8) return;
        foreach (Rule_Card card in cardList)
        {
            if (card.StateValue(20) > 0 && value > 0) continue;
            int prevValue = card.state[stateNo] + card.turnState[stateNo];

            if (!turn)
            {
                if (stateNo == 24 || stateNo == 27)
                {
                    card.state[stateNo] += value;
                }
                else if (stateNo == 16 || stateNo == 18)
                {
                    if (card.state[stateNo] == 0 && card.turnState[stateNo] == 0 && value > 0) card.state[stateNo] = value;
                    if (value == 0) card.state[stateNo] = value;
                }
                else
                {
                    card.state[stateNo] = value;
                }
            }
            else
            {
                if (stateNo == 24 || stateNo == 27)
                {
                    card.turnState[stateNo] += value;
                }
                else if (stateNo == 16 || stateNo == 18)
                {
                    if (card.state[stateNo] == 0 && card.turnState[stateNo] == 0 && value > 0) card.turnState[stateNo] = value;
                    if (value == 0) card.turnState[stateNo] = value;
                }
                else
                {
                    card.turnState[stateNo] = value;
                }
            }

            int afterValue = card.state[stateNo] + card.turnState[stateNo];

            if (prevValue != afterValue)
            {
                State state = new State
                {
                    order = card.order,
                    stateNo = stateNo,
                    value = afterValue
                };
                ProcessRequest("state", state.ToString());

                if (stateNo == 19)
                {
                    Event ev = new Event
                    {
                        type = "state",
                        targetCard = card.ToList(),
                        value = value,
                        state = stateNo
                    };
                    eventList.Add(ev);
                }
            }
        }
    }
    IEnumerator Act(List<Rule_Card> cardList, int value,bool skill,bool player)
    {
        if (value == 1) cardList.RemoveAll(c => c.act == true);
        else
        {
            cardList.RemoveAll(c => c.act == false);
            cardList.RemoveAll(c => c.StateValue(13) > 0 && c.player != player);
        }
        foreach (Rule_Card card in cardList)
        {
            bool prev = card.act;
            if (value == 1)card.act = true;
            else card.act = false;
            if (prev != card.act)
            {
                State state = new State
                {
                    order = card.order,
                    stateNo = -1,
                    value = value
                };
                ProcessRequest("state", state.ToString());
            }
        }
        Event ev = new Event()
        {
            type = "act",
            targetCard = cardList,
            skill = skill,
            player = player,
            value = value
        };
        eventList.Add(ev);
        yield return EventStart();
    }

    void ActNoStart(List<Rule_Card> cardList, int value, bool skill, bool player)
    {
        if (value == 1) cardList.RemoveAll(c => c.act == true);
        else
        {
            cardList.RemoveAll(c => c.act == false);
            cardList.RemoveAll(c => c.StateValue(13) > 0 && c.player != player);
        }
        foreach (Rule_Card card in cardList)
        {
            bool prev = card.act;
            if (value == 1) card.act = true;
            else card.act = false;
            if (prev != card.act)
            {
                State state = new State
                {
                    order = card.order,
                    stateNo = -1,
                    value = value
                };
                ProcessRequest("state", state.ToString());
            }
        }
        Event ev = new Event()
        {
            type = "act",
            targetCard = cardList,
            skill = skill,
            player = player,
            value = value
        };
        eventList.Add(ev);
    }
    IEnumerator End(bool player)
    {
        
        foreach (Rule_Card card in TargetList(player, "player", "field"))if (card.StateValue(15) > 0) yield return Act(card.ToList(), 1, false, false);

        yield return Power(TargetList(player,"both","field"), 99999, "damage",false,false);
        foreach(Rule_Card card in TargetList(player, "both", "field"))
        {
            yield return Power(card.ToList(), -card.powerChangeTurn, "turn",false, false);
        }
        for (int j = 0; j < 29; j++) yield return State(TargetList(player, "both", "field"), j, 0, true);

        Event ev = new Event
        {
            type = "end",
            player = player
        };
        eventList.Add(ev);
        yield return EventStart();

    }
    IEnumerator Life(bool player,int i)
    {
        Life life = new Life
        {
            player = player,
            change = i
        };
        int prevLife;
        int value;
        if (player)
        {
            prevLife = playerLife;
            playerLife += i;
            if (playerLife > 7) playerLife = 7;
            life.life = playerLife;
            value = playerLife - prevLife;
        }
        else
        {
            prevLife = enemyLife;
            enemyLife += i;
            if (enemyLife > 7) enemyLife = 7;
            life.life = enemyLife;
            value = enemyLife - prevLife;
        }
        ProcessRequest("life", life.ToString());
        if (value != 0)
        {
            Event ev = new Event
            {
                type = "life",
                player = player,
                value = value
            };
            eventList.Add(ev);
            yield return EventStart();
        }
        if (playerLife <= 0)
        {
            Finish finish = new Finish()
            {
                winner = false,
            };
            ProcessRequest("finish", finish.ToString());
        }
        if (enemyLife <= 0)
        {
            Finish finish = new Finish()
            {
                winner = true,
            };
            ProcessRequest("finish", finish.ToString());
        }
    }
    IEnumerator Power(List<Rule_Card> cardList, int change,string type,bool skill,bool player)
    {
        Debug.Log("powerChange"+change+",Count"+cardList.Count);
        if (change == 0) yield break;
        if (type == "damage" && skill) cardList.RemoveAll(c => c.StateValue(11) > 0 && c.player != player);
        int value=0;
        foreach (Rule_Card card in cardList)
        {
            int prevPower = card.power;
            if (type == "level")
            {
                card.power += change;
                value = change;
            }
            if (type == "power")
            {
                card.power += change;
                card.powerChange += change;
                value = change;
            }
            if (type == "turn")
            {
                card.power += change;
                card.powerChangeTurn += change;
                value = change;
            }
            if (type == "damage")
            {
                int damage = change;
                if (card.StateValue(22) > 0) damage = change*2;
                card.power += damage;
                card.damage += damage;
                
                if (card.damage > 0)
                {
                    card.power -= card.damage;
                    damage -= card.damage;
                    card.damage = 0;
                }
                value = damage;
            }

            if (card.power != prevPower)
            {
                Power power = new Power
                {
                    type = type,
                    now = card.power,
                    order = card.order,
                    change = value,
                    skill = skill
                };
                ProcessRequest("power", power.ToString());
            }
        }
        if (skill)
        {
            Event ev = new Event()
            {
                type = type,
                player = player,
                targetCard = cardList,
                value = value,
                skill = skill
            };
            if (ev.type == "type") ev.type = "power";
            eventList.Add(ev);
            yield return EventStart();
        }
    }
    void PowerNoStart(List<Rule_Card> cardList, int change, string type, bool skill, bool player)
    {
        Debug.Log("powerChange" + change + ",Count" + cardList.Count);
        if (change == 0) return;
        if (type == "damage" && skill) cardList.RemoveAll(c => c.StateValue(11) > 0 && c.player != player);
        int value = 0;
        foreach (Rule_Card card in cardList)
        {
            int prevPower = card.power;
            if (type == "level")
            {
                card.power += change;
                value = change;
            }
            if (type == "power")
            {
                card.power += change;
                card.powerChange += change;
                value = change;
            }
            if (type == "turn")
            {
                card.power += change;
                card.powerChangeTurn += change;
                value = change;
            }
            if (type == "damage")
            {
                int damage = change;
                if (card.StateValue(22) > 0) damage = change * 2;
                card.power += damage;
                card.damage += damage;

                if (card.damage > 0)
                {
                    card.power -= card.damage;
                    damage -= card.damage;
                    card.damage = 0;
                }
                value = damage;
            }

            if (card.power != prevPower)
            {
                Power power = new Power
                {
                    type = type,
                    now = card.power,
                    order = card.order,
                    change = value,
                    skill = skill
                };
                ProcessRequest("power", power.ToString());
            }
        }
        if (skill)
        {
            Event ev = new Event()
            {
                type = type,
                player = player,
                targetCard = cardList,
                value = value,
                skill = skill
            };
            if (ev.type == "type") ev.type = "power";
            eventList.Add(ev);
        }
    }
    IEnumerator Level(List<Rule_Card> cardList,  int change)
    {
        foreach (Rule_Card card in cardList)
        {
            int prevLevel = card.level;
            int prevPower=0;
            if (card.level == 1) prevPower = card.card.power1;
            if (card.level == 2) prevPower = card.card.power2;
            if (card.level == 3) prevPower = card.card.power3;
            card.level += change;
            if (card.level > 3) card.level = 3;
            if (card.level < 1) card.level = 1;
            if (card.level != prevLevel)
            {
                Level level = new Level
                {
                    order = card.order,
                    now = card.level,
                    change = change
                };
                ProcessRequest("level", level.ToString());
                Event ev = new Event()
                {
                    type = "level",
                    targetCard = card.ToList(),
                    value = card.level - prevLevel
                };
                eventList.Add(ev);

                if (card.level > prevLevel) PowerNoStart(card.ToList(), 99999, "damage",false,false);
                int afterPower = 0;

                if (card.level == 1) afterPower = card.card.power1;
                if (card.level == 2) afterPower = card.card.power2;
                if (card.level == 3) afterPower = card.card.power3;
                PowerNoStart(card.ToList(), afterPower - prevPower, "level", false, false);
                if (card.level > prevLevel && card.level == 3)
                {
                    ActNoStart(card.ToList(), 1, false, false);
                    StateNoStart(card.ToList(), 0, 0, true);
                }

            }
        }
        yield return EventStart();
    }
    IEnumerator Summon(Rule_Card card,bool skill)
    {
        if(card.card.type=="ユニット")yield return State(card.ToList(), 0, 1, true);

        int stateNum = StateToNum(card.card.skill1);
        if (stateNum == 16 || stateNum == 18 || stateNum == 24 || stateNum == 27)
        {
            yield return State(card.ToList(), stateNum, (int)Char.GetNumericValue(card.card.skill1[2]), false);
        }
        else if (stateNum != -1)
        {
            yield return State(card.ToList(), stateNum, 1, false);
        }
        stateNum = StateToNum(card.card.skill2);
        if (stateNum == 16 || stateNum == 18 || stateNum == 24 || stateNum == 27)
        {
            yield return State(card.ToList(), stateNum, (int)Char.GetNumericValue(card.card.skill2[2]), false);
        }
        else if (stateNum != -1)
        {
            yield return State(card.ToList(), stateNum, 1, false);
        }
        stateNum = StateToNum(card.card.skill3);
        if (stateNum == 16 || stateNum == 18 || stateNum == 24 || stateNum == 27)
        {
            yield return State(card.ToList(), stateNum, (int)Char.GetNumericValue(card.card.skill3[2]), false);
        }
        else if (stateNum != -1)
        {
            yield return State(card.ToList(), stateNum, 1, false);
        }
        CardEffect effect = new CardEffect()
        {
            order = card.order,
            skill = skill
        };
        ProcessRequest("summon", effect.ToString());
        ProcessRequest("wait", "");
        Event ev = new Event
        {
            type = "summon",
            player = card.player,
            skill = skill,
            targetCard = card.ToList()
        };
        eventList.Add(ev);
        yield return EventStart();
        if (card.level == 3)
        {
            ev = new Event
            {
                type = "level",
                player = card.player,
                targetCard = card.ToList()
            };
            eventList.Add(ev);
            if (card.card.type == "ユニット") StateNoStart(card.ToList(), 0, 0, true);
            ActNoStart(card.ToList(), 1, false, false);
            yield return EventStart();
        }
    }
    IEnumerator Battle(Rule_Card attack,Rule_Card dffence)
    {
        Event ev = new Event
        {
            type = "battle",
            player = attack.player,
            targetCard = new List<Rule_Card>() { attack, dffence }
        };
        eventList.Add(ev);
        yield return EventStart();


        List<Rule_Card> field = TargetList(attack.player, "both", "field");
        if (field.Exists(c => c.order == attack.order) && field.Exists(c => c.order == dffence.order))
        {
            Battle battle = new Battle()
            {
                attack = attack.order,
                defence = dffence.order,
            };
            ProcessRequest("battle", battle.ToString());

            int attackDamage = -attack.power;
            int dffenceDamage = -dffence.power;
            yield return Power(dffence.ToList(), attackDamage, "damage", false, false);
            yield return Power(attack.ToList(), dffenceDamage, "damage", false, false);
            if (attack.power > 0 && dffence.power > 0)
            {

            }
            else if (attack.power > 0 && dffence.power <= 0)
            {
                yield return Death(dffence.ToList(), false,false,true);
                if (attack.StateValue(10) > 0) yield return Life(!attack.player, -1);
                yield return Level(attack.ToList(), 1);
            }
            else if (attack.power <= 0 && dffence.power > 0)
            {
                yield return Death(attack.ToList(), false, false, true);
                yield return Level(dffence.ToList(), 1);
            }
            if (attack.power <= 0 && dffence.power <= 0)
            {
                yield return Death(new List<Rule_Card>() { attack, dffence }, false, false, true);
            }
        }
    }

    IEnumerator Death(List<Rule_Card> cardList, bool player)
    {
        yield return Death(cardList, true, player, false);
    }
    IEnumerator Death(List<Rule_Card> cardList,bool skill, bool player,bool power)
    {
        if (skill) cardList.RemoveAll(c => c.StateValue(6) > 0 && c.power > 0 && c.player!=player);
        if (cardList.Count == 0) yield break;

        foreach (Rule_Card card in cardList)
        {
            CardEffect effect = new CardEffect()
            {
                order = card.order,
                skill = power
            };
            ProcessRequest("death", effect.ToString());
        }
        ProcessRequest("wait", "");

        List<Rule_Card> death = cardList.FindAll(c => c.StateValue(21) == 0);
        List<Rule_Card> sacrifice = cardList.FindAll(c => c.StateValue(21) > 0);
        Move(death, "field", "cemetary");
        Move(sacrifice, "field", "equip");
        Event ev = new Event()
        {
            type = "death",
            targetCard = cardList,
            skill = skill
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    IEnumerator Vanish(List<Rule_Card> cardList, bool player)
    {
        cardList.RemoveAll(c => c.StateValue(5) > 0 && c.player != player);
        if (cardList.Count == 0) yield break;
        foreach (Rule_Card card in cardList)
        {
            CardEffect effect = new CardEffect()
            {
                order = card.order
            };
            ProcessRequest("vanish", effect.ToString());
        }
        ProcessRequest("wait", "");

        yield return State(cardList, 23, 1, false);
        Move(cardList, "field", "cemetary");
        Event ev = new Event()
        {
            type = "panish",
            targetCard = cardList,
            player = player
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    IEnumerator Bounce(List<Rule_Card> cardList,bool player)
    {
        cardList.RemoveAll(c => c.StateValue(12) > 0 && c.player != player);
        if (cardList.Count == 0) yield break;
        foreach (Rule_Card card in cardList)
        {
            CardEffect effect = new CardEffect()
            {
                order = card.order
            };
            ProcessRequest("bounce", effect.ToString());
        }
        ProcessRequest("wait", "");
        Move(cardList, "field", "hand");
        Event ev = new Event()
        {
            type = "bounce",
            targetCard = cardList,
            player = player
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    IEnumerator EquipDeath(List<Rule_Card> cardList, bool player)
    {
        foreach (Rule_Card card in cardList)
        {
            CardEffect effect = new CardEffect()
            {
                order = card.order
            };
            ProcessRequest("equipdeath", effect.ToString());
        }
        ProcessRequest("wait", "");
        Move(cardList.ToList(), "equip", "cemetary");
        Event ev = new Event()
        {
            type = "equipdeath",
            player = player,
            targetCard = cardList,
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    IEnumerator HandDeath(List<Rule_Card> cardList, bool player)
    {
        foreach (Rule_Card card in cardList)
        {
            CardEffect effect = new CardEffect()
            {
                order = card.order
            };
            ProcessRequest("handdeath", effect.ToString());
        }
        ProcessRequest("wait", "");
        Move(cardList.ToList(), "hand", "cemetary");
        Event ev = new Event()
        {
            type = "handdeath",
            player = player,
            targetCard = cardList,
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    IEnumerator Magic(Rule_Card card)
    {
        List<Rule_Card> list0;
        Rule_Card card0;
        Select select;
        switch (card.card.title)
        {
            case "手招き":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return State(card0.ToList(), 3, 1, false);
                }
                break;
            case "メテオ":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    if (card.player) yield return Power(card0.ToList(), -1000 * (7 - enemyLife), "damage", true, card.player);
                    else yield return Power(card0.ToList(), -1000 * (7 - playerLife), "damage", true, card.player);
                }
                break;
            case "ファイアウォール":
                SkillDisplay(card.order, 0, true);
                list0 = TargetList(card.player, "enemy", "field");
                if (list0.Count > 0)
                {
                    yield return Power(list0, -6000 / list0.Count, "damage", true, card.player);
                }
                break;
            case "ハイテンション":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "player");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Level(card0.ToList(), 1);
                }
                break;
            case "リラックス":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "both");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Level(card0.ToList(), -1);
                }
                break;
            case "閃光":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Act(card0.ToList(), 0, true, card.player);
                }
                break;
            case "ライトニング":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                list0.RemoveAll(c => c.act);
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Power(card0.ToList(), -5000, "damage", true, card.player);
                }
                break;
            case "リサイクル":
                SkillDisplay(card.order, 0, true);
                list0 = TargetList(card.player, "player", "equip");
                if (list0.Count > 0)
                {
                    Move(list0, "equip", "cemetary");
                    foreach (Rule_Card c in list0) Draw(card.player, c.isLeft, 1);
                }
                break;
            case "サイドアシスト":
                SkillDisplay(card.order, 0, true);
                Draw(card.player, !card.isLeft, 1);
                break;
            case "ダブルサイド":
                SkillDisplay(card.order, 0, true);
                Draw(card.player, 1, 1);
                break;
        }
        SkillDisplay(card.order, 0, false);
    }
    bool Select(bool player,List<Rule_Card> cardList, bool cancel,string message)
    {
        if (cardList.Count == 0)
        {
            return false;
        }
        else
        {
            Select select = new Select
            {
                player = player,
                cancel = cancel,
                message = message
            };
            List<int> orderList = new List<int>();
            foreach (Rule_Card card in cardList) orderList.Add(card.order);
            select.orderList = orderList.ToArray();
            ProcessRequest("select", select.ToString());
            return true;
        }
    }
    IEnumerator Side(bool player)
    {
        string charaL;
        string charaR;
        if (player)
        {
            charaL = playerCharaL;
            charaR = playerCharaR;
        }
        else
        {
            charaL = enemyCharaL;
            charaR = enemyCharaR;
        }
        Side side = new Side
        {
            player = player,
            request = true,
            leftChara = charaL,
            rightChara = charaR
        };
        ProcessRequest("side", side.ToString());
        yield return AnswerWait(player,"side");
        side = BattleJson.Side.ToSide(content);
        side.request = false;
        side.leftChara = charaL;
        side.rightChara = charaR;
        ProcessRequest("side", side.ToString());

        Event ev = new Event
        {
            type = "side",
            player = player
        };
        eventList.Add(ev);
        yield return EventStart();
    }
    void Move(List<Rule_Card> cardList, string prev,string after)
    {
        List<int> orderList = new List<int>();
        foreach (Rule_Card card in cardList) {
            List<Rule_Card> prevArea = null;
            switch (prev)
            {
                case "hand":
                    if (card.player)
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
                    if (card.player)
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
                    if (card.player)
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
                    if (card.player)
                    {
                        prevArea = playerField;
                    }
                    else
                    {
                        prevArea = enemyField;
                    }
                    break;
                case "equip":
                    if (card.player)
                    {
                        prevArea = playerEquip;
                    }
                    else
                    {
                        prevArea = enemyEquip;
                    }
                    break;
            }
            List<Rule_Card> afterArea = null;
            switch (after)
            {
                case "hand":
                    if (card.player)
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
                    if (card.player)
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
                    if (card.player)
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
                    if (card.player)
                    {
                        afterArea = playerField;
                    }
                    else
                    {
                        afterArea = enemyField;
                    }
                    break;
                case "equip":
                    if (card.player)
                    {
                        afterArea = playerEquip;
                    }
                    else
                    {
                        afterArea = enemyEquip;
                    }
                    break;
            }
            if (after == "equip" && afterArea.Count == 4)
            {
                Move(afterArea[0].ToList(), "equip", "cemetary");
            }
            if (after == "hand" && afterArea.Count == 6) continue;

            orderList.Add(card.order);
            prevArea.Remove(card);
            afterArea.Add(card);
            if (card.StateValue(19) > 0) card.prevSilence = true;
            else card.prevSilence = false;
            if (prev == "hand" && after == "field")
            {

            }
            else
            {
                int panish = 0;
                if (after == "cemetary") panish = card.state[23];
                card.ReSet();
                if (after == "cemetary") card.state[23] = panish;
            }
        }
        Move move = new Move
        {
            order = orderList.ToArray(),
            prev = prev,
            after = after
        };
        ProcessRequest("move",move.ToString());
    }


    IEnumerator EventStart()
    {
        if (eventProcess) yield break;
        else
        {
            eventProcess = true;
        }
        while (eventList.Count > 0)
        {
            Debug.Log(eventNo+"---EventStart---" + eventList[0].type+"/Player"+ eventList[0].player);
            int eventNow = eventNo;
            eventNo++;
            Event ev = eventList[0].ShallowCopy();
            if (eventList.Count > 0) eventList.RemoveAt(0);
            switch (ev.type)
            {
                case "side":
                    yield return SideEvent(ev);
                    break;
                case "summon":
                    yield return SummonEvent(ev);
                    break;
                //case "life":
                    //yield return LifeEvent(ev);
                    //break;
                //case "state":
                    //yield return StateEvent(ev);
                    //break;
                case "attack":
                    yield return AttackEvent(ev);
                    break;
                case "dffence":
                    yield return DffenceEvent(ev);
                    break;
                case "battle":
                    yield return BattleEvent(ev);
                    break;
                case "break":
                    yield return BreakEvent(ev);
                    break;
                case "death":
                    yield return DeathEvent(ev);
                    break;
                case "equipdeath":
                    yield return EquipDeathEvent(ev);
                    break;
                case "level":
                    yield return LevelEvent(ev);
                    break;
                case "item":
                    yield return ItemEvent(ev);
                    break;
                case "end":
                    yield return EndEvent(ev);
                    break;
            }
            yield return AllEvent(ev);
            yield return EquipEvent(ev);
            Debug.Log(eventNow+"---EventEnd---" + ev.type + "/Player" + ev.player);
        }
        eventProcess = false;
        yield return EventEnd();
    }
    IEnumerator ItemEventStart()
    {
        while (eventList.Count > 0)
        {
            Debug.Log(eventNo + "---EventStart---" + eventList[0].type + "/Player" + eventList[0].player);
            int eventNow = eventNo;
            eventNo++;
            Event ev = eventList[0].ShallowCopy();
            if (eventList.Count > 0) eventList.RemoveAt(0);
            switch (ev.type)
            {
                case "side":
                    yield return SideEvent(ev);
                    break;
                case "summon":
                    yield return SummonEvent(ev);
                    break;
                //case "life":
                    //yield return LifeEvent(ev);
                    //break;
                //case "state":
                    //yield return StateEvent(ev);
                    //break;
                case "attack":
                    yield return AttackEvent(ev);
                    break;
                case "dffence":
                    yield return DffenceEvent(ev);
                    break;
                case "battle":
                    yield return BattleEvent(ev);
                    break;
                case "break":
                    yield return BreakEvent(ev);
                    break;
                case "death":
                    yield return DeathEvent(ev);
                    break;
                case "equipdeath":
                    yield return EquipDeathEvent(ev);
                    break;
                case "level":
                    yield return LevelEvent(ev);
                    break;
                case "item":
                    yield return ItemEvent(ev);
                    break;
                case "end":
                    yield return EndEvent(ev);
                    break;
            }
            yield return AllEvent(ev);
            yield return EquipEvent(ev);
            Debug.Log(eventNow+"---EventEnd---" + ev.type + "/Player" + ev.player);
        }
        //yield return EventEnd();
    }
    IEnumerator SideEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        Select select;
        List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
        foreach (Rule_Card card in field)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "タヌキ":
                    if (card.player != ev.player)
                    {
                        SkillDisplay(card.order, 0, true);
                        yield return Power(TargetList(card.player, "player", "field"),1000,"turn",true,card.player);
                    }
                    break;
                case "ロウソク":
                    if (card.player == ev.player)
                    {
                        SkillDisplay(card.order, 0, true);
                        yield return Death(card.ToList(), card.player);
                    }
                    break;
                case "エクレア":
                    if (card.player == ev.player)
                    {
                        SkillDisplay(card.order, 1, true);
                        list0 = TargetList(card.player, "enemy", "hand");
                        if (list0.Count >= 7)
                        {
                            yield return Level(card.ToList(), 1);
                        }
                    }
                    break;
                case "天使・クルル":
                    if (card.player != ev.player)
                    {
                        SkillDisplay(card.order, 1, true);
                        list0 = TargetList(card.player, "enemy", "hand");
                        if (list0.Count >= 8)
                        {
                            list0 = TargetList(card.player, "both", "field");
                            if (Select(card.player, list0, false, "効果の対象を選んでください"))
                            {
                                yield return AnswerWait(card.player, "select");
                                select = BattleJson.Select.ToSelect(content);
                                card0 = allCard.Find(c => c.order == select.orderList[0]);
                                yield return Level(card0.ToList(), -1);
                            }
                        }
                    }
                    break;
            }
        }
    }
    IEnumerator SummonEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        List<Rule_Card> list1;
        Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (!ev.skill)
            {
                switch (card.card.title)
                {
                    case "緑ドラ卵":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, !card.isLeft);
                        card0 = list0.Find(c => c.card.color == "緑");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "赤ドラ卵":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, !card.isLeft);
                        card0 = list0.Find(c => c.card.color == "赤");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "黄ドラ卵":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, !card.isLeft);
                        card0 = list0.Find(c => c.card.color == "黄");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "黒ドラ卵":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, !card.isLeft);
                        card0 = list0.Find(c => c.card.color == "黒");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "森のリザード":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.color == "緑");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "火のリザード":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.color == "赤");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "痺れリザード":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.color == "黄");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "毒のリザード":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.color == "黒");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "グリーンペンペン":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.type == "ユニット");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "レッドペンペン":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.type == "アイテム");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "イエローペンペン":
                        SkillDisplay(card.order, 0, true);
                        list0 = FindDeck(card.player, card.isLeft);
                        card0 = list0.Find(c => c.card.type == "マジック");
                        if (card0 != null) Draw(card0.ToList());
                        break;
                    case "エリーチカ":
                        SkillDisplay(card.order, 0, true);
                        NextManaAdd(card.player, 1);
                        break;
                    case "ギーク":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return State(card0.ToList(), 4, 1, false);
                        }
                        break;
                    case "ウルフギャング":
                        SkillDisplay(card.order, 0, true);
                        ManaAdd(card.player, 2);
                        break;
                    case "ピーター":
                        SkillDisplay(card.order, 0, true);
                        yield return Power(TargetList(card.player, "player", "field"),card.level*1000,"power",true,card.player);
                        break;
                    case "シルフィード":
                        SkillDisplay(card.order, 0, true);
                        Draw(card.player, card.isLeft, 2);
                        break;
                    case "アントワネット":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "both", "field");
                        list0.RemoveAll(c => c.power > 5000);
                        yield return State(list0, 3, 1, false);
                        break;
                    case "アイドル・カケル":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        list0.RemoveAll(c => c.power >= card.power);
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            //if (TargetList(card.player,"enemy","equip").Count == 4) Move(TargetList(card.player, "enemy", "equip")[0].ToList(), "equip", "cemetary");
                            Move(card0.ToList(), "field", "equip");
                        }
                        break;
                    case "ゴート":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "equip");
                        if (list0.Count > 0)
                        {
                            yield return EquipDeath(list0[0].ToList(), card.player);
                        }
                        break;
                    case "フレア":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        for(int i = 0; i < card.level; i++)
                        {
                            list0.RemoveAll(c => c.damage > 0);
                            if (list0.Count > 0) yield return Power(list0[0].ToList(), -3000, "damage", true, card.player);
                        }
                        break;
                    case "サラ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Power(card0.ToList(), -3000, "damage", true, card.player);
                            if (card0.power < 3000) Draw(card.player, card.isLeft, 1);
                        }
                        break;
                    case "ロングボウ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "equip");
                        if (list0.Count == 0)
                        {
                            yield return Level(card.ToList(), 1);
                        }
                        break;
                    case "サタン":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        if (list0.Count > 0)
                        {
                            yield return Power(list0, -9000 / list0.Count, "damage", true, card.player);
                        }
                        break;
                    case "エル":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "both", "field");
                        list0.RemoveAll(c => c == card);
                        if (list0.Count > 0)
                        {
                            yield return State(list0, 22, 1, false);
                        }
                        break;
                    case "兄貴分・ゴート":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "equip");
                        if (list0.Count > 0)
                        {
                            yield return EquipDeath(list0[0].ToList(), card.player);
                        }
                        break;
                    case "ショコラ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Act(card0.ToList(), 0, true, card.player);

                        }
                        break;
                    case "プリテス":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "player");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Act(card0.ToList(), 1, true, card.player);
                        }
                        break;
                    case "お菓子作り・ショコラ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        for (int i = 0; i < card.level; i++)
                        {
                            list0.RemoveAll(c => c.act ==false);
                            if (list0.Count > 0) yield return Act(list0[0].ToList(), 0,  true, card.player);
                        }
                        break;
                    case "料理人・マイケル":
                        SkillDisplay(card.order, 0, true);
                        Draw(card.player, 1, 1);
                        Draw(!card.player, 1, 1);
                        break;
                    case "ウッディ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        list0.RemoveAll(c => c.card.cost > card.level);
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Vanish(card0.ToList(), card.player);
                        }
                        break;
                    case "バステト":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        list0.RemoveAll(c => c.card.cost > card.level + 3);
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Bounce(card0.ToList(), card.player);
                        }
                        break;
                    case "エクレア":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "hand");
                        if (list0.Count >= 7)
                        {
                            yield return Level(card.ToList(), 1);
                        }
                        break;
                    case "気合・まい":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "player");
                        list0.RemoveAll(c => c.order == card.order);
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Bounce(card0.ToList(), card.player);
                            ManaAdd(card.player, 2);
                        }
                        break;
                    case "天使・クルル":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "both", "field");
                        list0.RemoveAll(c => c.card.cost > 3);
                        if (list0.Count > 0)
                        {
                            yield return Bounce(list0, card.player);
                        }
                        break;
                    case "月光・ネココ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        list0.RemoveAll(c => c.state[28] + c.turnState[28]==0);
                        if (list0.Count > 0)
                        {
                            yield return Vanish(list0, card.player);
                        }
                        SkillDisplay(card.order, 1, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        list0.RemoveAll(c => c.act);
                        if (list0.Count > 0)
                        {
                            yield return Bounce(list0, card.player);
                        }
                        break;
                    case "ネココ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        list0.RemoveAll(c => c.act);
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Bounce(card0.ToList(), card.player);
                        }
                        break;
                    case "秋・バステト":
                        SkillDisplay(card.order, 0, true);
                        yield return Level(card.ToList(), 2);
                        break;
                    case "星・サリエル":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        list0.RemoveAll(c => c.level > 1);
                        if (list0.Count > 0) yield return State(list0, 28, 1, false);
                        break;
                    case "ヤミ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        for (int i = 0; i < card.level; i++)
                        {
                            list0.RemoveAll(c => c.StateValue(19) > 0);
                            if (list0.Count > 0) yield return State(list0[0].ToList(), 19, 1, false);
                        }
                        break;
                    case "カタリナ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "player");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Death(card0.ToList(), card.player);
                        }
                        break;
                    case "アンコ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                        if (list0.Count >= 1) Move(list0[0].ToList(), "cemetary", "equip");
                        if (list0.Count >= 2) Move(list0[1].ToList(), "cemetary", "equip");
                        break;
                    case "パピヨン":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "both");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            if (card0.level == 3) yield return Death(card0.ToList(), card.player);
                            else yield return Level(card0.ToList(), 1);
                        }
                        break;
                    case "メタルナックル":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        int max = list0.Max(c => c.level);
                        if (max >= 2)
                        {
                            list0 = TargetList(card.player, "enemy", "hand");
                            if (list0.Count > 0) yield return HandDeath(list0[0].ToList(), card.player);
                        }
                        if(max >= 3)
                        {
                            list0 = TargetList(card.player, "enemy", "hand");
                            if (list0.Count > 0) yield return HandDeath(list0[0].ToList(), card.player);
                        }
                        break;
                    case "魔女・ヤミ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        list0.RemoveAll(c => c.StateValue(19) == 0);
                        if (list0.Count > 0)
                        {
                            list1 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                            if (list1.Count > 0) Move(list1[0].ToList(), "cemetary", "hand");
                        }
                        if (list0.Count > 1)
                        {
                            list1 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                            if (list1.Count > 0) Move(list1[0].ToList(), "cemetary", "hand");
                        }
                        break;
                    case "チョコ職人・マーブル":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetListSide(card.player, "player", "deck", true);
                        card0 = list0.Find(c => c.card.type == "アイテム");
                        if (card0 != null)
                        {
                            Move(card0.ToList(), "deck", "equip");
                        }
                        list0 = TargetListSide(card.player, "player", "deck", false);
                        card0 = list0.Find(c => c.card.type == "アイテム");
                        if (card0 != null)
                        {
                            Move(card0.ToList(), "deck", "equip");
                        }
                        break;
                    case "必殺・カタリナ":
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "enemy");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Death(card0.ToList(), card.player);
                        }
                        break;
                    case "スライムゾンビ":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                        card0 = list0.Find(c => c.card.type == "ユニット" && c.card.cost == 2);
                        if (card0 != null && TargetList(card.player, "player", "field").Count != 5)
                        {
                            Move(card0.ToList(), "cemetary", "field");
                            yield return Summon(card0, true);
                        }
                        break;
                    case "中級・エリゴス":
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        list0.RemoveAll(c => c.level == 1);
                        yield return Death(list0, card.player);
                        break;
                }
                List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
                foreach (Rule_Card other in field)
                {
                    if (other.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == other)) continue;
                    switch (other.card.title)
                    {
                        case "コドラコ":
                            if (card.player != other.player && TargetList(first && firstPlayer, "both", "field").Exists(c => c.order == card.order))
                            {
                                SkillDisplay(other.order, 0, true);
                                yield return Power(card.ToList(), -1000, "power", true, card.player);
                            }
                            break;
                    }
                }
            }
        }
    }
    //void LifeEvent(Event ev)
    //{
    //    List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
    //    foreach (Rule_Card card in field)
    //    {
    //        if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
    //        switch (card.card.title)
    //        {
    //        }
    //    }
    //}
    //void StateEvent(Event ev)
    //{
    //    foreach (Rule_Card card in ev.targetCard)
    //    {
    //        switch (card.card.title)
    //        {
    //        }
    //    }
    //}
    IEnumerator BreakEvent(Event ev)
    {
        List<Rule_Card> list0;
        Rule_Card card0;
        Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "フレイヤ":
                    SkillDisplay(card.order, 0, true);
                    ManaAdd(card.player, 1);
                    break;
                case "レーナ":
                    SkillDisplay(card.order, 0, true);
                    yield return Level(card.ToList(), 1);
                    break;
                case "マリ":
                    SkillDisplay(card.order, 0, true);
                    list0 = SelectField(card.player, "enemy");
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return State(card0.ToList(), 28, 1, false);
                    }
                    break;
            }
        }
    }
    IEnumerator AttackEvent(Event ev)
    {
        List<Rule_Card> list0;
        Rule_Card card0;
        Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "ジョン":
                    SkillDisplay(card.order, 0, true);
                    yield return  Power(card.ToList(), 1000, "power", true, card.player);
                    break;
                case "突撃ゴブリン":
                    SkillDisplay(card.order, 0, true);
                    yield return  Power(card.ToList(), 1000*card.level, "turn", true, card.player);
                    break;
                case "ショートボウ":
                    SkillDisplay(card.order, 0, true);
                    list0 = SelectField(card.player, "enemy");
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return  Power(card0.ToList(), -1000 * card.level, "damage", true, card.player);
                    }
                    break;
                case "トモシビ":
                    SkillDisplay(card.order, 0, true);
                    list0 = SelectField(card.player, "enemy");
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return  Power(new List<Rule_Card>() {card,card0 }, -card.power, "damage", true, card.player);
                    }
                    break;
                case "レウス":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy","field");
                    yield return  Power(list0, -1000 * card.level, "damage", true, card.player);
                    break;
                case "特攻ゴブリン":
                    SkillDisplay(card.order, 0, true);
                    yield return  Power(card.ToList(), 2000 * card.level, "turn", true, card.player);
                    break;
                case "ロングボウ":
                    SkillDisplay(card.order, 1, true);
                    list0 = SelectField(card.player, "enemy");
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return  Power(card0.ToList(), -2000 * card.level, "damage", true, card.player);
                    }
                    break;
                case "サタン":
                    SkillDisplay(card.order, 1, true);
                    if(card.player) yield return Power(card.ToList(), 1000 * (7-enemyLife), "turn", true, card.player);
                    else yield return Power(card.ToList(), 1000 * (7 - playerLife), "turn", true, card.player);
                    break;
                case "攻め時・リコ":
                    SkillDisplay(card.order, 1, true);
                    yield return Power(TargetList(card.player, "player", "field"), 3000, "turn", true, card.player);
                    break;
                case "怪盗・カリン":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy", "field");
                    list0.RemoveAll(c=>c.power<card.power);
                    yield return State(list0, 2, 1, false);
                    break;
                case "ネココ":
                    SkillDisplay(card.order, 1, true);
                    list0 = TargetList(card.player, "enemy", "hand");
                    if (list0.Count >= 7) yield return State(card.ToList(), 16, 1, true);
                    break;
                case "星・サリエル":
                    SkillDisplay(card.order, 1, true);
                    list0 = TargetList(card.player, "enemy", "hand");
                    if (list0.Count >= 7)
                    {
                        list0 = SelectField(card.player, "enemy");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Act(card0.ToList(), 0, true, card.player);
                        }
                    }
                    break;
            }
            List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
            foreach (Rule_Card other in field)
            {
                if (other.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
                switch (other.card.title)
                {
                    case "攻め時・リコ":
                        if (card.player == other.player && card.state[14] + card.turnState[14]>0)
                        {
                            SkillDisplay(other.order, 1, true);
                            list0 = SelectField(card.player, "enemy");
                            if (Select(card.player, list0, false, "効果の対象を選んでください"))
                            {
                                yield return AnswerWait(card.player, "select");
                                select = BattleJson.Select.ToSelect(content);
                                card0 = allCard.Find(c => c.order == select.orderList[0]);
                                yield return Power(card0.ToList(), -2000, "damage", true, card.player);
                            }
                        }
                        break;
                }
            }
        }
    }
    IEnumerator DffenceEvent(Event ev)
    {
        //List<Rule_Card> list0;
        //Rule_Card card0;
        //Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "マリア":
                    SkillDisplay(card.order, 0, true);
                    yield return Power(card.ToList(),1000,"turn",true,card.player);
                    break;
                case "キティ":
                    SkillDisplay(card.order, 1, true);
                    yield return Act(card.ToList(), 0, true, card.player);
                    break;
            }
            List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
            foreach (Rule_Card other in field)
            {
                if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
                switch (other.card.title)
                {
                    case "小隊長・マリア":
                        if (card.player == other.player)
                        {
                            SkillDisplay(other.order, 0, true);
                            yield return Power(card.ToList(), 1000, "turn", true, card.player);
                        }
                        break;
                }
            }
        }
    }
    IEnumerator BattleEvent(Event ev)
    {
        List<Rule_Card> list0;
        Rule_Card card0;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "キティ":
                    SkillDisplay(card.order, 0, true);
                    yield return Power(card.ToList(), 2000, "turn", true, card.player);
                    break;
                    /*
                case "兄貴分・ゴート":
                    card0 = ev.targetCard.Find(c => c.player != card.player);
                    if (card0.powerChange + card0.powerChangeTurn + card0.damage < 0)
                    {
                        SkillDisplay(card.order, 2, true);
                        yield return Death(card0.ToList(), card.player);
                    }
                    break;
                    */

                case "怪盗・カリン":
                    SkillDisplay(card.order, 1, true);
                    list0 = TargetList(card.player, "enemy", "equip");
                    if (list0.Count > 0) yield return EquipDeath(list0[0].ToList(), card.player);
                    break;
                case "キヨコ":
                    SkillDisplay(card.order, 0, true);
                    card0 = ev.targetCard.Find(c => c.player != card.player);
                    if(card0.level>1)yield return Power(card0.ToList(), -3000, "power", true, card.player);
                    break;
                case "リア":
                    SkillDisplay(card.order, 0, true);
                    card0 = ev.targetCard.Find(c => c.player != card.player);
                    if (card0.StateValue(19) > 0)
                    {
                        yield return Death(card0.ToList(), card.player);
                        yield return State(card.ToList(), 19, 1, false);
                    }
                    break;
            }
            List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
            foreach (Rule_Card other in field)
            {
                if (other.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
                switch (other.card.title)
                {
                    case "ヒーロー・ニナ":
                        if (card.player == other.player && card != other)
                        {
                            SkillDisplay(other.order, 0, true);
                            yield return Level(other.ToList(), 1);
                        }
                        break;
                }
            }
            
        }
    }
    IEnumerator DeathEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.prevSilence) continue;
            switch (card.card.title)
            {
                case "ブラックペンペン":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy", "hand");
                    if (list0.Count > 0) yield return HandDeath(list0[0].ToList(), card.player);
                    break;
                case "ロウソク":
                    SkillDisplay(card.order, 1, true);
                    list0 = TargetList(card.player, "both","field");
                    yield return Power(list0, -5000, "damage", true, card.player);
                    break;
                case "スカル":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetListSide(card.player,"player","cemetary",card.isLeft);
                    card0 = list0.Find(c => c.card.type == "ユニット" || c.card.type == "進化ユニット");
                    if (card0!=null)Move(card0.ToList(), "cemetary", "hand");
                    break;
                case "シェード":
                    if (!ev.skill) break;
                    SkillDisplay(card.order, 0, true);
                    list0 = SelectField(card.player, "enemy");
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return Death(card0.ToList(), card.player);
                    }
                    break;
                case "エリゴス":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy","field");
                    list0.RemoveAll(c => c.level == 1);
                    yield return Death(list0,  card.player);
                    break;
                case "シャドウ":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy", "field");
                    yield return State(list0, 19,1, false);
                    break;
                case "黒魔法・ヤミ":
                    SkillDisplay(card.order, 0, true);
                    list0 = TargetList(card.player, "enemy", "field");
                    list0.RemoveAll(c => c.StateValue(19) > 0);
                    if (list0.Count > 0) yield return State(list0[0].ToList(), 19, 1, false);
                    break;
                case "魔女・ヤミ":
                    SkillDisplay(card.order, 1, true);
                    list0 = SelectField(card.player, "enemy");
                    list0.RemoveAll(c => c.StateValue(19) == 0);
                    if (Select(card.player, list0, false, "効果の対象を選んでください"))
                    {
                        yield return AnswerWait(card.player, "select");
                        select = BattleJson.Select.ToSelect(content);
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return Death(card0.ToList(),  card.player);
                    }
                    break;
            }
            List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
            foreach (Rule_Card other in field)
            {
                if (other.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == other)) continue;
                switch (other.card.title)
                {
                    case "黒魔法・ヤミ":
                        if (card.player == other.player)
                        {
                            SkillDisplay(other.order, 0, true);
                            list0 = TargetList(card.player, "enemy","field");
                            list0.RemoveAll(c => c.StateValue(19) > 0);
                            if(list0.Count>0)yield return State(list0[0].ToList(), 19,1,false);
                        }
                        break;
                }
            }
        }
    }
    IEnumerator LevelEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        List<Rule_Card> list1;
        List<Rule_Card> list2;
        Select select;
        foreach (Rule_Card card in ev.targetCard)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "キティ":
                    if (card.level == 3)
                    {
                        SkillDisplay(card.order, 2, true);
                        yield return Act(card.ToList(), 0, true, card.player);
                    }
                    break;
                case "ヒーロー・ニナ":
                    if (card.level == 3)
                    {
                        SkillDisplay(card.order, 1, true);
                        yield return State(card.ToList(), 10, 1, false);
                        yield return Level(card.ToList(), -2);
                    }
                    break;
                case "ホノカ":
                    if (card.level == 3)
                    {
                        SkillDisplay(card.order, 1, true);
                        yield return Life(!card.player,-1);
                    }
                    break;
                case "マリア":
                    if (ev.value < 0)
                    {
                        SkillDisplay(card.order, 1, true);
                        Draw(card.player,card.isLeft,1);
                    }
                    break;
                case "小隊長・マリア":
                    if (ev.value < 0)
                    {
                        SkillDisplay(card.order, 1, true);
                        Draw(card.player, card.isLeft, 1);
                    }
                    break;
                case "エクレア":
                    if (ev.value < 0)
                    {
                        SkillDisplay(card.order,2, true);
                        list0 = SelectField(card.player, "both");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            if (card.player == card0.player) yield return Act(card0.ToList(), 1, true, card.player);
                            else yield return Act(card0.ToList(), 0, true, card.player);
                        }
                    }
                    break;
                case "秋・バステト":
                    if (ev.value < 0)
                    {
                        SkillDisplay(card.order, 1, true);
                        list0 = TargetList(card.player, "enemy", "field");
                        if (list0.Count > 0) yield return Bounce(list0[0].ToList(), card.player);
                    }
                    break;
                case "フランケン":
                    if (card.level == 3)
                    {
                        SkillDisplay(card.order, 0, true);
                        list0 = TargetListSide(card.player, "enemy", "hand", true);
                        list1 = TargetListSide(card.player, "enemy", "hand", false);
                        list2 = new List<Rule_Card>();
                        if (list0.Count > 0) list2.Add(list0[0]);
                        if (list1.Count > 0) list2.Add(list1[0]);
                        yield return HandDeath(list2, card.player);
                    }
                    break;
            }
        }
    }
    IEnumerator EquipDeathEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        foreach (Rule_Card card in ev.targetCard)
        {
            List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
            foreach (Rule_Card other in field)
            {
                if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == other)) continue;
                switch (other.card.title)
                {
                    case "兄貴分・ゴート":
                        if (card.player!=other.player)
                        {
                            SkillDisplay(other.order, 1, true);
                            list0 = TargetList(other.player, "enemy","field");
                            card0 = list0.Find(c => c.power == list0.Max(c => c.power));
                            if(card0!=null)yield return Power(card0.ToList(), -2000, "damage",true, other.player);
                        }
                        break;
                }
            }
        }
    }
    IEnumerator ItemEvent(Event ev)
    {
        //Rule_Card card0;
        List<Rule_Card> list0;
        //Select select;
        List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
        foreach (Rule_Card card in field)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "チョコ職人・マーブル":
                    SkillDisplay(card.order, 1, true);
                    list0 = TargetList(card.player, "enemy", "field");
                    if (list0.Count > 0) yield return Power(list0[0].ToList(), -1000, "power", true, card.player);
                    yield return Power(card.ToList(), 1000, "power", true, card.player);
                    break;
            }
        }
    }
    IEnumerator AllEvent(Event ev)
    {
        //Rule_Card card0;
        //List<Rule_Card> list0;
        //List<Rule_Card> list1;
        //Select select;
        List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
        foreach (Rule_Card card in field){
        if (!TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "ゴブリン":
                    if (card.flag == null)
                    {
                        card.flag = new List<int>() { 0 };
                    }
                    if (card.StateValue(19) == 0)
                    {
                        int life;
                        if (card.player) life = playerLife;
                        else life = enemyLife;
                        int change = life * 1000 - card.flag[0] * 1000;
                        card.flag[0] = life;
                        if(change!=0)yield return Power(card.ToList(), change, "power", true, card.player);
                    }
                    else
                    {
                        int life=0;
                        int change = life * 1000 - card.flag[0] * 1000;
                        card.flag[0] = life;
                        if (change != 0) yield return Power(card.ToList(), change, "power", true, card.player);
                    }
                    break;
            }
        }
    }
    IEnumerator EndEvent(Event ev)
    {
        Rule_Card card0;
        List<Rule_Card> list0;
        Select select;
        List<Rule_Card> field = TargetList(first && firstPlayer, "both", "field");
        foreach (Rule_Card card in field)
        {
            if (card.StateValue(19) != 0 || !TargetList(first && firstPlayer, "both", "field").Exists(c => c == card)) continue;
            switch (card.card.title)
            {
                case "エリザベス":
                    if (card.player == ev.player)
                    {
                        SkillDisplay(card.order, 0, true);
                        ManaAdd(card.player,2);
                    }
                    break;
                case "エリゴス":
                    if (card.player == ev.player && TargetList(card.player,"enemy","hand").Count<=4)
                    {
                        SkillDisplay(card.order, 0, true);
                        list0 = SelectField(card.player, "both");
                        if (Select(card.player, list0, false, "効果の対象を選んでください"))
                        {
                            yield return AnswerWait(card.player, "select");
                            select = BattleJson.Select.ToSelect(content);
                            card0 = allCard.Find(c => c.order == select.orderList[0]);
                            yield return Level(card0.ToList(), -1);
                        }
                    }
                    break;
            }
        }
    }
    IEnumerator EventEnd()
    {
        SkillDisplay(-1, -1, false);
        List<Rule_Card> field = TargetList(first && firstPlayer,"both","field");
        List<Rule_Card> target = field.FindAll(c => c.power <= 0);
        if (target.Count > 0)yield return Death(target, true, false, true);

        //List<Rule_Card> death = field.FindAll(c => c.StateValue(21) == 0);
        //List<Rule_Card> sacrifice = field.FindAll(c => c.StateValue(21) > 0);
        //Move(death, "field", "cemetary");
        //Move(sacrifice, "cemetary", "equip");
        //Event ev = new Event()
        //{
        //    type = "death",
        //    targetCard = target,
        //    skill = true
        //};
        //eventList.Add(ev);
        //yield return EventStart();
    }

    void SkillDisplay(int order, int value, bool start)
    {
        ProcessRequest("skill", new Skill(order, value, start).ToString());
    }
    List<Rule_Card> FindDeck(bool player,bool isLeft)
    {
        if (player)
        {
            if (isLeft) return playerLeftDeck;
            else return playerRightDeck;
        }
        else
        {
            if (isLeft) return enemyLeftDeck;
            else return enemyRightDeck;
        }
    }
    List<Rule_Card> SelectField(bool player,string target)
    {
        List<Rule_Card> select = new List<Rule_Card>();
        if (target == "both")
        {
            if (player)
            {
                select.AddRange(playerField);
                select.AddRange(enemyField);
            }
            else
            {
                select.AddRange(enemyField);
                select.AddRange(playerField);
            }
            select.RemoveAll(c => c.StateValue(17) > 0);
        }
        if (target == "player")
        {
            if(player) select.AddRange(playerField);
            else select.AddRange(enemyField);
            select.RemoveAll(c => c.StateValue(17) > 0);
        }
        if (target == "enemy")
        {
            if (player) select.AddRange(enemyField);
            else select.AddRange(playerField);
            select.RemoveAll(c => c.StateValue(17) > 0);
        }
        return select;
    }
    List<Rule_Card> TargetList(bool player, string target,string area)
    {
        List<Rule_Card> select = new List<Rule_Card>();
        if (area == "field")
        {
            if (target == "both")
            {
                if (player)
                {
                    select.AddRange(playerField);
                    select.AddRange(enemyField);
                }
                else
                {
                    select.AddRange(enemyField);
                    select.AddRange(playerField);
                }
            }
            if (target == "player")
            {
                if (player) select.AddRange(playerField);
                else select.AddRange(enemyField);
            }
            if (target == "enemy")
            {
                if (player) select.AddRange(enemyField);
                else select.AddRange(playerField);
            }
        }
        if (area == "equip")
        {
            if (target == "both")
            {
                if (player)
                {
                    select.AddRange(playerEquip);
                    select.AddRange(enemyEquip);
                }
                else
                {
                    select.AddRange(enemyEquip);
                    select.AddRange(playerEquip);
                }
            }
            if (target == "player")
            {
                if (player) select.AddRange(playerEquip);
                else select.AddRange(enemyEquip);
            }
            if (target == "enemy")
            {
                if (player) select.AddRange(enemyEquip);
                else select.AddRange(playerEquip);
            }
        }
        if (area == "cemetary")
        {
            if (target == "both")
            {
                if (player)
                {
                    select.AddRange(playerLeftCemetary);
                    select.AddRange(playerRightCemetary);
                    select.AddRange(enemyLeftCemetary);
                    select.AddRange(enemyRightCemetary);
                }
                else
                {
                    select.AddRange(enemyLeftCemetary);
                    select.AddRange(enemyRightCemetary);
                    select.AddRange(playerLeftCemetary);
                    select.AddRange(playerRightCemetary);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    select.AddRange(playerLeftCemetary);
                    select.AddRange(playerRightCemetary);
                }
                else
                {
                    select.AddRange(enemyLeftCemetary);
                    select.AddRange(enemyRightCemetary);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    select.AddRange(enemyLeftCemetary);
                    select.AddRange(enemyRightCemetary);
                }
                else
                {
                    select.AddRange(playerLeftCemetary);
                    select.AddRange(playerRightCemetary);
                }
            }
        }
        if (area == "hand")
        {
            if (target == "both")
            {
                if (player)
                {
                    select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                    select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                }
                else
                {
                    select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                    select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                }
                else
                {
                    select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    if (enemyLeftHand.Count >= enemyRightHand.Count)
                    {
                        select.AddRange(enemyLeftHand);
                        select.AddRange(enemyRightHand);
                    }
                    else
                    {
                        select.AddRange(enemyRightHand);
                        select.AddRange(enemyLeftHand);
                    }
                }
                else
                {
                    if (playerLeftHand.Count >= playerRightHand.Count)
                    {
                        select.AddRange(playerLeftHand);
                        select.AddRange(playerRightHand);
                    }
                    else
                    {
                        select.AddRange(playerRightHand);
                        select.AddRange(playerLeftHand);
                    }
                }
            }
        }
        return select;
    }
    List<Rule_Card> TargetListSide(bool player, string target, string area,bool isLeft)
    {
        List<Rule_Card> select = new List<Rule_Card>();
        if (area == "cemetary")
        {
            if (target == "both")
            {
                if (player)
                {
                    if(isLeft)select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
            }
            select.RemoveAll(c => c.StateValue(23) > 0);
        }
        if (area == "panish")
        {
            if (target == "both")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(enemyLeftCemetary);
                    else select.AddRange(enemyRightCemetary);
                }
                else
                {
                    if (isLeft) select.AddRange(playerLeftCemetary);
                    else select.AddRange(playerRightCemetary);
                }
            }
            select.RemoveAll(c => c.StateValue(23) == 0);
        }
        if (area == "hand")
        {
            if (target == "both")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                    if (isLeft) select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                    if (isLeft) select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(enemyLeftHand);
                    select.AddRange(enemyRightHand);
                }
                else
                {
                    if (isLeft) select.AddRange(playerLeftHand);
                    select.AddRange(playerRightHand);
                }
            }
        }
        if (area == "deck")
        {
            if (target == "both")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftDeck);
                    select.AddRange(playerRightDeck);
                    if (isLeft) select.AddRange(enemyLeftDeck);
                    select.AddRange(enemyRightDeck);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftDeck);
                    select.AddRange(enemyRightDeck);
                    if (isLeft) select.AddRange(playerLeftDeck);
                    select.AddRange(playerRightDeck);
                }
            }
            if (target == "player")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(playerLeftDeck);
                    select.AddRange(playerRightDeck);
                }
                else
                {
                    if (isLeft) select.AddRange(enemyLeftDeck);
                    select.AddRange(enemyRightDeck);
                }
            }
            if (target == "enemy")
            {
                if (player)
                {
                    if (isLeft) select.AddRange(enemyLeftDeck);
                    select.AddRange(enemyRightDeck);
                }
                else
                {
                    if (isLeft) select.AddRange(playerLeftDeck);
                    select.AddRange(playerRightDeck);
                }
            }
        }
        return select;
    }
    List<Rule_Card> SelectItem(bool selectPlayer,Event ev)
    {
        List<Rule_Card> list = new List<Rule_Card>();
        List<Rule_Card> equip = TargetList(selectPlayer, "player", "equip");
        List<Rule_Card> field = TargetList(selectPlayer, "player", "field");
        bool action = selectPlayer == ev.player;
        foreach (Rule_Card card in equip)
        {
            if(card.card.color!="白")if (!field.Exists(c => c.card.color == card.card.color)) continue;
            switch (card.card.title)
            {
                case "応急セット":
                    if (ev.value < 0 && ev.type == "damage" && ev.targetCard.Exists(c=>c.player==selectPlayer)) list.Add(card);
                    break;
                case "標準装備":
                    if (ev.type == "battle")
                        if(TargetList(card.player,"both","field").Exists(c=>c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "黄金の槍":
                    if (ev.type == "battle" && action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "待ち伏せ":
                    if (ev.type == "attack" && !action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order)) list.Add(card);
                    break;
                case "騎士団装備":
                    if (ev.type == "battle")
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "トラバサミ":
                    if (ev.type == "summon" && !action) list.Add(card);
                    break;
                case "応援":
                    if (ev.type == "side")list.Add(card);
                    break;
                case "突撃":
                    if (ev.type == "battle" && action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "泣き面に蜂":
                    if (ev.type == "break" && action)
                        if(SelectField(card.player,"enemy").Count>0)list.Add(card);
                    break;
                case "連鎖爆発":
                    if (ev.type == "equipdeath") 
                        if (ev.targetCard.Exists(c => c.player != selectPlayer))
                            if (SelectField(card.player, "enemy").Count > 0) list.Add(card);
                    break;
                case "早駆":
                    if (ev.type == "summon" && action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order)) list.Add(card);
                    break;
                case "燃え上がる戦意":
                    if (ev.type == "battle")
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order)) list.Add(card);
                    break;
                case "バトンタッチ":
                    if (ev.type == "summon" && action)
                        if (SelectField(card.player, "player").Count > 0) list.Add(card);
                    break;
                case "鉄の盾":
                    if (ev.type == "battle" && !action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "バリアフィールド":
                    if (ev.type == "side" && !action) list.Add(card);
                    break;
                case "ステルス":
                    if (ev.type == "attack" && action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order)) list.Add(card);
                    break;
                case "一時休戦":
                    if (ev.type == "battle")
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order)) list.Add(card);
                    break;
                case "恐怖":
                    if (ev.type == "summon" && !action)
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order)) list.Add(card);
                    break;
                case "魔女の木":
                    if (ev.type == "battle")
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))
                                if (TargetList(card.player, "enemy", "hand").Count<=4)list.Add(card);
                    break;
                case "サイレンス":
                    if (ev.type == "end")
                        if (SelectField(card.player, "enemy").Count > 0) list.Add(card);
                    break;
                case "死の選別":
                    if (ev.type == "attack" && action)
                        if (SelectField(card.player, "enemy").Exists(c=>c.level>1)) list.Add(card);
                    break;
                case "オーロラ":
                    if (ev.type == "end") list.Add(card);
                    break;
                case "道ずれ":
                    if (ev.type == "battle")
                        if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order))
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[1].order))list.Add(card);
                    break;
                case "初級霊召喚":
                    if (ev.type == "death")
                        if (ev.skill)
                            if (ev.targetCard.Exists(c=>c.player ==selectPlayer)) list.Add(card);
                    break;
                case "想定通り":
                    if (ev.type == "side" && action)
                        if (TargetList(card.player, "enemy", "field").Count >= TargetList(card.player, "player", "field").Count + 2) list.Add(card);
                    break;
                case "少女の祈り":
                    if (ev.type == "summon" && action)
                        if (SelectField(card.player, "enemy").Count > 0)
                            if (TargetList(card.player, "both", "field").Exists(c => c.order == ev.targetCard[0].order)) list.Add(card);
                    break;
            }
        }
        return list;
    }
    IEnumerator EquipEvent(Event ev)
    {
        Select select;
        Rule_Card card0;
        List<Rule_Card> equipList;
        Rule_Card forced;
        bool pass = false;
        int firstMana;
        int secondMana;
        while (true)
        {
            if (ev.player)
            {
                firstMana = playerMana;
                secondMana = enemyMana;
            }
            else
            {
                firstMana = enemyMana;
                secondMana = playerMana;
            }
            equipList = SelectItem(ev.player, ev);
            equipList.RemoveAll(c => c.card.cost > firstMana);
            forced = equipList.Find(c => c.card.skill1 == "強制発動");
            if (forced != null)
            {
                pass = false;
                yield return EquipEffect(forced, ev.targetCard, ev.player);
            }
            else
            {
                if (Select(ev.player, equipList, true, "発動するアイテムを選択してください"))
                {
                    yield return AnswerWait(ev.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    if (select.cancel)
                    {
                        if (pass) break;
                        else pass = true;
                    }
                    else
                    {
                        pass = false;
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return EquipEffect(card0, ev.targetCard, ev.player);
                    }
                }
                else
                {
                    if (pass) break;
                    else pass = true;
                }
            }
            if (ev.player)
            {
                firstMana = playerMana;
                secondMana = enemyMana;
            }
            else
            {
                firstMana = enemyMana;
                secondMana = playerMana;
            }
            equipList = SelectItem(!ev.player, ev);
            equipList.RemoveAll(c => c.card.cost > secondMana);
            forced = equipList.Find(c => c.card.skill1 == "強制発動");
            if (forced != null)
            {
                pass = false;
                yield return EquipEffect(forced, ev.targetCard, ev.player);
            }
            else
            {
                if (Select(!ev.player, equipList, true, "発動するアイテムを選択してください"))
                {
                    yield return AnswerWait(!ev.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    if (select.cancel)
                    {
                        if (pass) break;
                        else pass = true;
                    }
                    else
                    {
                        pass = false;
                        card0 = allCard.Find(c => c.order == select.orderList[0]);
                        yield return EquipEffect(card0, ev.targetCard, ev.player);
                    }
                }
                else
                {
                    if (pass) break;
                    else pass = true;
                }
            }
        }
    }
    IEnumerator EquipEffect(Rule_Card card,List<Rule_Card> list,bool evPlayer)
    {
        List<Rule_Card> list0;
        Select select;
        Rule_Card card0;
        CardEffect effect = new CardEffect()
        {
            order = card.order
        };
        ProcessRequest("magic", effect.ToString());
        Move(card.ToList(), "equip", "cemetary");
        ManaAdd(card.player, -card.card.cost);
        switch (card.card.title)
        {
            case "応急セット":
                SkillDisplay(card.order, 0, true);
                Draw(card.player, card.isLeft, 1);
                if(card.player!=evPlayer)yield return Power(TargetList(card.player, "player", "field"), 99999, "damage", true, card.player);
                break;
            case "標準装備":
                SkillDisplay(card.order, 0, true);
                yield return Power(list.Find(c=>c.player==card.player).ToList(), 2000, "turn", true, card.player);
                if (firstPlayer == card.player)if (first) yield return Power(list.Find(c => c.player == card.player).ToList(), 1000, "turn", true, card.player);
                if (!firstPlayer == card.player) if (!first) yield return Power(list.Find(c => c.player == card.player).ToList(), 1000, "turn", true, card.player);
                break;
            case "黄金の槍":
                SkillDisplay(card.order, 0, true);
                card0 = list.Find(c => c.player == card.player);
                if (card0.state[10]+card0.turnState[10]==0)yield return State(card0.ToList(), 10,1,true);
                else yield return Power(card0.ToList(), 3000, "turn", true,card.player);
                break;
            case "待ち伏せ":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "player");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Act(card0.ToList(), 1, true, card.player);
                }
                break;
            case "騎士団装備":
                SkillDisplay(card.order, 0, true);
                yield return Power(list.Find(c => c.player == card.player).ToList(), 4000, "turn", true, card.player);
                if (firstPlayer == card.player) if (!first) yield return Power(list.Find(c => c.player == card.player).ToList(), 1000, "turn", true, card.player);
                if (!firstPlayer == card.player) if (first) yield return Power(list.Find(c => c.player == card.player).ToList(), 1000, "turn", true, card.player);
                break;
            case "トラバサミ":
                SkillDisplay(card.order, 0, true);
                ManaAdd(!card.player, -2);
                break;
            case "応援":
                SkillDisplay(card.order, 0, true);
                yield return Power(TargetList(card.player,"player","field"), 3000, "turn", true, card.player);
                break;
            case "突撃":
                SkillDisplay(card.order, 0, true);
                yield return Power(list.Find(c => c.player == card.player).ToList(), 3000, "turn", true, card.player);
                break;
            case "泣き面に蜂":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Power(card0.ToList(), -5000, "damage", true, card.player);
                }
                break;
            case "連鎖爆発":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Power(card0.ToList(), -4000, "damage", true, card.player);
                }
                break;
            case "早駆":
                SkillDisplay(card.order, 0, true);
                yield return State(list[0].ToList(),14,1,false);
                break;
            case "燃え上がる戦意":
                SkillDisplay(card.order, 0, true);
                list0 = TargetList(card.player, "both","field");
                card0 = list.Find(c => c.player == card.player);
                list0.Remove(card0);
                if (list0.Count>0) yield return Power(list0, -3000, "damage", true, card.player);
                break;
            case "バトンタッチ":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "player");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Bounce(card0.ToList(), card.player);
                    ManaAdd(card.player, 2);
                }
                break;
            case "鉄の盾":
                SkillDisplay(card.order, 0, true);
                yield return Power(list.Find(c => c.player == card.player).ToList(), 3000, "turn", true, card.player);
                break;
            case "バリアフィールド":
                SkillDisplay(card.order, 0, true);
                yield return Power(TargetList(card.player, "player", "field"), 2000, "turn", true, card.player);
                break;
            case "ステルス":
                SkillDisplay(card.order, 0, true);
                yield return State(list[0].ToList(), 16, 1, true);
                break;
            case "一時休戦":
                SkillDisplay(card.order, 0, true);
                yield return Bounce(list.Find(c => c.player == card.player).ToList(),card.player);
                break;
            case "恐怖":
                SkillDisplay(card.order, 0, true);
                yield return Level(list[0].ToList(), 1);
                list0 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                card0 = list0.Find(c => c.card.type == "ユニット" || c.card.type == "進化ユニット");
                if (card0 != null)
                {
                    if(TargetList(card.player,"player","equip").Count<=1)Move(card0.ToList(), "cemetary", "equip");
                    else Move(card0.ToList(), "cemetary", "hand");
                }
                break;
            case "魔女の木":
                SkillDisplay(card.order, 0, true);
                if(TargetList(card.player,"enemy","hand").Count<=4)yield return Power(list.Find(c => c.player != card.player).ToList(), -4000, "power", true, card.player);
                break;
            case "サイレンス":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Power(card0.ToList(), -2000, "power", true, card.player);
                    yield return State(card0.ToList(), 19, 1, false);
                }
                break;
            case "死の選別":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                list0.RemoveAll(c => c.level == 1);
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    yield return Death(card0.ToList(), card.player);
                }
                SkillDisplay(card.order, 0, true);
                break;
            case "オーロラ":
                SkillDisplay(card.order, 0, true);
                list0 = TargetList(card.player, "both", "field");
                foreach(Rule_Card c in list0)yield return Power(c.ToList(), 5000 - c.power, "power", true, card.player);
                break;
            case "道ずれ":
                SkillDisplay(card.order, 0, true);
                yield return Death(list,  card.player);
                break;
            case "初級霊召喚":
                SkillDisplay(card.order, 0, true);
                list0 = TargetListSide(card.player, "player", "cemetary", card.isLeft);
                card0 = list0.Find(c => c.card.type == "ユニット" && c.card.cost >= 2);
                if (card0 != null && TargetList(card.player, "player", "field").Count != 5)
                {
                    Move(card0.ToList(), "cemetary", "field");
                    yield return Summon(card0, true);
                }
                break;
            case "想定通り":
                SkillDisplay(card.order, 0, true);
                if(TargetList(card.player,"enemy","field").Count>= TargetList(card.player, "player", "field").Count + 2)
                {
                    ManaAdd(card.player, 1);
                    Draw(card.player, card.isLeft, 1);
                }
                break;
            case "少女の祈り":
                SkillDisplay(card.order, 0, true);
                list0 = SelectField(card.player, "enemy");
                if (Select(card.player, list0, false, "効果の対象を選んでください"))
                {
                    yield return AnswerWait(card.player, "select");
                    select = BattleJson.Select.ToSelect(content);
                    card0 = allCard.Find(c => c.order == select.orderList[0]);
                    if (card0.card.color == "緑") yield return State(list, 20, 1, false);
                    if (card0.card.color == "赤") yield return State(list, 11, 1, false);
                    if (card0.card.color == "黄") yield return State(list, 5, 1, false);
                    if (card0.card.color == "黒") yield return State(list, 6, 1, false);
                }
                break;
        }
        Event ev = new Event()
        {
            type = "item",
            player = card.player,
            targetCard = card.ToList(),
        };
        eventList.Add(ev);
        yield return ItemEventStart();
    }
    int StateToNum(string state)
    {
        int num = -1;
        switch (state)
        {
            case "戦闘準備":
                num = 0;
                break;
            case "攻撃禁止":
                num = 1;
                break;
            case "防御禁止":
                num = 2;
                break;
            case "強制攻撃":
                num = 3;
                break;
            case "強制防御":
                num = 4;
                break;
            case "消滅無効":
                num = 5;
                break;
            case "破壊無効":
                num = 6;
                break;
            case "合札禁止":
                num = 7;
                break;
            case "強制発動":
                num = 8;
                break;
            case "貫通":
                num = 10;
                break;
            case "頑強":
                num = 11;
                break;
            case "体幹":
                num = 12;
                break;
            case "信念":
                num = 13;
                break;
            case "速攻":
                num = 14;
                break;
            case "再起":
                num = 15;
                break;
            case "浸透1":
                num = 16;
                break;
            case "浸透2":
                num = 16;
                break;
            case "浸透3":
                num = 16;
                break;
            case "浸透4":
                num = 16;
                break;
            case "浸透5":
                num = 16;
                break;
            case "隠密":
                num = 17;
                break;
            case "巨大2":
                num = 18;
                break;
            case "巨大3":
                num = 18;
                break;
            case "巨大4":
                num = 18;
                break;
            case "巨大5":
                num = 18;
                break;
            case "沈黙":
                num = 19;
                break;
            case "加護":
                num = 20;
                break;
            case "生贄":
                num = 21;
                break;
            case "火傷":
                num = 22;
                break;
            case "消滅":
                num = 23;
                break;
            case "重撃1":
                num = 24;
                break;
            case "重撃2":
                num = 24;
                break;
            case "重撃3":
                num = 24;
                break;
            case "強襲":
                num = 25;
                break;
            case "挑発":
                num = 26;
                break;
            case "猛毒1":
                num = 27;
                break;
            case "猛毒2":
                num = 27;
                break;
            case "猛毒3":
                num = 27;
                break;
            case "呪縛":
                num = 28;
                break;
        }
        return num;
    }
    public void Receive(bool player, string process, string content)
    {
        //Debug.Log("rule:" + process + ":" + content);
        answerPlayer = player;
        this.process = process;
        this.content = content;
        /*
        if (player)
        {
            if (process == null)
            {
                contentPlayer += content;
            }
            else if (process == "new")
            {
                processPlayer = null;
                contentPlayer = null;
            }
            else
            {
                processPlayer = process;
            }
        }
        else
        {
            if (process == null)
            {
                contentEnemy += content;
            }
            else if (process == "new")
            {
                processEnemy = null;
                contentEnemy = null;
            }
            else
            {
                processEnemy = process;
            }
        }
        */

        if (player)
        {
            processPlayer = process;
            contentPlayer = content;
        }
        else
        {
            processEnemy = process;
            contentEnemy = content;
        }

    }
    IEnumerator AnswerWaitW(string phase)
    {
        while (true)
        {
            while (processPlayer == null) yield return new WaitForSecondsRealtime(0.1f);
            while (contentPlayer == null) yield return new WaitForSecondsRealtime(0.1f);
            if (processPlayer == phase) break;
            else ErrorMessage(true, phase, processPlayer, contentPlayer);
        }
        while (true)
        {
            while (processEnemy == null) yield return new WaitForSecondsRealtime(0.1f);
            while (contentEnemy == null) yield return new WaitForSecondsRealtime(0.1f);
            if (processEnemy == phase) break;
            else ErrorMessage(false, phase, processEnemy, contentEnemy);
        }
    }
    IEnumerator AnswerWait(bool player, string phase)
    {
        if (player)
        {
            while (true)
            {
                while (processPlayer == null) yield return new WaitForSecondsRealtime(0.1f);
                while (contentPlayer == null) yield return new WaitForSecondsRealtime(0.1f);
                if (processPlayer == phase) break;
                else ErrorMessage(player, phase, processPlayer, contentPlayer);
            }
        }
        else
        {
            while (true)
            {
                while (processEnemy == null) yield return new WaitForSecondsRealtime(0.1f);
                while (contentEnemy == null) yield return new WaitForSecondsRealtime(0.1f);
                if (processEnemy == phase) break;
                else ErrorMessage(player, phase, processEnemy, contentEnemy);
            }
        }
    }
    IEnumerator AnswerWait(string phase)
    {
        while (true)
        {
            while (process == null) yield return new WaitForSecondsRealtime(0.1f);
            while (content == null) yield return new WaitForSecondsRealtime(0.1f);
            if (process == phase) break;
            else ErrorMessage(answerPlayer, phase, process, content);
        }
    }
    void ErrorMessage(bool player, string phase, string process, string content)
    {
        Debug.Log("Error:" + "phase=" + phase + "," + "process=" + process + "content=" + content);
        Error error = new Error
        {
            player = player,
            phase = phase,
            process = process,
            content = content
        };
        ProcessRequest("error", error.ToString());
    }
    void ProcessRequest(string process, string content)
    {
        this.process = null;
        this.content = null;
        processPlayer = null;
        contentPlayer = null;
        processEnemy = null;
        contentEnemy = null;
        pun.ProcessRequest(process, content);
    }
}

[Serializable]
class Rule_Card
{
    public string id;
    public Card card;
    public int order;
    public bool isLeft;
    public bool player;

    public int level;
    public int power;
    public int powerChange;
    public int powerChangeTurn;
    public int damage;
    public int[] state;
    public int[] turnState;
    public bool act;
    public bool prevSilence;

    public List<int> flag;

    public void Initial(Card card,int order, bool isLeft,bool player)
    {
        this.card = card;
        id = card.id;
        this.order=order;
        this.isLeft = isLeft;
        this.player = player;
        state = new int[29];
        turnState = new int[29];
        act = true;
        power = card.power1;
        level = 1;
    }
    public void ReSet()
    {
        act = true;
        power = card.power1;
        level = 1;
        state = new int[29];
        turnState = new int[29];
        powerChange = 0;
        powerChangeTurn = 0;
        damage = 0;
    }
    public int StateValue(int stateNo)
    {
        if (stateNo != 19)
        {
            if (state[19] + turnState[19] == 0)
            {
                if (stateNo == 24 || stateNo == 27)
                {
                    return state[stateNo] + turnState[stateNo];
                }
                else
                {
                    if (state[stateNo] != 0)
                    {
                        return state[stateNo];
                    }
                    else
                    {
                        return turnState[stateNo];
                    }
                }
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return state[19] + turnState[19];
        }
    }

    public List<Rule_Card> ToList()
    {
        return new List<Rule_Card>() { this };
    }
}

[Serializable] 
class Event
{
    public string type;
    public List<Rule_Card> targetCard;
    //public Rule_Card skillCard;
    public bool player;
    public int value;
    public int state;
    public bool skill;
    public Event ShallowCopy()
    {
        return (Event)MemberwiseClone();
    }
}