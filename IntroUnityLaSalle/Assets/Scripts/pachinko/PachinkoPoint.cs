using UnityEngine;

public class PachinkoPoint : MonoBehaviour
{
    public float points = 1000;

    private void OnDestroy()
    {
        //Pachinko.instance.AddPoints(points);
    }
}
