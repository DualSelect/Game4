using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Battle_Match : MonoBehaviourPunCallbacks
{
    private Hashtable propsToSet;
    private Dictionary<string, RoomInfo> dictionary = new Dictionary<string, RoomInfo>();
    public Text lowNum;
    public Text highNum;
    public Text freeNum;
    public Text roomNum;
    public InputField roomPass;
    public GameObject roomPassError;

    public Dropdown bookDrop;
    public GameObject[] vote;
    public GameObject[] deside;
    public GameObject[] cancel;
    public Text[] player;
    public Text matching;
    int time;
    public GameObject deckError;
    public GameObject oldMessage;

    private const string URL =
            "https://script.google.com/macros/s/AKfycbzNaFvtG7jolJKHSD4bNP75xWNsvasArLV6fo3rjI3LEkIqo-hrx-0-fAQoEDBT-3kM/exec";
    void Start()
    {
        PhotonNetwork.NickName = "Player";
        PhotonNetwork.ConnectUsingSettings();
        foreach(Text text in player)
        {
            text.text = PlayerPrefs.GetString("Player", "新規プレイヤー");
        }

        StartCoroutine(MatchingTime());
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        _ = LoadNews("");
    }
    public async Cysharp.Threading.Tasks.UniTask<string> LoadNews(string masterName)
    {
        // シート名を追加パラメータとしてAPIを叩くURLを決定。
        // GASでは "exec"のあとに "?" をつけて "hoge=fuga" などと追記することでGETにパラメータを付与できる
        GameObject.Destroy(GameObject.Find("MainThreadDispatcher"));
        string sheetName = "News";
        var url = URL + "?sheetName=" + sheetName;
        Debug.Log(url);
        var result = await GetMasterAsync(url);
        try
        {
            Debug.Log(result);
            List<News> newsList = ListFromJson<News>(result);
            if (newsList != null)
            {
                newsList.Sort((a, b) => b.ver.CompareTo(a.ver));
                if (newsList[0].ver.ToString() == PhotonNetwork.AppVersion.Substring(0, newsList[0].ver.ToString().Length)) NewVersion();
                else OldVersion();
            }
            return masterName;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return e.Message;
        }
    }
    void NewVersion()
    {
        int rate = PlayerPrefs.GetInt("RP", 1000);
        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                if (rate < 1800) deside[i].GetComponent<Button>().interactable = true;
            }
            else if (i == 1)
            {
                if (rate >= 1700) deside[i].GetComponent<Button>().interactable = true;
            }
            else deside[i].GetComponent<Button>().interactable = true;
        }
    }
    void OldVersion()
    {
        deside[2].GetComponent<Button>().interactable = true;
        deside[3].GetComponent<Button>().interactable = true;
        oldMessage.SetActive(true);
    }
    public void LowRank()
    {
        PlayerPrefs.SetInt("Rank", 1);
        if (DeckCheak())
        {
            deckError.SetActive(false);
            Decide(0);
            propsToSet = new Hashtable();
            propsToSet["Room"] = "Low";
            PhotonNetwork.JoinRandomRoom(propsToSet, 2);
        }
        else
        {
            deckError.SetActive(true);
        }
    }
    public void HighRank()
    {
        PlayerPrefs.SetInt("Rank", 1);
        if (DeckCheak())
        {
            deckError.SetActive(false);
            Decide(1);
            propsToSet = new Hashtable();
            propsToSet["Room"] = "High";
            PhotonNetwork.JoinRandomRoom(propsToSet, 2);
        }
        else
        {
            deckError.SetActive(true);
        }
    }
    public void FreeRank()
    {
        PlayerPrefs.SetInt("Rank", 0);
        if (DeckCheak())
        {
            deckError.SetActive(false);
            Decide(2);
            propsToSet = new Hashtable();
            propsToSet["Room"] = "Free";
            PhotonNetwork.JoinRandomRoom(propsToSet, 2);
        }
        else
        {
            deckError.SetActive(true);
        }
    }
    public void RoomRank()
    {
        PlayerPrefs.SetInt("Rank", 0);
        if (DeckCheak())
        {
            deckError.SetActive(false);
            Decide(3);
            if (roomPass.text.Length != 4)
            {
                roomPassError.SetActive(true);
                return;
            }
            roomPassError.SetActive(false);
            propsToSet = new Hashtable();
            propsToSet["Room"] = "Room";
            propsToSet["Pass"] = roomPass.text;
            PhotonNetwork.JoinRandomRoom(propsToSet, 2);
        }
        else
        {
            deckError.SetActive(true);
        }
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.CustomRoomProperties = propsToSet;
        roomOptions.CustomRoomPropertiesForLobby = new[] { "Room" };
        if(propsToSet["Room"].ToString()=="Room") roomOptions.CustomRoomPropertiesForLobby = new[] { "Room","Pass" };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PhotonNetwork.Instantiate("BattlePun", new Vector3(0, 0, 0), Quaternion.identity);
    }
    public override void OnRoomListUpdate(List<RoomInfo> changedRoomList)
    {
        foreach (var info in changedRoomList)
        {
            if (!info.RemovedFromList)
            {
                dictionary[info.Name] = info;
            }
            else
            {
                dictionary.Remove(info.Name);
            }
        }
        int low = 0;
        int high = 0;
        int free = 0;
        int room = 0;
        foreach (var dic in dictionary)
        {
            if (dic.Value.CustomProperties["Room"] == null) continue;
            if (dic.Value.CustomProperties["Room"].ToString() == "Low")
            {
                low++;
            }
            if (dic.Value.CustomProperties["Room"].ToString() == "High")
            {
                high++;
            }
            if (dic.Value.CustomProperties["Room"].ToString() == "Free")
            {
                free++;
            }
            if (dic.Value.CustomProperties["Room"].ToString() == "Room")
            {
                room++;
            }
        }
        lowNum.text = "現在" + low + "部屋";
        highNum.text = "現在" + high + "部屋";
        freeNum.text = "現在" + free + "部屋";
        roomNum.text = "現在" + room + "部屋";
    }

    bool DeckCheak()
    {
        string prevId ="";
        int num = 0;
        for (int j = 0; j < 30; j++)
        {
            string id = PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "L" + j, "");
            if (id == "")
            {
                return false;
            }
            if(id != prevId)
            {
                num++;
                prevId = id;
            }
        }
        if(num>16) return false;

        num = 0;
        for (int j = 0; j < 30; j++)
        {
            string id = PlayerPrefs.GetString("D" + (bookDrop.value + 1) + "R" + j, "");
            if (id == "")
            {
                return false;
            }
            if (id != prevId)
            {
                num++;
                prevId = id;
            }
        }
        if (num > 16) return false;

        return true;
    }

    void Decide(int i)
    {
        bookDrop.enabled = false;
        if (i==3 && roomPass.text.Length != 4)
        {
            return;
        }
        foreach (GameObject obj in deside)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in cancel)
        {
            obj.SetActive(true);
        }
        vote[i].SetActive(true);
        time = 0;
        matching.text = "マッチング中\n" + time;
        matching.gameObject.SetActive(true);
    }
    public void Cancel()
    {
        bookDrop.enabled = true;
        PhotonNetwork.LeaveRoom();
        foreach (GameObject obj in deside)
        {
            obj.SetActive(true);
        }
        foreach (GameObject obj in cancel)
        {
            obj.SetActive(false);
        }
        foreach (GameObject obj in vote)
        {
            obj.SetActive(false);
        }
        matching.gameObject.SetActive(false);
    }
    System.Collections.IEnumerator MatchingTime()
    {
        while (true)
        {
            matching.text = "マッチング中\n" + time;
            time++;
            yield return new WaitForSecondsRealtime(1f);
        }
    }
    private static async UniTask<string> GetMasterAsync(string url)
    {
        var request = UnityWebRequest.Get(url);

        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
        {
            throw new Exception(request.error);
        }

        return request.downloadHandler.text;
    }
    public static List<T> ListFromJson<T>(string json)
    {
        var newJson = "{ \"list\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.list;
    }

    [Serializable]
    class Wrapper<T>
    {
        public List<T> list;
    }
}
