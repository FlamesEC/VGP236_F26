using UnityEngine;

public class JumpPad : InheritableVelocityObject
{
    [Space(20)]
    public float JumpForce = 20;

    private void Awake()
    {
        selfVelocity = Vector3.up * JumpForce;
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            AudioManager.instance.PlaySFX("sfx_jumppad");

        base.OnCollisionExit(collision);
    }
}
