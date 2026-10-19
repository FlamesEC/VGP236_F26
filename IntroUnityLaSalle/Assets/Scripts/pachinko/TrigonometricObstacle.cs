using NUnit.Framework;
using UnityEngine;

public class TrigonometricObstacle : ObstacleMover
{
    public enum TrigonometricFunctions
    {
        SINE,
        COSINE,
        TANGENT
    }

    [System.Serializable]
    public class TrigonometricWeigh
    {
        public TrigonometricFunctions type;
        public Vector3 scale;
        public float speed;
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
                    pos += Mathf.Sin(Time.time * weigh.speed) * weigh.scale;
                    break;
                case TrigonometricFunctions.COSINE:
                    pos += Mathf.Cos(Time.time * weigh.speed) * weigh.scale;
                    break;
                case TrigonometricFunctions.TANGENT:
                    pos += Mathf.Tan(Time.time * weigh.speed) * weigh.scale;
                    break;
            }
        }

        return pos;
    }
}
