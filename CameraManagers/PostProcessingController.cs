using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    public static PostProcessingController Instance { get; private set; }
    [SerializeField] private GameObject playerCamera;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private FilmGrain _filmGrain;
    private Bloom _bloom;
    private ColorAdjustments _colorAdjustments;
    private Volume _volume;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _volume = playerCamera.GetComponent<Volume>();
    }

    public void SetVolumeProfile(VolumeProfile volumeProfile) => _volume.profile = volumeProfile;

    // Методы для изменения параметров в активновном времени

    public void SetNewVignetteSmoothness(float newValue, float speed = 1f) =>
        StartCoroutine(SetVignetteSmoothness(newValue, speed));

    public void SetNewChromaticAberration(float newIntensity, float speed = 1f) =>
        StartCoroutine(SetChromaticAberration(newIntensity, speed));

    public void SetNewFilmGrain(float newIntensity, float speed = 1f, FilmGrainLookupParameter newType = null) =>
        StartCoroutine(SetFilmGrain(newIntensity, speed, newType));

    public void SetNewBloom(float newIntensity, float speed = 1f, Color newColor = default) =>
        StartCoroutine(SetBloom(newIntensity, speed, newColor));

    public void SetNewContrastColorAdjusments(float speed = 1f, float newContrast = 0f, Color newColor = default) =>
        StartCoroutine(SetContrastColorAdjusments(newContrast, newColor, speed));
    
    public void SetNewSaturationColorAdjusments(float speed = 1f, float newSaturation = 0f, Color newColor = default) =>
        StartCoroutine(SetSaturationColorAdjusments(newSaturation, newColor, speed));
    
    private IEnumerator SetVignetteSmoothness(float newValue, float speed)
    {
        _volume.profile.TryGet(out _vignette);
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
    
    private IEnumerator SetChromaticAberration(float newValue, float speed)
    {
        _volume.profile.TryGet(out _chromaticAberration);
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
    
    private IEnumerator SetFilmGrain(float newValue, float speed, FilmGrainLookupParameter newType)
    {
        _volume.profile.TryGet(out _filmGrain);
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
    
    private IEnumerator SetBloom(float newValue, float speed, Color newColor = default)
    {
        _volume.profile.TryGet(out _bloom);
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
    
    private IEnumerator SetContrastColorAdjusments(float newContrast, Color newColor, float speed)
    {
        _volume.profile.TryGet(out _colorAdjustments);
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
    
    private IEnumerator SetSaturationColorAdjusments(float newSaturation, Color newColor, float speed)
    {
        _volume.profile.TryGet(out _colorAdjustments);
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