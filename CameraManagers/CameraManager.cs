using System.Threading.Tasks;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraManager
{
    public static CameraManager Instance { get; private set; }
    public float StartCameraSize;

    private Volume _volume;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private FilmGrain _filmGrain;
    private Bloom _bloom;
    private ColorAdjustments _colorAdjustments;
    private Task _currentZoom;

    public Task Initialize(Camera playerCamera)
    {
        Instance = this;
        StartCameraSize = Player.Instance.virtualCamera.GetComponent<CinemachineVirtualCamera>().m_Lens
            .FieldOfView;
        _volume = playerCamera.GetComponent<Volume>();
        return Task.CompletedTask;
    }

    public async Task CameraZoom(float changeSize, bool smoothly = false)
    {
        if (smoothly)
        {
            // Если задача уже выполняется
            if (_currentZoom != null && !_currentZoom.IsCompleted)
                return;

            _currentZoom = SmoothlyZoom(changeSize);
            await _currentZoom;
        }
        else
            Player.Instance.virtualCamera.m_Lens.FieldOfView = StartCameraSize + changeSize;
    }

    private async Task SmoothlyZoom(float changeSize)
    {
        if (changeSize == 0)
            changeSize = StartCameraSize - Player.Instance.virtualCamera.m_Lens.FieldOfView;

        float change = changeSize < 0 ? -1f : 1f;
        float targetSize = Player.Instance.virtualCamera.m_Lens.FieldOfView + changeSize;

        while (!Mathf.Approximately(Player.Instance.virtualCamera.m_Lens.FieldOfView, targetSize)) // Пока не целое
        {
            Player.Instance.virtualCamera.m_Lens.FieldOfView += change;
            await Task.Delay(15);
        }

        _currentZoom = null;
    }

    /// <summary>
    /// Гибкое изменение value у "propertyName"
    /// </summary>
    private async void AdjustParameter<T>(T parameter, float targetValue, float speed, string propertyName)
    {
        float startValue = (float)typeof(T).GetProperty(propertyName)?.GetValue(parameter);
        float step = 0.01f * Mathf.Sign(targetValue - startValue);

        while (Mathf.Abs(targetValue - startValue) > Mathf.Epsilon)
        {
            startValue += step;
            typeof(T).GetProperty(propertyName)?.SetValue(parameter, startValue);
            await Task.Delay(Mathf.RoundToInt(10 * speed));
        }
    }

    public void SetVolumeProfile(VolumeProfile volume)
    {
        if (volume)
            _volume.profile = volume;
    }

    public void SetNewVignetteSmoothness(float newValue, float speed = 1f)
    {
        _volume.profile.TryGet(out _vignette);
        AdjustParameter(_vignette, newValue, speed, "smoothness");
    }

    public void SetNewChromaticAberration(float newValue, float speed = 1f)
    {
        _volume.profile.TryGet(out _chromaticAberration);
        AdjustParameter(_chromaticAberration, newValue, speed, "intensity");
    }

    public void SetNewFilmGrain(float newValue, float speed = 1f, FilmGrainLookupParameter newType = null)
    {
        _volume.profile.TryGet(out _filmGrain);
        if (newType != null)
            _filmGrain.type = newType;
        AdjustParameter(_filmGrain, newValue, speed, "intensity");
    }

    public void SetNewBloom(float newValue, float speed = 1f, Color newColor = default)
    {
        _volume.profile.TryGet(out _bloom);
        if (newColor != default)
            _bloom.tint.value = newColor;
        AdjustParameter(_bloom, newValue, speed, "intensity");
    }

    public void SetNewContrastColorAdjustments(float speed = 1f, float newContrast = 0f, Color newColor = default)
    {
        _volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        AdjustParameter(_colorAdjustments, newContrast, speed, "contrast");
    }

    public void SetNewSaturationColorAdjustments(float speed = 1f, float newSaturation = 0f,
        Color newColor = default)
    {
        _volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        AdjustParameter(_colorAdjustments, newSaturation, speed, "saturation");
    }
}