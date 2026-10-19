using NUnit.Framework;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TrigonometricObstacle : ObstacleMover
{
    public enum TrigonometricFunctions
    {
        SINE,
        COSINE
    }

    [System.Serializable]
    public class TrigonometricWeigh
    {
        public TrigonometricFunctions type;
        public Vector3 scale;
        public float speed;
        public float timeOffset;
    }

    public TrigonometricWeigh[] Afflictions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override Vector3 GetNewPos()
    {
        Vector3 pos = StartPos;

        foreach (TrigonometricWeigh weigh in Afflictions)
        {
            TrigonometricFunctions func = weigh.type;

            switch (func)
            {
                case TrigonometricFunctions.SINE:
                    pos += Mathf.Sin((Time.time + weigh.timeOffset) * (weigh.speed / 100f)) * weigh.scale;
                    break;
                case TrigonometricFunctions.COSINE:
                    pos += Mathf.Cos((Time.time + weigh.timeOffset) * (weigh.speed / 100f)) * weigh.scale;
                    break;
            }
        }

        return pos;
    }

    private void OnDrawGizmos()
    {
        foreach (TrigonometricWeigh weigh in Afflictions)
        {
            Color gizmoColor = Color.white;
            switch (weigh.type)
            {
                case TrigonometricFunctions.SINE:
                    gizmoColor = Color.red;
                    break;
                case TrigonometricFunctions.COSINE:
                    gizmoColor = Color.yellow;
                    break;
            }

            Gizmos.color = gizmoColor;

            if (Application.isPlaying)
            {
                Gizmos.DrawLine(StartPos, StartPos + weigh.scale);
                Gizmos.DrawLine(StartPos, StartPos - weigh.scale);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + weigh.scale);
                Gizmos.DrawLine(transform.position, transform.position - weigh.scale);
            }
        }
    }
}
