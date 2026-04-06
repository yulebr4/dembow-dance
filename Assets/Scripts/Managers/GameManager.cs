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
    public GameObject victoryPanel;
    public GameObject backgroundParticles;

    [Header("Scripts de Juego")]
    public NoteSpawner noteSpawner;
    public InputManager inputManager;
    public ScoreManager scoreManager;
    public GameObject GameplayBackground;
    public GameObject HitZonesContainer;

    [Header("UI Pixelada")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private PixelHealthBar healthBar;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI bestScoreGameOverText;
    [SerializeField] private TextMeshProUGUI victoryScoreText;
    [SerializeField] private TextMeshProUGUI victoryBestScoreText;

    [Header("Estado del Juego")]
    public bool isPlaying = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Revisar si venimos de un "REPETIR NIVEL"
        int isRestarting = PlayerPrefs.GetInt("IsRestarting", 0);

        if (isRestarting == 1)
        {
            // Venimos de repetir: Ocultar bandera, mostrar menú y pedir nombre
            PlayerPrefs.SetInt("IsRestarting", 0);
            ShowMainMenu();
            if (LeaderboardManager.Instance != null)
                LeaderboardManager.Instance.ShowEnterNameBeforeGame();
        }
        else
        {
            // Inicio normal o volver desde victoria: Solo menú principal limpio
            ShowMainMenu();
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        UpdateScore(0);
        UpdateCombo(0);

        if (scoreManager != null)
            UpdateHealth(scoreManager.maxHealth);
        else
            UpdateHealth(100);

        if (finalScoreText != null)
            finalScoreText.text = "PUNTAJE FINAL\n000000";
    }

    public void ShowMainMenu()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

        isPlaying = false;
        Time.timeScale = 1f;

        SetParticles(false);


        if (PauseManager.Instance != null)
            PauseManager.Instance.HidePausePanel();

        if (GameplayBackground != null)
            GameplayBackground.SetActive(false);

        if (HitZonesContainer != null)
            HitZonesContainer.SetActive(false);

        // Paneles
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        ResetDynamicBackground();

        // Parar sistemas
        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.enabled = false;
        }

        if (inputManager != null)
            inputManager.enabled = false;

        CleanupNotes();
        InitializeUI();
    }
    // Llamado desde el boton JUGAR
    public void OnPlayButtonPressed()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (LeaderboardManager.Instance != null)
            LeaderboardManager.Instance.ShowEnterNameBeforeGame();
    }

    // Llamado desde LeaderboardManager al confirmar nombre
    public void StartGame()
    {
        isPlaying = true;

        if (GameplayBackground != null)
            GameplayBackground.SetActive(true);
        if (HitZonesContainer != null)
            HitZonesContainer.SetActive(true);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (victoryPanel != null) 
            victoryPanel.SetActive(false);

        if (ComboFireEffect.Instance != null)
            ComboFireEffect.Instance.ResetFire();

        SetParticles(false);

        ResetDynamicBackground();

        if (scoreManager != null)
        {
            scoreManager.score = 0;
            scoreManager.combo = 0;
            scoreManager.health = scoreManager.maxHealth;
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

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayCurrentSong();
    }



    public void WinGame()
    {
        isPlaying = false;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

        if (ComboFireEffect.Instance != null)
            ComboFireEffect.Instance.ResetFire();

        CleanupNotes();

        Time.timeScale = 0f;

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (GameplayBackground != null)
            GameplayBackground.SetActive(false);
        if (HitZonesContainer != null)
            HitZonesContainer.SetActive(false);

        // --- NUEVA SECCIÓN DE PUNTAJE ---
        // Actualizamos los textos antes de mostrar el panel
        if (victoryScoreText != null && scoreManager != null)
            victoryScoreText.text = $"{scoreManager.score:D6}";

        int bestScore = PlayerPrefs.GetInt("Score_0", 0);
        if (victoryBestScoreText != null)
            victoryBestScoreText.text = $"{bestScore:D6}";
        // --------------------------------

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        SetParticles(true);


        // Guardar puntaje automáticamente al ganar
        if (LeaderboardManager.Instance != null && scoreManager != null)
        {
            string diff = GetCurrentDifficultyName();
            LeaderboardManager.Instance.SaveScoreAuto(scoreManager.score, diff);
        }

        Debug.Log("¡Nivel completado con éxito!");
    }

    public void GameOver()
    {
        isPlaying = false;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

      
        if (ComboFireEffect.Instance != null)
            ComboFireEffect.Instance.ResetFire();

        CleanupNotes();

        Time.timeScale = 0f;

        if (GameplayBackground != null)
            GameplayBackground.SetActive(false);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        SetParticles(true);

        if (finalScoreText != null && scoreManager != null)
            finalScoreText.text = $"{scoreManager.score:D6}";

        int bestScore = PlayerPrefs.GetInt("Score_0", 0);
        if (bestScoreGameOverText != null)
            bestScoreGameOverText.text = $"{bestScore:D6}";

        if (LeaderboardManager.Instance != null && scoreManager != null)
        {
            string diff = GetCurrentDifficultyName();
            LeaderboardManager.Instance.SaveScoreAuto(scoreManager.score, diff);
        }

        if (noteSpawner != null)
        {
            noteSpawner.StopSpawning();
            noteSpawner.enabled = false;
        }

        if (inputManager != null)
            inputManager.enabled = false;

        Debug.Log("GAME OVER!");
        Debug.Log("Particles ON GameOver");
    }

    private string GetCurrentDifficultyName()
    {
        return PlayerPrefs.GetInt("Difficulty", 1) switch
        {
            0 => "FACIL",
            2 => "DIFICIL",
            3 => "AUTOMATICA",
            _ => "NORMAL"
        };
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        // SÍ activamos la bandera para que pida nombre al recargar
        PlayerPrefs.SetInt("IsRestarting", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- NUEVO: VOLVER AL MENÚ (Desde Paneles Finales) ---
    public void VolverAlMenuDesdePanelFinal()
    {
        Time.timeScale = 1f;
        // IMPORTANTE: Asegurar que la bandera de reiniciar esté en 0
        PlayerPrefs.SetInt("IsRestarting", 0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void QuitGame()
    {
        Application.Quit();
    }

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

    private void ResetDynamicBackground()
    {
        DynamicBackground dynamicBG = FindObjectOfType<DynamicBackground>();
        if (dynamicBG != null)
            dynamicBG.ResetBackground();
    }

    public void SetParticles(bool state)
    {
        if (backgroundParticles != null)
            backgroundParticles.SetActive(state);
    }

    public void ContinuarDesdeGameOver()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (scoreManager != null)
            scoreManager.ResetScoreManager();

        StartGame();
    }
}