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
    
    public void Initialize(Camera playerCamera)
    {
        Instance = this;
        StartCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;
        _postProcessingController = new PostProcessingController();
        _postProcessingController.Volume = playerCamera.GetComponent<Volume>();
        _coroutineContainer = CoroutineContainer.Instance;
    }

    public void CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
            _coroutineContainer.StartCoroutine(SmoothlyZoom(changeSize));
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = StartCameraSize + changeSize;
    }

    public IEnumerator SmoothlyZoom(float changeSize)
    {
        if (changeSize == 0)
            changeSize = StartCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView;

        float change = changeSize < 0 ? -1f : 1f;
        float targetSize = Player.Instance.virtualCamera.m_Lens.FieldOfView + changeSize;

        while (Player.Instance.virtualCamera.m_Lens.FieldOfView != targetSize) // Пока не целое
        {
            Player.Instance.virtualCamera.m_Lens.FieldOfView += change;
            yield return null;
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