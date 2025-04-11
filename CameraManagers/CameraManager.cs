using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraManager: MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [HideInInspector] public float startCameraSize;

    private void Awake() => Instance = this;
    
    private void Start()
    {
        startCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;
    }

    public void CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
            StartCoroutine(SmoothlyZoom(changeSize));
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = CameraManager.Instance.startCameraSize + changeSize;
    }

    private IEnumerator SmoothlyZoom(float changeSize)
    {
        if (changeSize < 0) // Если отрицательное
        {
            for (float i = 0; i > changeSize; i--)
            {
                Player.Instance.virtualCamera.m_Lens.FieldOfView -= 1f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else if (changeSize > 0)
        {
            for (float i = 0; i < changeSize; i++)
            {
                Player.Instance.virtualCamera.m_Lens.FieldOfView += 1f;
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
            StartCoroutine(SmoothlyZoom(startCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView));
    }
}