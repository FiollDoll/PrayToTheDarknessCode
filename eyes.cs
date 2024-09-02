using UnityEngine;

public class eyes : MonoBehaviour
{
    private Transform Target;

    [Min(0f)] public float Radius = 1;

    [Range(0f, 1f)] public float TrackingForce = 1;

    private Vector3 _initPos, toTarget;

    private void Start()
    {
        Target = GameObject.Find("Player").transform;
        _initPos = transform.localPosition;
    }

    private void Update()
    {
        toTarget = transform.parent.InverseTransformPoint(Target.position) - _initPos;
        if (toTarget.x < 10 && toTarget.x > -10)
            transform.localPosition = _initPos + Vector3.ClampMagnitude(toTarget * TrackingForce, Radius);
    }
}
