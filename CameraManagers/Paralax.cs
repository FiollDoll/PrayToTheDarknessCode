using UnityEngine;
using UnityEngine.Serialization;

public class Paralax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float parallax;
    [SerializeField] private string locationName;
    private float _startPosX;

    private void Start() => _startPosX = transform.localPosition.x;

    private void Update()
    {
        if (ManageLocation.Instance.totalLocation.gameName != locationName && locationName != "") return;
        float distX = (cam.transform.position.x - _startPosX) * (1 - parallax);
        transform.localPosition = new Vector3(_startPosX + distX, transform.localPosition.y, transform.localPosition.z);
    }
}
