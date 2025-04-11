using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController
{
    public Volume Volume;
    public GameObject PlayerCamera;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private FilmGrain _filmGrain;
    private Bloom _bloom;
    private ColorAdjustments _colorAdjustments;


    public PostProcessingController()
    {
    }

    public void SetVolumeProfile(VolumeProfile volumeProfile) => Volume.profile = volumeProfile;

    // Методы для изменения параметров в активновном времени

    public IEnumerator SetVignetteSmoothness(float newValue, float speed)
    {
        Volume.profile.TryGet(out _vignette);
        float startValue = _vignette.smoothness.value;

        // Если меньше
        if (newValue < startValue)
        {
            while (newValue < startValue)
            {
                _vignette.smoothness.value -= 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newValue > startValue)
            {
                _vignette.smoothness.value += 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }

    public IEnumerator SetChromaticAberration(float newValue, float speed)
    {
        Volume.profile.TryGet(out _chromaticAberration);
        float startValue = _chromaticAberration.intensity.value;

        // Если меньше
        if (newValue < startValue)
        {
            while (newValue < startValue)
            {
                _chromaticAberration.intensity.value -= 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newValue > startValue)
            {
                _chromaticAberration.intensity.value += 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }

    public IEnumerator SetFilmGrain(float newValue, float speed, FilmGrainLookupParameter newType)
    {
        Volume.profile.TryGet(out _filmGrain);
        if (newType != null)
            _filmGrain.type = newType;
        float startValue = _filmGrain.intensity.value;

        // Если меньше
        if (newValue < startValue)
        {
            while (newValue < startValue)
            {
                _filmGrain.intensity.value -= 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newValue > startValue)
            {
                _filmGrain.intensity.value += 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }

    public IEnumerator SetBloom(float newValue, float speed, Color newColor = default)
    {
        Volume.profile.TryGet(out _bloom);
        if (newColor != default)
            _bloom.tint.value = newColor;
        float startValue = _bloom.intensity.value;

        // Если меньше
        if (newValue < startValue)
        {
            while (newValue < startValue)
            {
                _bloom.intensity.value -= 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newValue > startValue)
            {
                _bloom.intensity.value += 0.01f;
                newValue -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }

    public IEnumerator SetContrastColorAdjusments(float newContrast, Color newColor, float speed)
    {
        Volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        float startContrast = _colorAdjustments.contrast.value;

        // Если меньше
        if (newContrast < startContrast)
        {
            while (newContrast < startContrast)
            {
                _colorAdjustments.contrast.value -= 0.01f;
                newContrast -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newContrast > startContrast)
            {
                _colorAdjustments.contrast.value += 0.01f;
                newContrast -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }

    public IEnumerator SetSaturationColorAdjusments(float newSaturation, Color newColor, float speed)
    {
        Volume.profile.TryGet(out _colorAdjustments);
        if (newColor != default)
            _colorAdjustments.colorFilter.value = newColor;

        float startContrast = _colorAdjustments.saturation.value;

        // Если меньше
        if (newSaturation < startContrast)
        {
            while (newSaturation < startContrast)
            {
                _colorAdjustments.saturation.value -= 0.01f;
                newSaturation -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
        else // Если больше
        {
            while (newSaturation > startContrast)
            {
                _colorAdjustments.saturation.value += 0.01f;
                newSaturation -= 0.01f;
                yield return new WaitForSeconds(0.001f * speed);
            }
        }
    }
}