using UnityEngine;

public class paralax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float Parallax;
    [SerializeField] private string locationName;
    [SerializeField] private allScripts scripts;
    private float startPosX;

    private void Start() => startPosX = transform.localPosition.x;

    private void Update()
    {
        if (scripts.locations.totalLocation.gameName == locationName || locationName == "")
        {
            float distX = (cam.transform.position.x * (1 - Parallax));
            transform.localPosition = new Vector3(startPosX + distX, transform.localPosition.y, transform.localPosition.z);
        }
    }
}
