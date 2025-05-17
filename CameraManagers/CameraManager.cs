using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager
{
    public static CameraManager Instance { get; private set; }
    private PostProcessingController _postProcessingController;
    public float StartCameraSize;
    private CoroutineContainer _coroutineContainer;
    
    public void Initialize(CoroutineContainer coroutineContainer, Camera playerCamera)
    {
        Instance = this;
        StartCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;
        _postProcessingController = new PostProcessingController();
        _postProcessingController.Volume = playerCamera.GetComponent<Volume>();
        _coroutineContainer = coroutineContainer;
    }

    public void CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
            _coroutineContainer.StartCoroutine(SmoothlyZoom(changeSize));
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = StartCameraSize + changeSize;
    }

    private IEnumerator SmoothlyZoom(float changeSize)
    {
        if (changeSize == 0)
            changeSize = StartCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView;

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
    }

    public void SetVolumeProfile(VolumeProfile volume) => _postProcessingController.SetVolumeProfile(volume);

    public void SetNewVignetteSmoothness(float newValue, float speed = 1f) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetVignetteSmoothness(newValue, speed));

    public void SetNewChromaticAberration(float newIntensity, float speed = 1f) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetChromaticAberration(newIntensity, speed));

    public void SetNewFilmGrain(float newIntensity, float speed = 1f, FilmGrainLookupParameter newType = null) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetFilmGrain(newIntensity, speed, newType));

    public void SetNewBloom(float newIntensity, float speed = 1f, Color newColor = default) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetBloom(newIntensity, speed, newColor));

    public void SetNewContrastColorAdjusments(float speed = 1f, float newContrast = 0f, Color newColor = default) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetContrastColorAdjusments(newContrast, newColor, speed));

    public void SetNewSaturationColorAdjusments(float speed = 1f, float newSaturation = 0f, Color newColor = default) =>
        _coroutineContainer.StartCoroutine(_postProcessingController.SetSaturationColorAdjusments(newSaturation, newColor, speed));
}