using UnityEngine;

public class ConstantObstacleRotator : ObstacleRotator
{
    public Vector3 rotSpeed;

    public override Vector3 GetNewRot()
    {
        return StartRot + rotSpeed * Time.time;
    }
}
