using UnityEngine;

public class eyes : MonoBehaviour
{
    private Transform Target;
 
    [Min(0f)] public float Radius = 1;
 
    [Range(0f, 1f)] public float TrackingForce = 1;
 
    private Vector3 _initPos;
 
    private void Start()
    {
        Target = GameObject.Find("Player").transform;
        if (transform.parent != null)
            _initPos = transform.localPosition;
        else
            _initPos = transform.position;
    }
 
    private void Update()
    {
        Vector3 toTarget;
        if (transform.parent != null)
        {
            toTarget = transform.parent.InverseTransformPoint(Target.position) - _initPos;
            transform.localPosition = _initPos + Vector3.ClampMagnitude(toTarget * TrackingForce, Radius);
        }
        else
        {
            toTarget = Target.position - _initPos;
            transform.position = _initPos + Vector3.ClampMagnitude(toTarget * TrackingForce, Radius);
        }
    }
}
