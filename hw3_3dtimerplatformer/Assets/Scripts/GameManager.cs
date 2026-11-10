using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    float timeTrack = 0;
    public TextMeshProUGUI timerText;

    public bool running = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack)) * 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
            timeTrack += Time.deltaTime;

        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack))*100);
    }
}
