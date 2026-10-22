
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private GameManager gameManager;

    [Header("Interface")]
    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private GameObject pausePopup;
    [SerializeField] private GameObject pauseOptionsPopup;

    [Header("Áudio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        // Evita iniciar o sistema com referências ausentes.
        if (gameManager == null ||
            pauseOverlay == null ||
            pausePopup == null ||
            pauseOptionsPopup == null ||
            musicSlider == null ||
            sfxSlider == null)
        {
            Debug.LogError(
                "PauseController: configure todas as referências no Inspector.",
                this
            );

            enabled = false;
            return;
        }

        // Garante que os painéis comecem na configuração correta.
        pausePopup.SetActive(true);
        pauseOptionsPopup.SetActive(false);
        pauseOverlay.SetActive(false);

        // Recupera os mesmos volumes usados pelo Menu Inicial.
        musicSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(MusicVolumeKey, 1f)
        );

        sfxSlider.SetValueWithoutNotify(
            PlayerPrefs.GetFloat(SFXVolumeKey, 1f)
        );

        // Registra as alterações dos sliders.
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        // Dentro das Opções, ESC retorna ao Pause.
        if (gameManager.IsPaused &&
            pauseOptionsPopup.activeSelf)
        {
            BackToPause();
            return;
        }

        // ESC no menu de Pause continua a partida.
        if (gameManager.IsPaused)
        {
            ContinueGame();
            return;
        }

        // Só abre o Pause durante a gameplay.
        if (gameManager.IsPlaying)
        {
            OpenPause();
        }
    }

    public void OpenPause()
    {
        if (!gameManager.IsPlaying)
            return;

        gameManager.PauseGame();

        pausePopup.SetActive(true);
        pauseOptionsPopup.SetActive(false);
        pauseOverlay.SetActive(true);
    }

    public void ContinueGame()
    {
        if (!gameManager.IsPaused)
            return;

        pauseOverlay.SetActive(false);

        gameManager.ResumeGame();
    }

    public void OpenOptions()
    {
        if (!gameManager.IsPaused)
            return;

        // Troca a janela principal pela janela de Opções.
        pausePopup.SetActive(false);
        pauseOptionsPopup.SetActive(true);
    }

    public void BackToPause()
    {
        if (!gameManager.IsPaused)
            return;

        PlayerPrefs.Save();

        pauseOptionsPopup.SetActive(false);
        pausePopup.SetActive(true);
    }

    public void RestartGame()
    {
        PlayerPrefs.Save();

        // Reutiliza o reinício que já existe no GameManager.
        gameManager.RestartGame();
    }

    public void ReturnToMainMenu()
    {
        PlayerPrefs.Save();

        gameManager.ReturnToMainMenu();
    }

    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    private void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    private void OnDestroy()
    {
        // Remove os eventos registrados nos sliders.
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
