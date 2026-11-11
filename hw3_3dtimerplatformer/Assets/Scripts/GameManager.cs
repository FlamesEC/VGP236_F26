using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    float timeTrack = 0;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;
    float bestTime = 0;
    PlatformerPlayer player;

    bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlatformerPlayer>();
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack)) * 100);
        
        bestTime = PlayerPrefs.GetFloat("record", 0);
        ApplyBestTime();
    }

    public void ApplyBestTime()
    {
        PlayerPrefs.SetFloat("record", bestTime);

        if (bestTime == 0)
            bestTimeText.text = "--:--:--";
        else
            bestTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(bestTime / 60f), Mathf.Floor(bestTime) % 60, (bestTime - Mathf.Floor(bestTime)) * 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
        {
            if (bestTime > 0 && timeTrack <= bestTime && timeTrack + Time.deltaTime > bestTime)
                timerText.color = Color.softRed;

            timeTrack += Time.deltaTime;
        }

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack))*100);
    }

    public void SetTimerStatus(bool status)
    {
        running = status;

        if (status) // Start
        {
            AudioManager.instance.PlaySFX("sfx_floor");
            AudioManager.instance.PlayMusic("mus_venus");
        }
        else // End
        {
            AudioManager.instance.PlaySFX("sfx_floor", 1, 1.45f);
            AudioManager.instance.StopMusic(2);

            if (timeTrack < bestTime || bestTime == 0)
            {
                timerText.color = Color.green;
                bestTime = timeTrack;
                ApplyBestTime();
            }
        }
    }

    public void RestartLevel()
    {
        instance = null;
        player.enabled = false;
        AudioManager.instance.PlaySFX("sfx_time");
        AudioManager.instance.StopMusic(2);
        SceneManager.LoadScene(0);
    }
}
