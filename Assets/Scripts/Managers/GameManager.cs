using UnityEngine.SceneManagement;
using UnityEngine;

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
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        isPlaying = false;
        Time.timeScale = 1f;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        // Desactivar spawning
        if (noteSpawner != null)
            noteSpawner.enabled = false;

        // Desactivar input
        if (inputManager != null)
            inputManager.enabled = false;
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

        // Resetear score
        if (scoreManager != null)
        {
            scoreManager.score = 0;
            scoreManager.combo = 0;
            scoreManager.health = scoreManager.maxHealth;
        }

        // Activar spawning
        if (noteSpawner != null)
        {
            noteSpawner.enabled = true;
            // Limpiar notas viejas
            Note[] oldNotes = FindObjectsOfType<Note>();
            foreach (Note note in oldNotes)
            {
                Destroy(note.gameObject);
            }
        }

        // Activar input
        if (inputManager != null)
            inputManager.enabled = true;
    }

    public void GameOver()
    {
        isPlaying = false;

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Desactivar spawning
        if (noteSpawner != null)
            noteSpawner.enabled = false;

        // Desactivar input
        if (inputManager != null)
            inputManager.enabled = false;

        Debug.Log("GAME OVER!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
