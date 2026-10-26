using UnityEngine;

[ExecuteInEditMode]
public class PlatformEditorModifier : MonoBehaviour
{
    public BoxCollider2D col;
    public SpriteRenderer sr;

    [Space(20)]
    public Vector2 Size;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Size = sr.size;
    }

    // Update is called once per frame
    void Update()
    {
        sr.size = Size;
        col.size = Size;
    }
}
