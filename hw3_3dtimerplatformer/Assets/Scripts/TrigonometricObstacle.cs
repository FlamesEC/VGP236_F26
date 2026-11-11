using NUnit.Framework;
using UnityEngine;

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

    private void Start()
    {
        transform.position = GetStartPos();
    }

    public Vector3 GetStartPos()
    {
        Vector3 pos = transform.position;
        if (Application.isPlaying)
            pos = StartPos;

        foreach (TrigonometricWeigh weigh in Afflictions)
        {
            TrigonometricFunctions func = weigh.type;

            switch (func)
            {
                case TrigonometricFunctions.SINE:
                    pos += Mathf.Sin(weigh.timeOffset * (weigh.speed / 100f) - (Mathf.PI / 2)) * weigh.scale;
                    break;
                case TrigonometricFunctions.COSINE:
                    pos += Mathf.Cos(weigh.timeOffset * (weigh.speed / 100f) - (Mathf.PI / 2)) * weigh.scale;
                    break;
            }
        }

        return pos;
    }

    public override Vector3 GetNewSpeed()
    {
        Vector3 speed = StartSpeed;

        foreach (TrigonometricWeigh weigh in Afflictions)
        {
            TrigonometricFunctions func = weigh.type;

            switch (func)
            {
                case TrigonometricFunctions.SINE:
                    speed += Mathf.Sin((Time.timeSinceLevelLoad + weigh.timeOffset) * (weigh.speed / 100f)) * weigh.scale * (weigh.speed / 100f);
                    break;
                case TrigonometricFunctions.COSINE:
                    speed += Mathf.Cos((Time.timeSinceLevelLoad + weigh.timeOffset) * (weigh.speed / 100f)) * weigh.scale * (weigh.speed / 100f);
                    break;
            }
        }

        return speed;
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

        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(GetStartPos(), 1);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(GetStartPos(), 1);
        }
    }
}
