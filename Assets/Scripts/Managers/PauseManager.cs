using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        // Detectar tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null && GameManager.Instance.isPlaying)
            {
                if (isPaused)
                    ResumeGame();
                else
                    PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pausa el juego

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        Debug.Log("Juego pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Reactiva el juego

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Debug.Log("Juego reanudado");
    }
    public void GoToMenuFromPause()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // Desactivar panel de pausa
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Ir al menu
        if (GameManager.Instance != null)
            GameManager.Instance.ShowMainMenu();

        Debug.Log("Volviendo al menú principal desde pausa");
    }

    // Método helper para desactivar panel desde fuera
    public void HidePausePanel()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        isPaused = false;
    }
}

