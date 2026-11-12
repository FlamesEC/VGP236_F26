using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    float timeTrack = 0;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;
    float bestTime = 0;
    PlatformerPlayer player;

    public Canvas MOBILE_CANVAS;
    private PointerEventData pointerData;
    private List<RaycastResult> results = new();

    bool running = false;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        instance = this;

        pointerData = new PointerEventData(EventSystem.current);

        MOBILE_CANVAS.gameObject.SetActive(Keyboard.current == null && Touchscreen.current != null);
    }

    bool IsPointerOverUI(Vector2 position)
    {
        pointerData.position = position;
        results.Clear();

        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count > 0;
    }

    public Vector2 GetTouchscreenDelta()
    {
        Vector2 delta = Vector2.zero;

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (IsPointerOverUI(Input.touches[i].position))
                continue;

            delta += Input.touches[i].deltaPosition;
        }

        return delta;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlatformerPlayer>();
        
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(timeTrack / 60f), Mathf.Floor(timeTrack) % 60, (timeTrack - Mathf.Floor(timeTrack)) * 100);
        
        bestTime = PlayerPrefs.GetFloat("record", 0);
        ApplyBestTime();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        if (Touchscreen.current != null && Touchscreen.current.wasUpdatedThisFrame)
            MOBILE_CANVAS.gameObject.SetActive(true);

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
