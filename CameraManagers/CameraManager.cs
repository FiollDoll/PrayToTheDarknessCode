using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager
{
    public static CameraManager Instance { get; private set; }
    private PostProcessingController _postProcessingController;
    public float StartCameraSize;

    public void Initialize(Camera playerCamera)
    {
        Instance = this;
        StartCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;
        _postProcessingController = new PostProcessingController();
        _postProcessingController.Volume = playerCamera.GetComponent<Volume>();
    }

    public void CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
            SmoothlyZoom(changeSize);
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = StartCameraSize + changeSize;
    }

    public async Task SmoothlyZoom(float changeSize)
    {
        if (changeSize == 0)
            changeSize = StartCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView;

        float change = changeSize < 0 ? -1f : 1f;
        float targetSize = Player.Instance.virtualCamera.m_Lens.FieldOfView + changeSize;

        while (!Mathf.Approximately(Player.Instance.virtualCamera.m_Lens.FieldOfView, targetSize)) // Пока не целое
        {
            Player.Instance.virtualCamera.m_Lens.FieldOfView += change;
            await Task.Delay(10);
        }
    }

    public void SetVolumeProfile(VolumeProfile volume) => _postProcessingController.SetVolumeProfile(volume);

    public void SetNewVignetteSmoothness(float newValue, float speed = 1f) =>
        _postProcessingController.SetVignetteSmoothness(newValue, speed);

    public void SetNewChromaticAberration(float newIntensity, float speed = 1f) =>
        _postProcessingController.SetChromaticAberration(newIntensity, speed);

    public void SetNewFilmGrain(float newIntensity, float speed = 1f, FilmGrainLookupParameter newType = null) =>
        _postProcessingController.SetFilmGrain(newIntensity, speed, newType);

    public void SetNewBloom(float newIntensity, float speed = 1f, Color newColor = default) =>
        _postProcessingController.SetBloom(newIntensity, speed, newColor);

    public void SetNewContrastColorAdjustments(float speed = 1f, float newContrast = 0f, Color newColor = default) =>
        _postProcessingController.SetContrastColorAdjustments(newContrast, newColor, speed);

    public void SetNewSaturationColorAdjustments(float speed = 1f, float newSaturation = 0f,
        Color newColor = default) =>
        _postProcessingController.SetSaturationColorAdjustments(newSaturation, newColor, speed);
}