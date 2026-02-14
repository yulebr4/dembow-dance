using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI healthText;

    [Header("Puntuación")]
    public int score = 0;
    public int combo = 0;
    public int maxCombo = 0;

    [Header("Vida")]
    public int health = 100;
    public int maxHealth = 100;

    [Header("Valores de Puntos")]
    public int perfectPoints = 300;
    public int goodPoints = 200;
    public int okPoints = 100;

    [Header("Sistema de Combo")]
    public int comboMultiplier = 10; // Puntos extra por combo

    private void Start()
    {
        UpdateUI();
    }

    public void AddScore(string judgement)
    {
        int basePoints = 0;
        bool breakCombo = false;

        switch (judgement)
        {
            case "Perfect":
                basePoints = perfectPoints;
                combo++;
                break;

            case "Good":
                basePoints = goodPoints;
                combo++;
                break;

            case "OK":
                basePoints = okPoints;
                combo++;
                break;

            case "Miss":
                breakCombo = true;
                LoseHealth(10);
                break;
        }

        // Aplicar multiplicador de combo
        if (combo > 1)
        {
            basePoints += (combo - 1) * comboMultiplier;
        }

        score += basePoints;

        // Romper combo si falló
        if (breakCombo)
        {
            if (combo > maxCombo)
                maxCombo = combo;
            combo = 0;
        }

        UpdateUI();
    }

    private void LoseHealth(int damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;

        // Game Over cuando health = 0
        if (health <= 0)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {score:N0}";

        if (comboText != null)
        {
            if (combo > 1)
                comboText.text = $"Combo: x{combo}";
            else
                comboText.text = "";
        }

        if (healthText != null)
            healthText.text = $"Vida: {health}/{maxHealth}";
    }
}
