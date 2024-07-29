using UnityEngine;

public class paralax : MonoBehaviour
{
    [SerializeField] private GameObject cam;
    [SerializeField] private float Parallax;
    private float startPosX;

    private void Start() => startPosX = transform.position.x;

    private void Update()
    {
        float distX = (cam.transform.position.x * (1 - Parallax));
        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);
    }
}
