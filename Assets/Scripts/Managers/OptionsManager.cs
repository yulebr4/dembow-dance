using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject leaderboardPanel;
    public GameObject controlsPanel;

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
    public GlowBorder glowAuto;


    [Header("Dificultad Automática")]
    public float autoMinSpeed = 2f;   // Velocidad inicial
    public float autoMaxSpeed = 14f;  // Velocidad máxima al final

    [Header("Scenario UI")]
    public UnityEngine.UI.Image scenarioPreviewImage;
    public TMPro.TextMeshProUGUI scenarioNameText;

    [Header("Scenario UI Multi-Slot")]
    // Cambiamos 'scenarioPreviewImage' por un array de imágenes
    public UnityEngine.UI.Image[] scenarioSlots;
    public TMPro.TextMeshProUGUI[] scenarioSlotNames;
    public Color selectedColor = Color.white;
    public Color unselectedColor = new Color(0.2f, 0.2f, 0.2f, 1f); // Oscuro


    [Header("Song Names")]
    public string[] songNames = {
        "PRENDE",
        "BUM BUM",
        "SE DEJO DEL NOVIO",
        "LO QUE PUEDA"
    };

    private float[] difficultySpeeds = { 3f, 5f, 12f, 5f };
    private int currentDifficulty;
    private int currentSongIndex;

    private Color dotActive = new Color(0.81f, 0.55f, 0.96f); // #CF8DF6
    private Color dotInactive = new Color(0.1f, 0.1f, 0.16f); // #1A1A2A


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

        UpdateScenarioUI();
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
        if (GameManager.Instance != null)
            GameManager.Instance.SetParticles(false);

        if (ScenarioManager.Instance != null)
        {
            ScenarioManager.Instance.SaveScenario();
            ScenarioManager.Instance.ApplyScenario();
        }
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

        if (diff == 3)
            return GetAutoSpeed();

        return difficultySpeeds[diff];
    }

    private float GetAutoSpeed()
    {
        if (MusicManager.Instance == null) return autoMinSpeed;

        AudioSource audio = MusicManager.Instance.audioSource;
        if (audio == null || audio.clip == null) return autoMinSpeed;

        float progress = audio.time / audio.clip.length;
        return Mathf.Lerp(autoMinSpeed, autoMaxSpeed, progress);
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
        if (glowAuto != null) glowAuto.SetActive(currentDifficulty == 3);

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

    public void NextScenario()
    {
        if (ScenarioManager.Instance == null) return;
        int next = (ScenarioManager.Instance.GetCurrentIndex() + 1) % ScenarioManager.Instance.GetCount();
        ScenarioManager.Instance.SetScenario(next);
        UpdateScenarioUI();
    }

    public void PreviousScenario()
    {
        if (ScenarioManager.Instance == null) return;
        int prev = ScenarioManager.Instance.GetCurrentIndex() - 1;
        if (prev < 0) prev = ScenarioManager.Instance.GetCount() - 1;
        ScenarioManager.Instance.SetScenario(prev);
        UpdateScenarioUI();
    }

    // Esta es la función que hace la magia
    private void UpdateScenarioUI()
    {
        if (ScenarioManager.Instance == null) return;

        int currentIndex = ScenarioManager.Instance.GetCurrentIndex();
        int totalSprites = ScenarioManager.Instance.GetCount();
        int totalNames = ScenarioManager.Instance.scenarioNames.Length;

        for (int i = 0; i < scenarioSlots.Length; i++)
        {
            if (scenarioSlots[i] == null) continue;

            if (i < totalSprites)
            {
                // Ponemos el Sprite
                scenarioSlots[i].sprite = ScenarioManager.Instance.scenarioSprites[i];

                // Ponemos el Nombre en el texto correspondiente a ese slot
                if (i < scenarioSlotNames.Length && scenarioSlotNames[i] != null && i < totalNames)
                {
                    scenarioSlotNames[i].text = ScenarioManager.Instance.scenarioNames[i];

                    // OPCIONAL: Que el texto también se oscurezca si no está seleccionado
                    scenarioSlotNames[i].color = (i == currentIndex) ? Color.white : new Color(0.5f, 0.5f, 0.5f);
                }

                scenarioSlots[i].gameObject.SetActive(true);
                scenarioSlots[i].color = (i == currentIndex) ? selectedColor : unselectedColor;
            }
            else
            {
                scenarioSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenControls()
    {
        // 1. Ocultar el menú principal
        if (GameManager.Instance != null && GameManager.Instance.mainMenuPanel != null)
        {
            GameManager.Instance.mainMenuPanel.SetActive(false);
            // 2. Apagar partículas (como pediste)
            GameManager.Instance.SetParticles(false);
        }

        if (ComboFireEffect.Instance != null)
        {
            ComboFireEffect.Instance.ResetFire();
        }

        // 3. Mostrar el panel de controles
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("¡OJO! No has arrastrado el ControlsPanel al script OptionsManager en el Inspector.");
        }
    }

    public void CloseControls()
    {
        // 1. Ocultar el panel de controles
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        // 2. Volver a mostrar el menú principal
        if (GameManager.Instance != null && GameManager.Instance.mainMenuPanel != null)
        {
            GameManager.Instance.mainMenuPanel.SetActive(true);
            // 3. Mantener partículas apagadas o encenderlas (tú decides)
            GameManager.Instance.SetParticles(false);
        }
    }
}