
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Interface de Opções")]
    [SerializeField] private GameObject optionsOverlay;

    [Header("Interface de Como Jogar")]
    [SerializeField] private GameObject howToPlayOverlay;

    [Header("Sliders de Áudio")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Chaves utilizadas para salvar as preferências de áudio.
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        // Garante que o tempo esteja normal ao entrar no menu.
        Time.timeScale = 1f;

        // Os dois pop-ups começam fechados.
        optionsOverlay.SetActive(false);
        howToPlayOverlay.SetActive(false);

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
        // Verifica se o jogador pressionou ESC.
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        // Se Como Jogar estiver aberto, ESC fecha esse pop-up.
        if (howToPlayOverlay.activeSelf)
        {
            CloseHowToPlay();
            return;
        }

        // Se Opções estiver aberto, ESC fecha as opções.
        if (optionsOverlay.activeSelf)
        {
            CloseOptions();
        }
    }

    public void PlayGame()
    {
        // Restaura o tempo antes de carregar a gameplay.
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    // Abre o pop-up de Opções.
    public void OpenOptions()
    {
        // Impede que os dois pop-ups fiquem abertos juntos.
        howToPlayOverlay.SetActive(false);

        optionsOverlay.SetActive(true);
    }

    // Fecha o pop-up de Opções.
    public void CloseOptions()
    {
        // Salva os volumes escolhidos antes de fechar.
        PlayerPrefs.Save();

        optionsOverlay.SetActive(false);
    }

    // Abre o pop-up de Como Jogar.
    public void OpenHowToPlay()
    {
        // Garante que Opções esteja fechado.
        if (optionsOverlay.activeSelf)
        {
            CloseOptions();
        }

        // Exibe o painel com as instruções do jogo.
        howToPlayOverlay.SetActive(true);
    }

    // Fecha o pop-up de Como Jogar.
    public void CloseHowToPlay()
    {
        // Esconde as instruções e retorna ao menu principal.
        howToPlayOverlay.SetActive(false);
    }

    // Salva a preferência de volume da música.
    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
    }

    // Salva a preferência de volume dos efeitos sonoros.
    private void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, value);
    }

    public void QuitGame()
    {
        // Salva as preferências antes de sair.
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
        // Remove os eventos registrados pelos sliders.
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }
}
