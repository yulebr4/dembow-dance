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
    public GameObject songInfoPanel; // Arrastra el SongInfoPanel aquí

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
        HideSongPanel(); // Ocultar al inicio
    }

    private void Update()
    {
        // 1. Actualizar barra de progreso mientras suena
        if (audioSource.isPlaying && audioSource.clip != null)
        {
            progressBarFill.fillAmount = audioSource.time / audioSource.clip.length;
        }

        // 2. DETECTAR VICTORIA (Cuando termina la canción)
        // Si el juego inició, la música no está sonando y no es porque esté en pausa
        if (gameStarted && !audioSource.isPlaying && audioSource.time == 0)
        {
            SongFinished();
        }
    }

    private void SongFinished()
    {
        gameStarted = false;
        Debug.Log("¡Canción terminada! Llamando a WinGame");

        if (GameManager.Instance != null)
        {
            // CAMBIO AQUÍ: Ahora llama a WinGame en lugar de GameOver
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
        audioSource.clip = songs[currentSongIndex];
        audioSource.Play();
        gameStarted = true;
        ShowSongPanel(); // Mostrar panel al jugar
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

