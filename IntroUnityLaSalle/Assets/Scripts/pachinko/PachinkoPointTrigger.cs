using UnityEngine;

public class PachinkoPointTrigger : MonoBehaviour
{
    GameObject ballHit;
    public int points;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Ball")
            return;

        if (ballHit == collision.gameObject)
            return;

        ballHit = collision.gameObject;

        Pachinko.instance.AddPoints(points);
    }
}
