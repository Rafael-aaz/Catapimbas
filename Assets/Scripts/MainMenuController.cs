
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Interface de Opções")]
    [SerializeField] private GameObject optionsOverlay;

    [Header("Sliders de Áudio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        Time.timeScale = 1f;

        // O pop-up começa fechado.
        optionsOverlay.SetActive(false);

        // Recupera os volumes salvos anteriormente.
        musicSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f)
        );

        sfxSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f)
        );

        // Detecta automaticamente mudanças nos sliders.
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Update()
    {
        // ESC fecha as opções somente quando estão abertas.
        if (optionsOverlay.activeSelf &&
            Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseOptions();
        }
    }

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void OpenOptions()
    {
        optionsOverlay.SetActive(true);
    }

    public void CloseOptions()
    {
        // Salva os volumes escolhidos antes de fechar.
        PlayerPrefs.Save();

        optionsOverlay.SetActive(false);
    }

    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    private void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();

        // No Editor, apenas informa a tentativa de sair.
        // Na build, fecha o jogo.
#if UNITY_EDITOR
        Debug.Log("SAIR: o jogo será fechado na build.");
#else
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // Remove os eventos registrados pelo script.
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
