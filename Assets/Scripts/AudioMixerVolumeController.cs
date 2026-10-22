
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixerVolumeController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private const string MusicParameter = "MusicVolumeDb";
    private const string SFXParameter = "SFXVolumeDb";

    private void Start()
    {
        if (audioMixer == null ||
            musicSlider == null ||
            sfxSlider == null)
        {
            Debug.LogError(
                "AudioMixerVolumeController: configure todas as referências.",
                this
            );
            return;
        }

        // Recupera os volumes salvos e aplica ao Mixer.
        ApplyMusicVolume(
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f)
        );

        ApplySFXVolume(
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f)
        );

        // Atualiza o volume em tempo real ao mover os sliders.
        musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }

    private void ApplyMusicVolume(float value)
    {
        SetMixerVolume(MusicParameter, value);
    }

    private void ApplySFXVolume(float value)
    {
        SetMixerVolume(SFXParameter, value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        // Converte o valor do slider (0 a 1) em decibéis.
        // O valor zero representa silêncio.
        float decibels = value <= 0f
            ? -80f
            : Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;

        if (!audioMixer.SetFloat(parameter, decibels))
        {
            Debug.LogWarning(
                "Parâmetro não encontrado no Audio Mixer: " + parameter,
                this
            );
        }
    }

    private void OnDestroy()
    {
        // Remove os eventos registrados nos sliders.
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(ApplyMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(ApplySFXVolume);
    }
}
