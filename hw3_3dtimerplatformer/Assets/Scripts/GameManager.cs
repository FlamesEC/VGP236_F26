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
    PlatformerPlayer player;

    bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlatformerPlayer>();
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack)) * 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
            timeTrack += Time.deltaTime;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack))*100);
    }

    public void SetTimerStatus(bool status)
    {
        running = status;
        AudioManager.instance.PlaySFX("sfx_floor", 1, status ? 1 : 1.45f);
    }

    public void RestartLevel()
    {
        instance = null;
        player.enabled = false;
        AudioManager.instance.PlaySFX("sfx_time");
        SceneManager.LoadScene(0);
    }
}
