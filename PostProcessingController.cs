using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingController : MonoBehaviour
{
    [SerializeField] private GameObject playerCamera;
    private Vignette _vignette;
    private ChromaticAberration _chromaticAberration;
    private FilmGrain _filmGrain;
    private Bloom _bloom;
    private ColorAdjustments _colorAdjustments;
    private Volume _volume;

    public void Initialize()
    {
        _volume = playerCamera.GetComponent<Volume>();
        SetNewVignetteSmoothness(1, 2f);
        SetNewChromaticAberration(1, 0.5f);
        SetNewFilmGrain(1, 1, new FilmGrainLookupParameter(FilmGrainLookup.Large01));
    }

    public void SetVolumeProfile(VolumeProfile volumeProfile) => _volume.profile = volumeProfile;

    // Методы для изменения параметров в активновном времени

    public void SetNewVignetteSmoothness(float newValue, float speed = 1f) =>
        StartCoroutine(SetVignetteSmoothness(newValue, speed));

    public void SetNewChromaticAberration(float newIntensity, float speed = 1f) =>
        StartCoroutine(SetChromaticAberration(newIntensity, speed));

    public void SetNewFilmGrain(float newIntensity, float speed = 1f, FilmGrainLookupParameter newType = null) =>
        StartCoroutine(SetFilmGrain(newIntensity, speed, newType));

    // Отложено.
    //public void SetNewBloom(float newIntensity, float speed = 1f, Color newColor = default) => StartCoroutine()
    //public void SetNewColorAdjusments(float newContrast, float newSaturation)

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

    private IEnumerator SetFilmGrain(float newValue, float speed, FilmGrainLookupParameter newType = null)
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
}