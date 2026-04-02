using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DynamicBackground : MonoBehaviour
{
    [Header("Referencias")]
    public SpriteRenderer backgroundRenderer;
    public GameManager gameManager;
    public ScoreManager scoreManager;
    public AudioSource musicSource;

    [Header("Overlays UI")]
    public Image damageFlash;
    public Image beatPulse;

    [Header("Colores por Score")]
    public Color baseColor = Color.white;
    public Color colorNivel1 = new Color(1f, 0.9f, 0.4f);
    public Color colorNivel2 = new Color(1f, 0.4f, 0.1f);
    public Color colorNivel3 = new Color(0.8f, 0.1f, 1f);
    public int umbralNivel1 = 2500;
    public int umbralNivel2 = 5000;
    public int umbralNivel3 = 10000;

    [Header("Beat Pulse")]
    public float beatSensitivity = 0.1f;
    public float pulseSpeed = 8f;
    public float maxPulseAlpha = 0.15f;

    [Header("Damage Flash")]
    public float flashDuration = 0.3f;
    public float flashMaxAlpha = 0.4f;

    // Audio
    private float[] audioSamples = new float[256];
    private float lastAudioLevel = 0f;
    private float currentBeatAlpha = 0f;

    // Colores del beat por nivel
    private Color beatColorNivel0 = new Color(0.5f, 0f, 1f);
    private Color beatColorNivel1 = new Color(1f, 0.6f, 0f);
    private Color beatColorNivel2 = new Color(1f, 0.1f, 0.5f);
    private Color beatColorNivel3 = new Color(0f, 1f, 0.8f);

    // Control interno
    private bool isFlashing = false;
    private bool isTransitioning = false;
    private int currentLevel = 0;

    void Start()
    {
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<SpriteRenderer>();
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();
        if (musicSource == null && MusicManager.Instance != null)
            musicSource = MusicManager.Instance.GetComponent<AudioSource>();

        backgroundRenderer.color = baseColor;
        currentLevel = 0;

        if (damageFlash != null) { Color c = damageFlash.color; c.a = 0f; damageFlash.color = c; }
        if (beatPulse != null) { Color c = beatPulse.color; c.a = 0f; beatPulse.color = c; }
    }

    void Update()
    {
        if (gameManager == null || scoreManager == null || !gameManager.isPlaying)
            return;

        CheckLevelUp(scoreManager.score);
        UpdateBeatPulse(scoreManager.score);
    }

    void CheckLevelUp(int score)
    {
        int newLevel = 0;
        if (score >= umbralNivel3) newLevel = 3;
        else if (score >= umbralNivel2) newLevel = 2;
        else if (score >= umbralNivel1) newLevel = 1;

        if (newLevel > currentLevel && !isTransitioning)
        {
            currentLevel = newLevel;
            StartCoroutine(LevelUpTransition(newLevel));
        }
    }

    private IEnumerator LevelUpTransition(int level)
    {
        isTransitioning = true;

        Color targetColor = baseColor;
        if (level == 1) targetColor = colorNivel1;
        else if (level == 2) targetColor = colorNivel2;
        else if (level == 3) targetColor = colorNivel3;

        if (level == 1)
        {
            // Nivel 1: Flash amarillo suave + cambio directo
            yield return StartCoroutine(PulseOverlay(beatPulse, new Color(1f, 1f, 0f), 0.5f, 0.5f));
            backgroundRenderer.color = targetColor;
        }
        else if (level == 2)
        {
            // Nivel 2: 3 flashes rapidos naranjas + cambio
            for (int i = 0; i < 3; i++)
            {
                yield return StartCoroutine(PulseOverlay(beatPulse, new Color(1f, 0.5f, 0f), 0.15f, 0.1f));
            }
            backgroundRenderer.color = targetColor;
        }
        else if (level == 3)
        {
            // Nivel 3: Flash blanco explosivo + cambio a purpura
            yield return StartCoroutine(PulseOverlay(beatPulse, new Color(1f, 1f, 1f), 0.8f, 0.8f));
            backgroundRenderer.color = targetColor;
            // Segundo flash de color
            yield return StartCoroutine(PulseOverlay(beatPulse, new Color(0.8f, 0f, 1f), 0.5f, 0.4f));
        }

        isTransitioning = false;
    }

    private IEnumerator PulseOverlay(Image overlay, Color color, float maxAlpha, float duration)
    {
        if (overlay == null) yield break;

        float elapsed = 0f;

        // Aparecer rapido (30% del tiempo)
        while (elapsed < duration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, maxAlpha, elapsed / (duration * 0.3f));
            color.a = alpha;
            overlay.color = color;
            yield return null;
        }

        elapsed = 0f;

        // Desaparecer lento (70% del tiempo)
        while (elapsed < duration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, elapsed / (duration * 0.7f));
            color.a = alpha;
            overlay.color = color;
            yield return null;
        }

        color.a = 0f;
        overlay.color = color;
    }

    void UpdateBeatPulse(int score)
    {
        if (beatPulse == null || musicSource == null || !musicSource.isPlaying || isTransitioning)
            return;

        musicSource.GetOutputData(audioSamples, 0);
        float audioLevel = 0f;
        foreach (float sample in audioSamples)
            audioLevel += Mathf.Abs(sample);
        audioLevel /= audioSamples.Length;

        float delta = audioLevel - lastAudioLevel;
        if (delta > beatSensitivity)
            currentBeatAlpha = maxPulseAlpha;

        lastAudioLevel = audioLevel;

        currentBeatAlpha = Mathf.Lerp(currentBeatAlpha, 0f, Time.deltaTime * pulseSpeed);

        Color pulseColor = beatColorNivel0;
        if (score >= umbralNivel3) pulseColor = beatColorNivel3;
        else if (score >= umbralNivel2) pulseColor = beatColorNivel2;
        else if (score >= umbralNivel1) pulseColor = beatColorNivel1;

        pulseColor.a = currentBeatAlpha;
        beatPulse.color = pulseColor;
    }

    public void TriggerDamageFlash()
    {
        if (!isFlashing && damageFlash != null)
            StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        float elapsed = 0f;

        while (elapsed < flashDuration * 0.3f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, flashMaxAlpha, elapsed / (flashDuration * 0.3f));
            Color c = damageFlash.color;
            c.a = alpha;
            damageFlash.color = c;
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < flashDuration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(flashMaxAlpha, 0f, elapsed / (flashDuration * 0.7f));
            Color c = damageFlash.color;
            c.a = alpha;
            damageFlash.color = c;
            yield return null;
        }

        Color final = damageFlash.color;
        final.a = 0f;
        damageFlash.color = final;
        isFlashing = false;
    }

    public void ResetBackground()
    {
        StopAllCoroutines();
        backgroundRenderer.color = baseColor;
        isFlashing = false;
        isTransitioning = false;
        currentBeatAlpha = 0f;
        currentLevel = 0;

        if (damageFlash != null) { Color c = damageFlash.color; c.a = 0f; damageFlash.color = c; }
        if (beatPulse != null) { Color c = beatPulse.color; c.a = 0f; beatPulse.color = c; }
    }
}



