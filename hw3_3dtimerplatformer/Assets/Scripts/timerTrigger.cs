using UnityEngine;

public class timerTrigger : MonoBehaviour
{
    public bool newTimerState = true;

    private void OnTriggerEnter(Collider other)
    {
        if (newTimerState)
            return;

        if (other.tag != "Player")
            return;

        GameManager.instance.SetTimerStatus(newTimerState);
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!newTimerState)
            return;

        if (other.tag != "Player")
            return;

        GameManager.instance.SetTimerStatus(newTimerState);
        Destroy(gameObject);
    }
}
