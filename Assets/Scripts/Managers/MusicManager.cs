using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip[] songs;

    [Header("Song Info")]
    public string[] songNames;
    public string[] artistNames;

    [Header("UI")]
    public Image progressBarFill;
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI artistNameText;
    public GameObject songInfoPanel;

    [Header("Audios de Interfaz (Arcade)")]
    public AudioClip menuLoop;
    public AudioClip victorySound;
    public AudioClip gameOverSound;

    private int currentSongIndex = 0;
    private bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentSongIndex = PlayerPrefs.GetInt("SongIndex", 0);
        UpdateSongInfo();
        HideSongPanel();

        // OPCIONAL: Empezar la música del menú al abrir el juego
        PlayMenuMusic();
    }

    private void Update()
    {
        if (audioSource.isPlaying && audioSource.clip != null && gameStarted)
        {
            progressBarFill.fillAmount = audioSource.time / audioSource.clip.length;
        }

        if (gameStarted && !audioSource.isPlaying && audioSource.time == 0)
        {
            SongFinished();
        }
    }

    // --- MÉTODOS DE INTERFAZ (ARCADE) ---

    public void PlayMenuMusic()
    {
        gameStarted = false;
        if (audioSource.clip == menuLoop && audioSource.isPlaying) return;

        audioSource.Stop();
        audioSource.clip = menuLoop;
        audioSource.loop = true;

        // Solo bajamos el volumen aquí
        audioSource.volume = 0.2f; // Ajusta este número (0.4f es el 40%) hasta que te guste

        audioSource.Play();
        HideSongPanel();
    }

    public void PlayVictorySound()
    {
        gameStarted = false;
        audioSource.Stop();
        audioSource.clip = victorySound;
        audioSource.loop = false; // Solo una vez
        audioSource.Play();
        HideSongPanel();
    }

    public void PlayGameOverSound()
    {
        gameStarted = false;
        audioSource.Stop();
        audioSource.clip = gameOverSound;
        audioSource.loop = false; // Solo una vez
        audioSource.Play();
        HideSongPanel();
    }

    // --- LÓGICA DE GAMEPLAY ---

    private void SongFinished()
    {
        gameStarted = false;
        Debug.Log("¡Canción terminada! Llamando a WinGame");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.WinGame();
        }
    }

    private void UpdateSongInfo()
    {
        if (songNames.Length > currentSongIndex && songNameText != null)
            songNameText.text = songNames[currentSongIndex];
        if (artistNames.Length > currentSongIndex && artistNameText != null)
            artistNameText.text = artistNames[currentSongIndex];
        if (progressBarFill != null)
            progressBarFill.fillAmount = 0f;
    }

    public void PlayCurrentSong()
    {
        if (songs.Length == 0) return;
        audioSource.Stop();

        // Volvemos a subir el volumen para que el Dembow suene con toda la fuerza
        audioSource.volume = 1.0f;

        audioSource.clip = songs[currentSongIndex];
        audioSource.loop = false;
        audioSource.Play();
        gameStarted = true;
        ShowSongPanel();
        UpdateSongInfo();
    }

    public void StopMusic()
    {
        audioSource.Stop();
        gameStarted = false;
        HideSongPanel();
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    private void ShowSongPanel()
    {
        if (songInfoPanel != null)
            songInfoPanel.SetActive(true);
    }

    private void HideSongPanel()
    {
        if (songInfoPanel != null)
            songInfoPanel.SetActive(false);
    }

    public void SetSongIndex(int index)
    {
        currentSongIndex = index;
        UpdateSongInfo();
    }

    public AudioClip GetCurrentSong() => songs[currentSongIndex];
    public int GetCurrentIndex() => currentSongIndex;
}