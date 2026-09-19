using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public float spawnX = 11f;
    public float yMin = -3.5f;
    public float yMax = 3.5f;

    [Header("Difficulty (optional)")]
    public float intervalDecreasePerSpawn = 0.05f;
    public float minInterval = 0.5f;

    private float timer = 0f;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
            spawnInterval = Mathf.Max(minInterval, spawnInterval - intervalDecreasePerSpawn);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null) return;
        float y = Random.Range(yMin, yMax);
        Instantiate(enemyPrefab, new Vector3(spawnX, y, 0f), Quaternion.identity);
    }
}
