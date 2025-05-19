using UnityEngine;
using UnityEngine.Serialization;

public class Paralax : MonoBehaviour
{
    [SerializeField] private float parallax;
    [SerializeField] private string locationName;
    private GameObject _cam;
    private float _startPosX;

    private void Start()
    {
        _startPosX = transform.localPosition.x;
        _cam = Camera.main.gameObject;
    }

    private void Update()
    {
        if (ManageLocation.Instance.TotalLocation.gameName != locationName && locationName != "") return;
        float distX = (_cam.transform.position.x - _startPosX) * (1 - parallax);
        transform.localPosition = new Vector3(_startPosX + distX, transform.localPosition.y, transform.localPosition.z);
    }
}
