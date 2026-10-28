using UnityEngine;

public class GameManagerBase : MonoBehaviour
{
    public static GameManagerBase instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;

        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public virtual void DisconnectPlayer(LocalPlayerObject p) {}

    public virtual void ConnectPlayer(LocalPlayerObject p) {}
}
