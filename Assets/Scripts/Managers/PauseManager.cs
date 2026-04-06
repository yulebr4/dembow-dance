using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    [Header("UI")]
    public GameObject pauseMenuPanel;

    [Header("Estadísticas en Pausa")]
    public TextMeshProUGUI scoreText; 
    public TextMeshProUGUI comboText; 

    [Header("Ajustes de Sonido")]
    public Slider volumeSlider;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Inicializar el slider con el volumen actual al empezar
        if (volumeSlider != null && MusicManager.Instance != null)
        {
            volumeSlider.value = MusicManager.Instance.audioSource.volume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
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

        UpdatePauseStats();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        // Pausar música
        if (MusicManager.Instance != null)
            MusicManager.Instance.PauseMusic();

        Debug.Log("Juego pausado");
    }

    // Función para actualizar los datos del cuadro de arriba
    private void UpdatePauseStats()
    {
        if (GameManager.Instance != null && GameManager.Instance.scoreManager != null)
        {
            // Accedemos a través del scoreManager que tiene tu GameManager
            int puntos = GameManager.Instance.scoreManager.score;
            int combo = GameManager.Instance.scoreManager.combo;

            if (scoreText != null)
                scoreText.text = "PUNTOS: " + puntos.ToString("D5");

            if (comboText != null)
                comboText.text = "COMBO: X" + combo.ToString();
        }
    }

    // Función para el Slider del cuadro de abajo
    public void SetVolume(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.audioSource.volume = value;
            // Guardamos para que se mantenga el volumen en la siguiente partida
            PlayerPrefs.SetFloat("GlobalVolume", value);
        }
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