
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlayerController player;
    [SerializeField] private ObstaclePair obstaclePrefab;
    [SerializeField] private GameManager gameManager;

    [Header("Geração")]
    [SerializeField, Min(0.5f)] private float spawnInterval = 2f;
    [SerializeField] private float spawnX = 11f;

    [Header("Altura dos Obstáculos")]
    [SerializeField] private float minSpawnY = -1.3f;
    [SerializeField] private float maxSpawnY = 1.3f;

    private float spawnTimer;
    private bool spawningStarted = false;

    private void Update()
    {
        // Só gera obstáculos durante uma partida ativa.
        if (gameManager == null || !gameManager.IsPlaying)
            return;

        if (player == null || obstaclePrefab == null)
            return;

        // Não gera obstáculos antes do primeiro clique.
        if (!player.IsGameStarted)
            return;

        // Gera o primeiro par assim que a partida começa.
        if (!spawningStarted)
        {
            spawningStarted = true;
            SpawnObstacle();
            return;
        }

        // Controla o intervalo entre os próximos pares.
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer -= spawnInterval;
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        // Sorteia uma altura mantendo a abertura entre os canos.
        float randomY = Random.Range(minSpawnY, maxSpawnY);

        Vector3 spawnPosition = new Vector3(
            spawnX,
            randomY,
            0f
        );

        Instantiate(
            obstaclePrefab,
            spawnPosition,
            Quaternion.identity
        );
    }
}
