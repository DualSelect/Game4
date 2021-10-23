using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
namespace BattleJson
{
    [Serializable]
    public class Info
    {
        public bool player;
        public string name;
        public int rp;
        public string[] leftDeck;
        public string[] rightDeck;
        public string leftChara;
        public string rightChara;
        public bool first;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Info ToInfo(string str)
        {
            return JsonUtility.FromJson<Info>(str);
        }
    }
    [Serializable]
    public class Mulligan
    {
        public bool player;
        public string[] leftDeck;
        public string[] rightDeck;
        public bool[] left;
        public bool[] right;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Mulligan ToMulligan(string str)
        {
            return JsonUtility.FromJson<Mulligan>(str);
        }
    }
    [Serializable]
    public class Mana
    {
        public bool player;
        public int now;
        public int next;
        public int change;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Mana ToMana(string str)
        {
            return JsonUtility.FromJson<Mana>(str);
        }
    }
    [Serializable]
    public class Turn
    {
        public bool player;
        public bool first;
        public int now;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Turn ToTurn(string str)
        {
            return JsonUtility.FromJson<Turn>(str);
        }
    }
    [Serializable]
    public class Main
    {
        public bool player;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Main ToMain(string str)
        {
            return JsonUtility.FromJson<Main>(str);
        }
    }
    [Serializable]
    public class Side
    {
        public bool player;
        public bool isLeft;
        public bool request;
        public string leftChara;
        public string rightChara;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Side ToSide(string str)
        {
            return JsonUtility.FromJson<Side>(str);
        }
    }
    [Serializable]
    public class Act
    {
        public bool player;
        public string act;
        public int order;
        public int orderTarget;
        public bool isLeft;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Act ToAct(string str)
        {
            return JsonUtility.FromJson<Act>(str);
        }
    }
    [Serializable]
    public class Error
    {
        public bool player;
        public string phase;
        public string process;
        public string content;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Error ToError(string str)
        {
            return JsonUtility.FromJson<Error>(str);
        }
    }
    [Serializable]
    public class Move
    {
        public int[] order;
        public string prev;
        public string after;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Move ToMove(string str)
        {
            return JsonUtility.FromJson<Move>(str);
        }
    }
    [Serializable]
    public class Select
    {
        public bool player;
        public int[] orderList;
        public string message;
        public bool cancel;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Select ToSelect(string str)
        {
            return JsonUtility.FromJson<Select>(str);
        }
    }
    [Serializable]
    public class Life
    {
        public bool player;
        public int life;
        public int change;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Life ToLife(string str)
        {
            return JsonUtility.FromJson<Life>(str);
        }
    }
    [Serializable]
    public class Level
    {
        public int order;
        public int now;
        public int change;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Level ToLevel(string str)
        {
            return JsonUtility.FromJson<Level>(str);
        }
    }
    [Serializable]
    public class Power
    {
        public string type;
        public int order;
        public int now;
        public int change;
        public bool skill;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Power ToPower(string str)
        {
            return JsonUtility.FromJson<Power>(str);
        }
    }
    [Serializable]
    public class State
    {
        public int order;
        public int stateNo;
        public int value;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static State ToState(string str)
        {
            return JsonUtility.FromJson<State>(str);
        }
    }
    [Serializable]
    public class Skill
    {
        public int order;
        public int value;
        public bool start;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Skill ToSkill(string str)
        {
            return JsonUtility.FromJson<Skill>(str);
        }
        public Skill(int order,int value,bool start)
        {
            this.order = order;
            this.value = value;
            this.start = start;
        }
    }
    [Serializable]
    public class Battle
    {
        public int attack;
        public int defence;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Battle ToBattle(string str)
        {
            return JsonUtility.FromJson<Battle>(str);
        }
    }
    public class CardEffect
    {
        public int order;
        public bool skill;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static CardEffect ToCardEffect(string str)
        {
            return JsonUtility.FromJson<CardEffect>(str);
        }
    }
    public class Finish
    {
        public bool winner;
        public new string ToString()
        {
            return JsonUtility.ToJson(this);
        }
        public static Finish ToFinish(string str)
        {
            return JsonUtility.FromJson<Finish>(str);
        }
    }
}
