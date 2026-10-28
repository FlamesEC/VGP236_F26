using UnityEngine;

[ExecuteInEditMode]
public class SpriteSizeDuplicator : MonoBehaviour
{
    public SpriteRenderer target;
    SpriteRenderer sr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sr == null || target == null)
            return;

        sr.size = target.size;
    }
}
