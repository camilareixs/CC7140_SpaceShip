using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float xMin = -8f;
    public float xMax = 8f;
    public float yMin = -4.5f;
    public float yMax = 4.5f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.15f;
    private float nextFireTime = 0f;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Invincibility")]
    public float invincibleDuration = 1.5f;
    private float invincibleTimer = 0f;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        GameManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        HandleMovement();
        HandleShooting();
        HandleInvincibility();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(h, v, 0).normalized * speed * Time.deltaTime;
        transform.position += move;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xMin, xMax);
        pos.y = Mathf.Clamp(pos.y, yMin, yMax);
        transform.position = pos;
    }

    void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null) return;
        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
    }

    void HandleInvincibility()
    {
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;
            // Blink effect
            float blink = Mathf.Sin(invincibleTimer * 20f);
            if (spriteRenderer != null)
                spriteRenderer.color = blink > 0 ? Color.white : new Color(1, 1, 1, 0.3f);

            if (invincibleTimer <= 0f && spriteRenderer != null)
                spriteRenderer.color = Color.white;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincibleTimer > 0f) return;

        currentHealth -= damage;
        invincibleTimer = invincibleDuration;
        GameManager.Instance?.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        GameManager.Instance?.Defeat();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
        else if (other.CompareTag("EnemyBullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }
}
