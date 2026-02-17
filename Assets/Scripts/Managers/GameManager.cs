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
            noteSpawner.StopSpawning(); // Usar el nuevo método
            noteSpawner.enabled = false;
            Debug.Log("Spawner detenido");
        }

        if (inputManager != null)
            inputManager.enabled = false;

        // LIMPIAR NOTAS
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
        Debug.Log("Menú principal listo");
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
        }

        if (noteSpawner != null)
        {
            noteSpawner.enabled = true;

            // Limpiar notas viejas
            Note[] oldNotes = FindObjectsOfType<Note>();
            foreach (Note note in oldNotes)
            {
                Destroy(note.gameObject);
            }

            // INICIAR SPAWNING
            noteSpawner.StartSpawning();
        }

        if (inputManager != null)
            inputManager.enabled = true;
    }

    public void GameOver()
    {
        isPlaying = false;
        Time.timeScale = 0f; // Pausar juego

        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

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
        PlayerPrefs.SetInt("IsRestarting", 1); // Marca que estamos reiniciando
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
