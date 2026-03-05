using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;

    [Header("Panels")]
    public GameObject leaderboardPanel;
    public GameObject enterNamePanel;  // Panel con imagen panel_ingresarnombre

    [Header("UI - Enter Name")]
    public TMP_InputField nameInputField;

    [Header("UI - Leaderboard")]
    public Transform contentParent;
    public GameObject entryPrefab;
    public TextMeshProUGUI emptyText;

    [Header("Settings")]
    public int maxEntries = 10;

    // Nombre guardado del jugador actual
    private string currentPlayerName = "JUGADOR";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
        if (enterNamePanel != null)
            enterNamePanel.SetActive(false);

        // Cargar nombre guardado si existe
        currentPlayerName = PlayerPrefs.GetString("PlayerName", "JUGADOR");
    }

    // =============================================
    // FLUJO: JUGAR -> INGRESAR NOMBRE -> JUEGO
    // =============================================

    // Llamado desde boton JUGAR
    public void ShowEnterNameBeforeGame()
    {
        if (enterNamePanel != null)
            enterNamePanel.SetActive(true);

        // Pre-cargar nombre anterior si existe
        if (nameInputField != null)
        {
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            nameInputField.text = savedName;
        }
    }

    // Llamado desde boton GUARDAR del panel de nombre
    public void ConfirmNameAndPlay()
    {
        // Guardar nombre
        if (nameInputField != null && nameInputField.text.Trim() != "")
            currentPlayerName = nameInputField.text.Trim().ToUpper();
        else
            currentPlayerName = "JUGADOR";

        // Guardar para proximas veces
        PlayerPrefs.SetString("PlayerName", currentPlayerName);
        PlayerPrefs.Save();

        // Cerrar panel de nombre
        if (enterNamePanel != null)
            enterNamePanel.SetActive(false);

        // Iniciar juego
        if (GameManager.Instance != null)
            GameManager.Instance.StartGame();
    }

    // Llamado desde boton CANCELAR del panel de nombre
    public void CancelEnterName()
    {
        if (enterNamePanel != null)
            enterNamePanel.SetActive(false);

        // Volver al menu principal
        if (GameManager.Instance != null)
            GameManager.Instance.ShowMainMenu();
    }

    // =============================================
    // FLUJO: GAME OVER -> GUARDAR AUTOMATICO
    // =============================================

    // Llamado automaticamente desde GameOver
    public void SaveScoreAuto(int score, string difficulty)
    {
        AddScore(score, currentPlayerName, difficulty);
        Debug.Log($"Puntaje guardado: {currentPlayerName} - {score} pts - {difficulty}");
    }

    private void AddScore(int score, string playerName, string difficulty)
    {
        for (int i = 0; i < maxEntries; i++)
        {
            int saved = PlayerPrefs.GetInt($"Score_{i}", -1);

            if (saved == -1 || score > saved)
            {
                for (int j = maxEntries - 1; j > i; j--)
                {
                    PlayerPrefs.SetInt($"Score_{j}",
                        PlayerPrefs.GetInt($"Score_{j - 1}", -1));
                    PlayerPrefs.SetString($"ScoreName_{j}",
                        PlayerPrefs.GetString($"ScoreName_{j - 1}", ""));
                    PlayerPrefs.SetString($"ScoreDiff_{j}",
                        PlayerPrefs.GetString($"ScoreDiff_{j - 1}", ""));
                }

                PlayerPrefs.SetInt($"Score_{i}", score);
                PlayerPrefs.SetString($"ScoreName_{i}", playerName);
                PlayerPrefs.SetString($"ScoreDiff_{i}", difficulty);
                PlayerPrefs.Save();
                break;
            }
        }
    }

    // =============================================
    // FLUJO: VER TABLA DESDE OPCIONES
    // =============================================

    public void OpenLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(true);

        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        bool hasEntries = false;

        for (int i = 0; i < maxEntries; i++)
        {
            int score = PlayerPrefs.GetInt($"Score_{i}", -1);
            if (score == -1) break;

            hasEntries = true;
            string name = PlayerPrefs.GetString($"ScoreName_{i}", "JUGADOR");
            string diff = PlayerPrefs.GetString($"ScoreDiff_{i}", "NORMAL");

            GameObject entry = Instantiate(entryPrefab, contentParent);
            TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length >= 3)
            {
                texts[0].text = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"#{i + 1}";
                texts[1].text = name;
                texts[2].text = $"{score:D6}";
            }

            Image bg = entry.GetComponent<Image>();
            if (bg != null)
            {
                if (i == 0) bg.color = new Color(1f, 0.84f, 0f, 0.25f);
                else if (i == 1) bg.color = new Color(0.75f, 0.75f, 0.75f, 0.25f);
                else if (i == 2) bg.color = new Color(0.8f, 0.5f, 0.2f, 0.25f);
                else bg.color = new Color(0.1f, 0.1f, 0.2f, 0.4f);
            }
        }

        if (emptyText != null)
            emptyText.gameObject.SetActive(!hasEntries);
    }

    public void CloseLeaderboard()
    {
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);

        // Volver a opciones
        if (OptionsManager.Instance != null)
            OptionsManager.Instance.OpenOptions();
    }

    public void ClearLeaderboard()
    {
        for (int i = 0; i < maxEntries; i++)
        {
            PlayerPrefs.DeleteKey($"Score_{i}");
            PlayerPrefs.DeleteKey($"ScoreName_{i}");
            PlayerPrefs.DeleteKey($"ScoreDiff_{i}");
        }
        PlayerPrefs.Save();
        LoadLeaderboard();
        Debug.Log("Records borrados");
    }
}