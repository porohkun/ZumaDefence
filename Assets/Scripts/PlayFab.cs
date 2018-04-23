using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MimiJson;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayFab : MonoBehaviour
{
    public enum ApiCall
    {
        LoginWithCustomID,
        UpdateUserData,
        UpdatePlayerStatistics,
        GetLeaderboard,
        UpdateUserTitleDisplayName
    }

    private static PlayFab _instance;
    public static string UserId { get; private set; }
    public static string SessionTicket { get; private set; }
    public static string PlayFabId { get; private set; }

    [SerializeField]
    private string _gameID;

    private static bool _logined = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
        _instance = this;
#if UNITY_EDITOR
        UserId = "EDITOR";
#else
            UserId = System.Guid.NewGuid().ToString();
#endif
    }

    public static void LoginUser(Action callback)
    {
        _logined = false;
#if UNITY_EDITOR
        UserId = "EDITOR";
#else
            UserId = System.Guid.NewGuid().ToString();
#endif
        //if (_logined)
        //    return;
        var json = new JsonObject(
            new JOPair("CustomId", UserId),
#if UNITY_EDITOR
                new JOPair("CreateAccount", false),
#else
                new JOPair("CreateAccount", true),
#endif
                new JOPair("TitleId", _instance._gameID));

        _instance.StartCoroutine(_instance.Send(json, ApiCall.LoginWithCustomID, (jsonResponse) =>
        {
            SessionTicket = jsonResponse["data"]["SessionTicket"];
            PlayFabId = jsonResponse["data"]["PlayFabId"];
            _logined = true;
            if (callback != null)
                callback();
        }));
    }

    public static void SendName(string name, Action callback)
    {
        var json = new JsonObject(
            new JOPair("DisplayName", System.Guid.NewGuid().ToString().Substring(0, 6) + "|" + name)
            );

        _instance.StartCoroutine(_instance.Send(json, ApiCall.UpdateUserTitleDisplayName, (_) => { if (callback != null) callback(); }));
    }

    public static void SendScores(string board, int score, Action callback)
    {
        var json = new JsonObject(
            new JOPair("Statistics", new JsonArray(
                new JsonObject(
                    new JOPair("StatisticName", board),
                    new JOPair("Value", score)
                    ))));

        _instance.StartCoroutine(_instance.Send(json, ApiCall.UpdatePlayerStatistics, (_) => { if (callback != null) callback(); }));
    }

    public static void GetScores(string board, Action<LeaderBoard> callback)
    {
        var json = new JsonObject(
            new JOPair("StatisticName", board),
            new JOPair("StartPosition", 0),
            new JOPair("MaxResultsCount", 10)
            );

        _instance.StartCoroutine(_instance.Send(json, ApiCall.GetLeaderboard, (jsonResponse) =>
         {
             if (callback != null)
             {
                 var lb = new LeaderBoard();
                 foreach (JsonObject position in jsonResponse["data"]["Leaderboard"].Array)
                 {
                     var name = position.ContainsKey("DisplayName") ? position["DisplayName"].String.Substring(7) : position["PlayFabId"].String;
                     var score = position["StatValue"];
                     var pos = position["Position"];
                     lb.SetPosition(pos, name, score);
                 }
                 callback(lb);
             }
         }));
    }

    private IEnumerator Send(JsonValue json, ApiCall call, Action<JsonValue> callback)
    {
        if (call != ApiCall.LoginWithCustomID)
            while (!_logined)
                yield return null;
        var headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json");
        if (call != ApiCall.LoginWithCustomID)
            headers.Add("X-Authentication", SessionTicket);

        WWW api = new WWW(string.Format("https://{0}.playfabapi.com/Client/{1}", _gameID, call), json.ToBytes(new JsonComposeSettings(false, encoding: Encoding.UTF8)), headers);
        while (!api.isDone)
            yield return null;
        Debug.Log(api.text);
        var jsonResponse = JsonValue.Parse(api.text);
        if (callback != null)
            callback(jsonResponse);
    }
}
