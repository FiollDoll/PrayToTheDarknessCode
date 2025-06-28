using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController
{
    public Volume Volume;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private FilmGrain _filmGrain;
    private Bloom _bloom;
    private ColorAdjustments _colorAdjustments;

    public PostProcessingController() { }

    public void SetVolumeProfile(VolumeProfile volumeProfile) => Volume.profile = volumeProfile;

    public async void AdjustParameter<T>(T parameter, float targetValue, float speed, string propertyName)
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

    public void SetVignetteSmoothness(float newValue, float speed)
    {
        Volume.profile.TryGet(out _vignette);
        AdjustParameter(_vignette, newValue, speed, "smoothness");
    }

    public void SetChromaticAberration(float newValue, float speed)
    {
        Volume.profile.TryGet(out _chromaticAberration);
        AdjustParameter(_chromaticAberration, newValue, speed, "intensity");
    }

    public void SetFilmGrain(float newValue, float speed, FilmGrainLookupParameter newType)
    {
        Volume.profile.TryGet(out _filmGrain);
        if (newType != null)
            _filmGrain.type = newType;
        AdjustParameter(_filmGrain, newValue, speed, "intensity");
    }

    public void SetBloom(float newValue, float speed, Color newColor = default)
    {
        Volume.profile.TryGet(out _bloom);
        if (newColor != default)
            _bloom.tint.value = newColor;
        AdjustParameter(_bloom, newValue, speed, "intensity");
    }

    public void SetContrastColorAdjustments(float newContrast, Color newColor, float speed)
    {
        Volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        AdjustParameter(_colorAdjustments, newContrast, speed, "contrast");
    }

    public void SetSaturationColorAdjustments(float newSaturation, Color newColor, float speed)
    {
        Volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        AdjustParameter(_colorAdjustments, newSaturation, speed, "saturation");
    }
}
