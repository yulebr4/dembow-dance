using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DynamicBackground : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public GameManager gameManager;
    public ScoreManager scoreManager;

    public Color baseColor = Color.white;
    public Color colorNivel1 = new Color(1f, 0.8f, 0.3f);
    public Color colorNivel2 = new Color(1f, 0.5f, 0f);
    public Color colorNivel3 = new Color(1f, 0.2f, 0.8f);

    public int umbralNivel1 = 2500;
    public int umbralNivel2 = 5000;
    public int umbralNivel3 = 10000;

    private int lastScore = 0;

    void Start()
    {
        if (backgroundRenderer == null)
            backgroundRenderer = GetComponent<SpriteRenderer>();

        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        backgroundRenderer.color = baseColor;
    }

    void Update()
    {
        if (gameManager == null || scoreManager == null || !gameManager.isPlaying)
            return;

        int currentScore = scoreManager.score;
        UpdateColorByScore(currentScore);

        lastScore = currentScore;
    }

    void UpdateColorByScore(int score)
    {
        Color targetColor = baseColor;

        if (score >= umbralNivel3)
            targetColor = colorNivel3;
        else if (score >= umbralNivel2)
            targetColor = colorNivel2;
        else if (score >= umbralNivel1)
            targetColor = colorNivel1;

        backgroundRenderer.color = Color.Lerp(
            backgroundRenderer.color,
            targetColor,
            Time.deltaTime * 2f
        );
    }

    public void ResetBackground()
    {
        StopAllCoroutines();
        backgroundRenderer.color = baseColor;
    }
}