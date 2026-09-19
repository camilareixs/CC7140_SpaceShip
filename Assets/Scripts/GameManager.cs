using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    public int scoreToWin = 3;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI healthText;

    [Header("Slow Motion")]
    public float slowTimeScale = 0.3f;
    public float slowDuration = 3f;
    public float slowCooldown = 8f;
    public TextMeshProUGUI slowStatusText;
    public KeyCode slowKey = KeyCode.Q;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject victoryPanel;
    public GameObject defeatPanel;
    public TextMeshProUGUI victoryScoreText;   // texto de pontuação dentro do painel de vitória
    public TextMeshProUGUI defeatScoreText;    // texto de pontuação dentro do painel de derrota

    private int score = 0;
    private bool gameOver = false;
    private bool isSlowed = false;
    private float slowTimer = 0f;
    private float cooldownTimer = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        UpdateScoreUI();
        UpdateSlowUI();
        gameplayPanel?.SetActive(true);
        victoryPanel?.SetActive(false);
        defeatPanel?.SetActive(false);
    }

    void Update()
    {
        if (gameOver) return;
        HandleSlowMotion();
    }

    void HandleSlowMotion()
    {
        if (isSlowed)
        {
            slowTimer -= Time.unscaledDeltaTime;
            if (slowTimer <= 0f)
            {
                isSlowed = false;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
                cooldownTimer = slowCooldown;
            }
        }
        else if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
            if (cooldownTimer < 0f) cooldownTimer = 0f;
        }

        if (Input.GetKeyDown(slowKey) && !isSlowed && cooldownTimer <= 0f)
            ActivateSlowMotion();

        UpdateSlowUI();
    }

    void ActivateSlowMotion()
    {
        isSlowed = true;
        slowTimer = slowDuration;
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * slowTimeScale;
    }

    void UpdateSlowUI()
    {
        if (slowStatusText == null) return;
        if (isSlowed)
            slowStatusText.text = $"SLOW-MO: {slowTimer:F1}s";
        else if (cooldownTimer > 0f)
            slowStatusText.text = $"Recarga: {cooldownTimer:F1}s";
        else
            slowStatusText.text = $"[{slowKey}] Slow-Mo";
    }

    public void AddScore(int points)
    {
        if (gameOver) return;
        score += points;
        UpdateScoreUI();
        if (score >= scoreToWin) Victory();
    }

    public void UpdateHealthUI(int current, int max)
    {
        if (healthText == null) return;
        string hearts = "";
        for (int i = 0; i < max; i++)
            hearts += i < current ? "♥ " : "♡ ";
        healthText.text = hearts.TrimEnd();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score} / {scoreToWin}";
    }

    public void Victory()
    {
        EndGame(true);
    }

    public void Defeat()
    {
        EndGame(false);
    }

    void EndGame(bool won)
    {
        gameOver = true;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        gameplayPanel?.SetActive(false);

        if (won)
        {
            if (victoryScoreText != null)
                victoryScoreText.text = $"Pontuação: {score}";
            victoryPanel?.SetActive(true);
        }
        else
        {
            if (defeatScoreText != null)
                defeatScoreText.text = $"Pontuação: {score}";
            defeatPanel?.SetActive(true);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGameOver() => gameOver;
}
