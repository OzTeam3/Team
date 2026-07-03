using UnityEngine;

public class RotatingPlatform : RollingTrapBase
{
    protected override void Awake()
    {
        base.Awake();

        if (_rotationAxis == Vector3.right)
        {
            _rotationAxis = Vector3.up;
        }
    }
}
