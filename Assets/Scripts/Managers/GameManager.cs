using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Referencias")]
    public GameObject mainMenuPanel;
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

    [Header("Scripts de Juego")]
    public NoteSpawner noteSpawner;
    public InputManager inputManager;
    public ScoreManager scoreManager;

    [Header("UI Pixelada - NUEVO")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private PixelHealthBar healthBar;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Estado del Juego")]
    public bool isPlaying = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Verificar si estamos reiniciando el juego
        int isRestarting = PlayerPrefs.GetInt("IsRestarting", 0);

        if (isRestarting == 1)
        {
            // Limpiar la marca
            PlayerPrefs.SetInt("IsRestarting", 0);

            // Iniciar el juego directamente
            StartGame();
        }
        else
        {
            // Mostrar menú principal normalmente
            ShowMainMenu();
        }

        // Inicializar UI
        InitializeUI();
    }

    // NUEVO: Inicializar la UI
    private void InitializeUI()
    {
        // Configurar textos iniciales
        UpdateScore(0);
        UpdateCombo(0);

        if (scoreManager != null)
        {
            UpdateHealth(scoreManager.maxHealth);
        }
        else
        {
            UpdateHealth(100);
        }

        if (finalScoreText != null)
            finalScoreText.text = "PUNTAJE FINAL\n000000";
    }

    public void ShowMainMenu()
    {
        isPlaying = false;
        Time.timeScale = 1f;

        Debug.Log("Mostrando menú principal...");

        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.HidePausePanel();
        }

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // DETENER SPAWNER
        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.enabled = false;
            Debug.Log("Spawner detenido");
        }

        if (inputManager != null)
            inputManager.enabled = false;

        // LIMPIAR NOTAS
        CleanupNotes();

        // Resetear UI
        InitializeUI();
    }

    public void StartGame()
    {
        isPlaying = true;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (scoreManager != null)
        {
            scoreManager.score = 0;
            scoreManager.combo = 0;
            scoreManager.health = scoreManager.maxHealth;

            // Actualizar UI con valores del ScoreManager
            UpdateScore(0);
            UpdateCombo(0);
            UpdateHealth(scoreManager.maxHealth);
        }

        if (noteSpawner != null)
        {
            noteSpawner.enabled = true;
            CleanupNotes();
            noteSpawner.StartSpawning();
        }

        if (inputManager != null)
            inputManager.enabled = true;
    }

    public void GameOver()
    {
        isPlaying = false;
        Time.timeScale = 0f;

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Mostrar puntaje final
        if (finalScoreText != null && scoreManager != null)
        {
            finalScoreText.text = $"PUNTAJE FINAL\n{scoreManager.score:D6}";
        }

        // DETENER SPAWNER
        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.enabled = false;
        }

        if (inputManager != null)
            inputManager.enabled = false;

        Debug.Log("GAME OVER!");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("IsRestarting", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    // NUEVO: Métodos de actualización de UI
    public void UpdateScore(int score)
    {
        if (scoreText != null)
            scoreText.text = $"PUNTAJE\n{score:D6}";
    }

    public void UpdateCombo(int combo)
    {
        if (comboText != null)
            comboText.text = $"COMBO\nx{combo}";
    }

    public void UpdateHealth(int health)
    {
        if (healthBar != null)
            healthBar.UpdateHealth(health);
    }

    // NUEVO: Método para limpiar notas
    private void CleanupNotes()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int notasDestruidas = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj != null && (obj.name.Contains("Note") || obj.GetComponent<Note>() != null))
            {
                Destroy(obj);
                notasDestruidas++;
            }
        }

        Debug.Log($"{notasDestruidas} notas destruidas");
    }
}