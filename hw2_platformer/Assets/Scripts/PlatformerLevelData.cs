using UnityEngine;

public class PlatformerLevelData : MonoBehaviour
{
    public Transform[] spawns;

    public Animator transitionAnimator;
    public AnimationClip transitionStart;
    public AnimationClip transitionEnd;

    public Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam.transparencySortMode = TransparencySortMode.CustomAxis;
        cam.transparencySortAxis = new Vector3(1, 0, 0);

        transitionAnimator.Play(transitionEnd.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < spawns.Length; i++)
        {
            Color color = Color.rebeccaPurple;

            if (i == 0)
                color = Color.blue;
            if (i == 1)
                color = Color.red;

            Gizmos.color = color;

            Gizmos.DrawSphere(spawns[i].transform.position, 0.25f);
        }
    }
}
