
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Playing,
        Paused,
        GameOver
    }

    [Header("Interface")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Textos de Pontuação")]
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private TMP_Text bestScoreText;

    [Header("Música")]
    [SerializeField] private GameMusicController gameMusicController;

    [Header("Efeitos Sonoros")]
    [SerializeField] private GameSFXController gameSFXController;

    private const string BestScoreKey = "BestScore";

    private int score = 0;
    private int bestScore = 0;

    public GameState CurrentState { get; private set; } = GameState.Ready;

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsPaused => CurrentState == GameState.Paused;
    public bool IsGameOver => CurrentState == GameState.GameOver;

    public int CurrentScore => score;

    private void Awake()
    {
        Time.timeScale = 1f;

        // Garante que o áudio não comece pausado
        // ao carregar ou reiniciar a cena.
        AudioListener.pause = false;

        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        score = 0;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        UpdateScoreUI();
    }

    public void StartGame()
    {
        if (CurrentState != GameState.Ready)
            return;

        CurrentState = GameState.Playing;

        // Começa a introdução junto com a gameplay.
        if (gameMusicController != null)
            gameMusicController.StartMusic();
    }

    public void PauseGame()
    {
        if (!IsPlaying)
            return;

        CurrentState = GameState.Paused;

        // Congela o jogo e o relógio do áudio.
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        if (!IsPaused)
            return;

        CurrentState = GameState.Playing;

        // Retoma a música exatamente de onde parou.
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }

    public void AddPoint()
    {
        if (!IsPlaying)
            return;

        score++;

        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (currentScoreText != null)
            currentScoreText.text = score.ToString();

        if (finalScoreText != null)
            finalScoreText.text = "PONTUAÇÃO: " + score;

        if (bestScoreText != null)
            bestScoreText.text = "MELHOR PONTUAÇÃO: " +
                Mathf.Max(bestScore, score);
    }

    private void SaveBestScore()
    {
        if (score <= bestScore)
            return;

        bestScore = score;

        PlayerPrefs.SetInt(BestScoreKey, bestScore);
        PlayerPrefs.Save();
    }

    
    public void EndGame()
    {
        if (!IsPlaying)
            return;

        CurrentState = GameState.GameOver;

        SaveBestScore();
        UpdateScoreUI();

        // Interrompe imediatamente a música da gameplay.
        if (gameMusicController != null)
            gameMusicController.StopMusic();

        float deathSoundDuration = 0f;

        // Reproduz o som de morte e consulta sua duração.
        if (gameSFXController != null)
        {
            gameSFXController.PlayDeath();
            deathSoundDuration = gameSFXController.DeathSoundDuration;
        }

        // Congela a gameplay imediatamente após a colisão.
        Time.timeScale = 0f;

        // O pop-up aparecerá após a duração do som de morte.
        StartCoroutine(ShowGameOverAfterDeath(deathSoundDuration));

        Debug.Log("GAME OVER! Pontuação: " + score);
    }

    
    private IEnumerator ShowGameOverAfterDeath(float delay)
    {
        // Espera em tempo real, mesmo com Time.timeScale = 0.
        if (delay > 0f)
        {
            yield return new WaitForSecondsRealtime(delay);
        }

        // Mostra a interface somente após o áudio terminar.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }



    public void RestartGame()
    {
        SaveBestScore();

        // Restaura áudio e tempo antes de recarregar.
        AudioListener.pause = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene("Game");
    }

    public void ReturnToMainMenu()
    {
        SaveBestScore();

        AudioListener.pause = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        // Evita deixar a próxima cena congelada.
        AudioListener.pause = false;
        Time.timeScale = 1f;
    }
}
