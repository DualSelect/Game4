using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KanKikuchi.AudioManager;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;
#if UNITY_WEBGL
#else
using Firebase.Firestore;
using Photon.Pun;
using Firebase.Extensions;
#endif
public class Deck_Menu : MonoBehaviour
{
    public InputField player;
    public Image[] rankColor;
    public Text rateText;
    public GameObject[] news;
    public GameObject newsContent;
    public GameObject prefab;
    private const string URL =
        "https://script.google.com/macros/s/AKfycbzNaFvtG7jolJKHSD4bNP75xWNsvasArLV6fo3rjI3LEkIqo-hrx-0-fAQoEDBT-3kM/exec";
    void Start()
    {
        if(PlayerPrefs.GetInt("Rank") == 1 && PlayerPrefs.GetInt("Battle") == 1)
        {
            Debug.Log("ペナルティ");
            int rpDiff = PlayerPrefs.GetInt("RpDiff");
            rpDiff /= 20;
            int rpChange = -20 + rpDiff;
            int rp = PlayerPrefs.GetInt("RP");
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
            DocumentReference docRef = db.Collection("WinLose").Document(PlayerPrefs.GetString("RoomNo") + "Error");
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
            PlayerPrefs.SetInt("Battle", 0);
#endif
        }


        BGMManager.Instance.ChangeBaseVolume(PlayerPrefs.GetInt("BGM",2) * 0.2f);
        SEManager.Instance.ChangeBaseVolume(PlayerPrefs.GetInt("SE", 3) * 0.2f);
        BGMManager.Instance.Play(BGMPath.MINSTREL2);
        player.text = PlayerPrefs.GetString("Player", "新規プレイヤー");
        int rate = PlayerPrefs.GetInt("RP", 1000);
        int maxRate = PlayerPrefs.GetInt("MaxRP", 1000);
        if (rate >= 1200) rankColor[0].color = new Color(1, 1, 1, 1);
        if (rate >= 1500) rankColor[1].color = new Color(1, 1, 1, 1);
        if (rate >= 1800) rankColor[2].color = new Color(1, 1, 1, 1);
        if (rate >= 2000) rankColor[3].color = new Color(1, 1, 1, 1);
        rateText.text = "現在:" + rate + "rp" + "\n" + "最高:" + maxRate + "rp";

        _ = LoadNews("");
    }
    public void ChangeName(string name)
    {
        PlayerPrefs.SetString("Player", name);
    }
    public async Cysharp.Threading.Tasks.UniTask<string> LoadNews(string masterName)
    {
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
                newsList.Sort((a, b) => b.id.CompareTo(a.id));
                newsContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 220 * newsList.Count);
                for (int i = 0; i < newsList.Count; i++)
                {
                    Vector3 v = new Vector3(0, - 200 * i, 0);
                    GameObject gameObject = Instantiate(prefab, v, Quaternion.identity);
                    Deck_News newsPrefab = gameObject.GetComponent<Deck_News>();
                    gameObject.transform.SetParent(newsContent.transform, false);
                    newsPrefab.Init(newsList[i]);
                }
                if (newsList[0].id > PlayerPrefs.GetFloat("news", 0))
                {
                    foreach(GameObject obj in news)
                    {
                        obj.SetActive(true);
                    }
                    PlayerPrefs.SetFloat("news", newsList[0].id);
                }
            }


            return masterName;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return e.Message;
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
