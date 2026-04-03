using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject leaderboardPanel;

    [Header("Difficulty Buttons")]
    public Button easyButton;
    public Button normalButton;
    public Button hardButton;

    [Header("Song UI")]
    public TextMeshProUGUI songNameText;  // SongNameText
    public TextMeshProUGUI trackText;     // TrackText (PISTA 1/4)
    public Image[] dots;                  // Los 4 cuadritos

    [Header("Glow Borders")]
    public GlowBorder glowEasy;
    public GlowBorder glowNormal;
    public GlowBorder glowHard;

    [Header("Song Names")]
    public string[] songNames = {
        "PRENDE",
        "BUM BUM",
        "SE DEJO DEL NOVIO",
        "LO QUE PUEDA"
    };

    private float[] difficultySpeeds = { 3f, 5f, 8f };
    private int currentDifficulty;
    private int currentSongIndex;

    private Color dotActive = new Color(0.81f, 0.55f, 0.96f); // #CF8DF6
    private Color dotInactive = new Color(0.1f, 0.1f, 0.16f); // #1A1A2A
    private Color selectedColor = new Color(0f, 1f, 0.5f);
    private Color unselectedColor = new Color(0.2f, 0.2f, 0.2f);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        currentSongIndex = PlayerPrefs.GetInt("SongIndex", 0);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        UpdateDifficultyButtons();
        UpdateSongUI();
    }

    public void OpenOptions()
    {
        currentDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        currentSongIndex = PlayerPrefs.GetInt("SongIndex", 0);
        UpdateDifficultyButtons();
        UpdateSongUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.mainMenuPanel.SetActive(false);
            GameManager.Instance.SetParticles(true); 
        }

        if (optionsPanel != null)
            optionsPanel.SetActive(true);
    }

    public void CancelOptions()
    {
        currentDifficulty = PlayerPrefs.GetInt("Difficulty", 1);
        currentSongIndex = PlayerPrefs.GetInt("SongIndex", 0);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.mainMenuPanel.SetActive(true);
            GameManager.Instance.SetParticles(false); 
        }
    }

    public void SaveOptions()
    {
        PlayerPrefs.SetInt("Difficulty", currentDifficulty);
        PlayerPrefs.SetInt("SongIndex", currentSongIndex);
        PlayerPrefs.Save();

        if (MusicManager.Instance != null)
            MusicManager.Instance.SetSongIndex(currentSongIndex);

        if (optionsPanel != null)
            optionsPanel.SetActive(false);
        if (GameManager.Instance != null)
            GameManager.Instance.mainMenuPanel.SetActive(true);
    }

    public void SetDifficulty(int level)
    {
        currentDifficulty = level;
        UpdateDifficultyButtons();
    }

    public void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songNames.Length;
        UpdateSongUI();
    }

    public void PreviousSong()
    {
        currentSongIndex--;
        if (currentSongIndex < 0) currentSongIndex = songNames.Length - 1;
        UpdateSongUI();
    }

    public void OpenLeaderboard()
    {
        // 1. Cerramos el panel de opciones (aquí sí existe la variable)
        if (optionsPanel != null)
            optionsPanel.SetActive(false);

        // 2. Apagamos partículas
        if (GameManager.Instance != null)
            GameManager.Instance.SetParticles(false);

        // 3. Llamamos a la tabla
        if (LeaderboardManager.Instance != null)
            LeaderboardManager.Instance.leaderboardPanel.SetActive(true);
        LeaderboardManager.Instance.LoadLeaderboard();
    }

    public float GetNoteSpeed()
    {
        int diff = PlayerPrefs.GetInt("Difficulty", 1);
        return difficultySpeeds[diff];
    }

    private void UpdateSongUI()
    {
        // Actualizar nombre
        if (songNameText != null)
            songNameText.text = songNames[currentSongIndex];

        // Actualizar PISTA X/4
        if (trackText != null)
            trackText.text = $"PISTA {currentSongIndex + 1} / {songNames.Length}";

        // Actualizar dots
        if (dots != null)
        {
            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i] != null)
                    dots[i].color = i == currentSongIndex ? dotActive : dotInactive;
            }
        }
    }

    private void UpdateDifficultyButtons()
    {
        if (glowEasy != null) glowEasy.SetActive(currentDifficulty == 0);
        if (glowNormal != null) glowNormal.SetActive(currentDifficulty == 1);
        if (glowHard != null) glowHard.SetActive(currentDifficulty == 2);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");

        // Si estás ejecutando el juego desde el editor de Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Si el juego ya está compilado (.exe o .apk)
        Application.Quit();
#endif
    }
}