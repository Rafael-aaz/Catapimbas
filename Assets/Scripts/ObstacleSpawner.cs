
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ObstaclePair obstaclePrefab;
    [SerializeField] private GameManager gameManager;

    [Header("Posição de Geração")]
    [SerializeField] private float spawnX = 11f;
    [SerializeField] private float minSpawnY = -1.5f;
    [SerializeField] private float maxSpawnY = 1.5f;

    [Header("Variação de Altura")]
    [SerializeField, Min(0f)] private float maxHeightDifference = 1.8f;

    [Header("Progressão de Dificuldade")]
    [SerializeField, Min(1)] private int pointsPerLevel = 8;
    [SerializeField, Min(0)] private int maxDifficultyLevel = 4;

    [Header("Abertura Vertical")]
    [SerializeField] private float initialGap = 2.6f;
    [SerializeField] private float gapReductionPerLevel = 0.1f;
    [SerializeField] private float minimumGap = 2.2f;

    [Header("Intervalo de Geração")]
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float intervalReductionPerLevel = 0.05f;
    [SerializeField] private float minimumSpawnInterval = 1.85f;

    private float spawnTimer;
    private bool spawningStarted = false;
    private int lastLoggedLevel = -1;

    // Guarda a altura do último par criado.
    private float previousSpawnY;
    private bool hasPreviousSpawn = false;

    private void Update()
    {
        if (player == null ||
            obstaclePrefab == null ||
            gameManager == null)
        {
            return;
        }

        // Não gera obstáculos antes do início,
        // durante o Pause ou após o Game Over.
        if (!gameManager.IsPlaying)
            return;

        int currentLevel = GetDifficultyLevel();

        // Informa no Console quando a dificuldade muda.
        if (currentLevel != lastLoggedLevel)
        {
            lastLoggedLevel = currentLevel;

            Debug.Log(
                "Dificuldade nível " + currentLevel +
                " | Abertura: " + GetGap(currentLevel).ToString("F2") +
                " | Intervalo: " + GetSpawnInterval(currentLevel).ToString("F2")
            );
        }

        // Gera o primeiro par quando a partida começa.
        if (!spawningStarted)
        {
            spawningStarted = true;
            SpawnObstacle(currentLevel);
            return;
        }

        spawnTimer += Time.deltaTime;

        // O intervalo diminui conforme a dificuldade aumenta.
        float currentInterval = GetSpawnInterval(currentLevel);

        if (spawnTimer >= currentInterval)
        {
            spawnTimer -= currentInterval;
            SpawnObstacle(currentLevel);
        }
    }

    private int GetDifficultyLevel()
    {
        // A cada 8 pontos sobe um nível,
        // respeitando o limite máximo configurado.
        int level = gameManager.CurrentScore /
                    Mathf.Max(1, pointsPerLevel);

        return Mathf.Clamp(level, 0, maxDifficultyLevel);
    }

    private float GetGap(int level)
    {
        // Reduz a abertura gradualmente, mas nunca
        // abaixo do mínimo configurado.
        float gap = initialGap -
                    (level * gapReductionPerLevel);

        return Mathf.Max(minimumGap, gap);
    }

    private float GetSpawnInterval(int level)
    {
        // O primeiro aumento de dificuldade altera
        // apenas a abertura. O intervalo começa a
        // diminuir a partir do segundo nível.
        int intervalLevel = Mathf.Max(0, level - 1);

        float interval = spawnInterval -
            (intervalLevel * intervalReductionPerLevel);

        return Mathf.Max(minimumSpawnInterval, interval);
    }

    
    private void SpawnObstacle(int level)
    {
        float minY = minSpawnY;
        float maxY = maxSpawnY;

        // A partir do segundo par, limita a diferença
        // de altura em relação ao obstáculo anterior.
        if (hasPreviousSpawn)
        {
            minY = Mathf.Max(
                minY,
                previousSpawnY - maxHeightDifference
            );

            maxY = Mathf.Min(
                maxY,
                previousSpawnY + maxHeightDifference
            );
        }

        // Escolhe uma altura aleatória dentro dos limites.
        float randomY = Random.Range(minY, maxY);

        // Guarda a posição para a próxima geração.
        previousSpawnY = randomY;
        hasPreviousSpawn = true;

        Vector3 spawnPosition = new Vector3(
            spawnX,
            randomY,
            0f
        );

        ObstaclePair newObstacle = Instantiate(
            obstaclePrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Mantém a integração com a pontuação.
        newObstacle.Initialize(player, gameManager);

        // Aplica a abertura correspondente à dificuldade.
        newObstacle.SetGap(GetGap(level));
    }

}
