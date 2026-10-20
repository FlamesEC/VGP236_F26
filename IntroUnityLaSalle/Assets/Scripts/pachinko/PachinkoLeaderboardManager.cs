using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.Impl;

[Serializable]
public class Leaderboard //https://docs.unity3d.com/6000.6/Documentation/ScriptReference/JsonUtility.html
{
    public List<string> Scores;

    public const string PlayerPrefsKeyName = "LeaderboardObj";

    public void SaveToPlayerPrefs()
    {
        // Convert this GameState instance to a JSON string
        string json = JsonUtility.ToJson(this);

        // Save the converted JSON into the PlayerPrefs
        PlayerPrefs.SetString(PlayerPrefsKeyName, json);
        PlayerPrefs.Save();
    }

    public static Leaderboard CreateFromPlayerPrefs()
    {
        // If the game was never saved before, the key will not exist; in this case return null
        if (!PlayerPrefs.HasKey(PlayerPrefsKeyName))
            return null;

        // Retrieve the saved JSON string from the player prefs
        string json = PlayerPrefs.GetString(PlayerPrefsKeyName);

        // Deserialize the JSON string into a new GameState object and return it
        return JsonUtility.FromJson<Leaderboard>(json);
    }

    public int GetScoreFromName(string name)
    {
        foreach (string score in Scores)
        {
            string[] s = score.Split('\\');

            if (s.Length < 2) continue;

            if (s[0] == name)
                return int.Parse(s[1]);
        }

        return 0;
    }

    public void AddScore(string name, int score)
    {
        RemoveScore(name);

        Scores.Add($"{name}\\{score}");
    }

    public void RemoveScore(string name)
    {
        int index = -1;

        for (int i = 0; i < Scores.Count; i++)
        {
            string[] s = Scores[i].Split('\\');

            if (s.Length == 0) continue;

            if (s[0] == name)
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
            Scores.RemoveAt(index);
    }
}

public class PachinkoLeaderboardManager : MonoBehaviour
{
    public class LeaderboardElement
    {
        public string name;
        public int score;

        public LeaderboardElement(string n, int s) { name = n; score = s; }
    }

    public List<GameObject> runtimeLeaderboardElements;
    public GameObject LeaderboardObj;

    public RectTransform LeaderboardContentObject;
    public Leaderboard LeaderboardC;

    public GameObject LeaderboardPrefabElement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Pachinko.instance.leaderboard = this;
        LeaderboardObj.SetActive(false);

        LeaderboardC = Leaderboard.CreateFromPlayerPrefs();
        if (LeaderboardC == null)
        {
            LeaderboardC = new Leaderboard();
            LeaderboardC.Scores = new List<string>();
        }

        UpdateLeaderboard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void UpdateLeaderboard()
    {
        if (LeaderboardC == null)
            return;

        for (int i = runtimeLeaderboardElements.Count - 1; i >= 0; i--)
            Destroy(runtimeLeaderboardElements[i]);

        List<LeaderboardElement> elements = new List<LeaderboardElement>();

        for (int i = 0; i < LeaderboardC.Scores.Count; i++)
        {
            string[] s = LeaderboardC.Scores[i].Split('\\');
            elements.Add(new LeaderboardElement(s[0], int.Parse(s[1])));
        }

        elements = elements.OrderByDescending(x =>  x.score).ToList();

        float height = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            PachinkoLeaderboardElement e = Instantiate(LeaderboardPrefabElement, LeaderboardContentObject).GetComponent<PachinkoLeaderboardElement>();
            e.SetIndexData(i);
            e.SetText($"{elements[i].name} - {elements[i].score.ToString("D6")}");

            height += e.image.rectTransform.sizeDelta.y;

            runtimeLeaderboardElements.Add(e.gameObject);
        }

        LeaderboardContentObject.sizeDelta = new Vector2(LeaderboardContentObject.sizeDelta.x, height);

        LeaderboardC.SaveToPlayerPrefs();
    }

    public void AddScore(string playerName, int score)
    {
        LeaderboardC.AddScore(playerName, score);
        UpdateLeaderboard();
    }

    public void AddScoreButtonUnity(TextMeshProUGUI reference)
    {
        AddScore(reference.text, Pachinko.instance.Points);
    }
}
