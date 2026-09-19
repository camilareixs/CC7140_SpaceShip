using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int health = 1;
    public int scoreValue = 1;
    public float speed = 2.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float fireRate = 2f;
    public float firstShotDelay = 1f;
    private float nextFireTime;

    void Start()
    {
        nextFireTime = Time.time + firstShotDelay + Random.Range(0f, fireRate);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        // Move left
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Shoot
        if (bulletPrefab != null && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate + Random.Range(-0.3f, 0.3f);
            Shoot();
        }

        // Destroy if off screen
        if (transform.position.x < -12f)
            Destroy(gameObject);
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, transform.position, Quaternion.identity);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance?.AddScore(scoreValue);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>()?.TakeDamage(1);
            Die();
        }
    }
}
