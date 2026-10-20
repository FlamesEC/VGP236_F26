using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pachinko : MonoBehaviour
{
    public static Pachinko instance;

    public PachinkoCamera cam;
    public GameObject Debug;
    public TextMeshProUGUI ScoreText;

    public PachinkoLeaderboardManager leaderboard;

    public float TopY = 0;
    public float BottomY = -60;

    public int Points = 0;
    public int Lives = 5;
    public Image hpIcons;

    private void Awake()
    {
        if (instance != null)
        {
            print($"INSTANCE OF {this.GetType()} ALREADY EXISTS.");
            Destroy(this);
            return;
        }

        instance = this;
    }

    void Start()
    {
        ScoreText.text = "SCORE: " + Points.ToString("D6");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            Debug.SetActive(!Debug.activeSelf);
    }

    public void AddPoints(int points)
    {
        Points += points;

        if (Points < 0)
            Points = 0;

        ScoreText.text = "SCORE: " + Points.ToString("D6");
    }

    public void StartFreeze(float timeScale, float secondAmount)
    {
        StartCoroutine(FreezeTime(timeScale, secondAmount));
    }

    IEnumerator FreezeTime(float timeScale, float secondAmount)
    {
        Time.timeScale = timeScale;
        yield return new WaitForSecondsRealtime(secondAmount);
        Time.timeScale = 1;
    }

    public bool CanPlay()
    {
        return Lives > 0;
    }

    public void RemoveLife()
    {
        Lives--;
        hpIcons.rectTransform.sizeDelta = new Vector2(100 * Lives, 100);
    }

    public void PlayerDeath()
    {
        if (Lives <= 0)
        {
            leaderboard.LeaderboardObj.SetActive(true);
        }
    }
}
