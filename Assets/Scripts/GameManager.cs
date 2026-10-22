
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Ready,
        Playing,
        GameOver
    }

    public GameState CurrentState { get; private set; } = GameState.Ready;

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsGameOver => CurrentState == GameState.GameOver;

    private void Awake()
    {
        // Garante que uma nova partida comece sem estar pausada.
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        if (CurrentState != GameState.Ready)
            return;

        CurrentState = GameState.Playing;
    }

    public void EndGame()
    {
        if (CurrentState != GameState.Playing)
            return;

        CurrentState = GameState.GameOver;

        // Congela a física e a movimentação dos obstáculos.
        Time.timeScale = 0f;

        Debug.Log("GAME OVER!");
    }

    private void OnDestroy()
    {
        // Evita deixar o tempo congelado ao sair da cena.
        Time.timeScale = 1f;
    }
}
