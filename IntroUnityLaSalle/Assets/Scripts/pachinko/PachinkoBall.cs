using UnityEditor;
using UnityEngine;

public class PachinkoBall : MonoBehaviour
{
    public AnimatedObject animator;

    private void OnEnable()
    {
        Pachinko.instance.cam.Target = transform;
    }

    private void Update()
    {
         if (transform.position.y < Pachinko.instance.BottomY - 10)
            Destroy(gameObject);
    }

    private void OnDisable()
    {
        Pachinko.instance.cam.RemoveTarget(transform);
    }

    private void OnDestroy()
    {
        Pachinko.instance.cam.RemoveTarget(transform);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "DefaultBounce" || collision.gameObject.tag == "BadBounce")
        {
            AnimatedObject anim = collision.gameObject.GetComponent<AnimatedObject>();

            if (anim != null)
                anim.changeAnimation(1);
            else
            {
                anim = collision.gameObject.GetComponentInChildren<AnimatedObject>();
                if (anim != null)
                    anim.changeAnimation(1);
            }

            animator.changeAnimation(1);

            Pachinko.instance.StartFreeze(0.05f, 0.1f);

            if (collision.gameObject.tag == "BadBounce")
                Pachinko.instance.AddPoints(-300);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "PointCollectible")
        {
            collision.enabled = false;

            AnimatedObject anim = collision.gameObject.GetComponent<AnimatedObject>();

            if (anim != null)
                anim.changeAnimation(1);
            else
            {
                anim = collision.gameObject.GetComponentInChildren<AnimatedObject>();
                if (anim != null)
                    anim.changeAnimation(1);
            }

            PachinkoPoint p = collision.GetComponent<PachinkoPoint>();
            if (p != null)
                Pachinko.instance.AddPoints(Mathf.RoundToInt(p.points));

            Destroy(collision.gameObject, 1);
        }
    }
}
