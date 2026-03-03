using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pauseMenuPanel;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameManager.Instance != null && GameManager.Instance.isPlaying)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // Pausar música
        if (MusicManager.Instance != null)
            MusicManager.Instance.PauseMusic();

        Debug.Log("Juego pausado");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Reanudar música
        if (MusicManager.Instance != null)
            MusicManager.Instance.ResumeMusic();

        Debug.Log("Juego reanudado");
    }

    public void GoToMenuFromPause()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        // Detener música al volver al menú
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopMusic();

        if (GameManager.Instance != null)
            GameManager.Instance.ShowMainMenu();
    }

    public void HidePausePanel()
    {
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        isPaused = false;
    }
}