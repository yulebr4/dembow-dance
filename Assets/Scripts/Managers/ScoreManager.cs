using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Referencias UI - Obsoletas (se mantienen por compatibilidad)")]
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
    public int comboBonusDivider = 10; // Para el bonus: 1 + combo/10

    private void Start()
    {
        // Asegurar que la UI se actualiza al inicio
        UpdateUI();
    }

    // MÉTODO PRINCIPAL - Adaptado para mantener tu lógica original
    public void AddScore(string judgement)
    {
        int basePoints = 0;
        bool breakCombo = false;

        switch (judgement)
        {
            case "Perfect":
                basePoints = perfectPoints;
                combo++;
                if (ComboFireEffect.Instance != null)
                    ComboFireEffect.Instance.OnPerfectHit(combo); // <--- Le pasamos el combo actual
                break;

            case "Good":
                basePoints = goodPoints;
                combo++;
                if (ComboFireEffect.Instance != null)
                    ComboFireEffect.Instance.OnMissOrNonPerfect();
                break;

            case "OK":
                basePoints = okPoints;
                combo++;
                if (ComboFireEffect.Instance != null)
                    ComboFireEffect.Instance.OnMissOrNonPerfect();
                break;

            case "Miss":
                breakCombo = true;
                TakeDamage(10); // Usa el nuevo método TakeDamage
                if (ComboFireEffect.Instance != null)
                    ComboFireEffect.Instance.OnMissOrNonPerfect();
                break;
        }

        if (JudgmentDisplay.Instance != null)
            JudgmentDisplay.Instance.ShowJudgment(judgement);

        // APLICAR MULTIPLICADOR DE COMBO (versión mejorada)
        if (combo > 1)
        {
            // Combinamos tu lógica original con el nuevo sistema de bonus
            int comboBonus = (combo - 1) * comboMultiplier;

            // Bonus adicional por cada 10 de combo (1 + combo/10)
            int percentageBonus = basePoints * (combo / comboBonusDivider);

            basePoints += comboBonus + percentageBonus;
        }

        score += basePoints;

        // Actualizar máximo combo
        if (combo > maxCombo)
            maxCombo = combo;

        // Romper combo si falló
        if (breakCombo)
        {
            ResetCombo();
        }

        // Actualizar UI a través del GameManager
        UpdateUI();

        Debug.Log($"Score: {score}, Combo: {combo}, Judgement: {judgement}");

        DynamicBackground dynamicBG = FindObjectOfType<DynamicBackground>();
        if (dynamicBG != null)
        {
            // Efectos especiales según el judgement
            switch (judgement)
            {
                case "Perfect":
                    // Cada 5 Perfects seguidos, efecto especial
                    if (combo % 5 == 0 && combo > 0)
                    {
                        Debug.Log($"¡PERFECT x{combo}! Efecto especial!");
                        // Aquí puedes activar un efecto de partículas o destello
                        // Por ejemplo: dynamicBG.TriggerPerfectStreak(combo);
                    }
                    break;

                case "Good":
                    // Efecto más sutil para Good
                    if (combo % 10 == 0 && combo > 0)
                    {
                        Debug.Log($"Good streak: {combo}");
                    }
                    break;

                case "Miss":
                    // Efecto cuando se pierde el combo
                    if (combo > 10) // Solo si teníamos un combo alto
                    {
                        Debug.Log($"¡Combo de {combo} roto! :(");
                        // Efecto de "tristeza" en el fondo
                    }
                    break;
            }

            // Efectos por alcanzar ciertos umbrales de puntuación
            if (score >= 1000 && score < 1010) // Justo al llegar a 1000
            {
                Debug.Log("¡Alcanzaste 1000 puntos!");
                // Efecto especial por hito de puntuación
            }
        }
    }


    // NUEVO: Método para añadir score directamente con puntos (por si lo necesitas)
    public void AddScore(int basePoints)
    {
        int points = basePoints * (1 + combo / comboBonusDivider); // Bonus por combo
        score += points;

        UpdateUI();
        Debug.Log($"Score añadido: {points}, Score total: {score}");
    }

    // NUEVO: Incrementar combo manualmente
    public void IncreaseCombo()
    {
        combo++;

        if (combo > maxCombo)
            maxCombo = combo;

        UpdateUI();
        Debug.Log($"Combo incrementado: {combo}");
    }

    // NUEVO: Resetear combo
    public void ResetCombo()
    {
        // Guardar el combo antes de resetearlo para efectos
        int oldCombo = combo;

        if (combo > maxCombo)
            maxCombo = combo;

        combo = 0;
        UpdateUI();
        Debug.Log("Combo reseteado");

        // Efecto cuando se pierde un combo alto
        if (oldCombo >= 20)
        {
            DynamicBackground dynamicBG = FindObjectOfType<DynamicBackground>();
            if (dynamicBG != null)
            {
                Debug.Log($"¡Se perdió un combo de {oldCombo}! Activando efecto de penalización");
                // Aquí podrías hacer que el fondo se oscurezca o algo similar
            }
        }
    }

    // NUEVO: Recibir daño
    public void TakeDamage(int damage)
    {
        health -= damage;
        health = Mathf.Max(0, health);

        UpdateUI();

        if (health <= 0)
        {
            Debug.Log("¡Salud cero! Game Over");
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }

    // NUEVO: Curar
    public void Heal(int amount)
    {
        health += amount;
        health = Mathf.Min(health, maxHealth);
        UpdateUI();
    }

    // NUEVO: Método para perder salud (mantiene compatibilidad con tu LoseHealth)
    private void LoseHealth(int damage)
    {
        TakeDamage(damage); // Redirige al nuevo método
    }

    // MÉTODO PRINCIPAL DE ACTUALIZACIÓN UI - Modificado para usar GameManager
    private void UpdateUI()
    {
        // ACTUALIZAR A TRAVÉS DEL GAME MANAGER (NUEVO SISTEMA)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateScore(score);
            GameManager.Instance.UpdateCombo(combo);
            GameManager.Instance.UpdateHealth(health);
        }

        // MANTENER COMPATIBILIDAD CON TEXTOS DIRECTOS (por si acaso)
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

    // NUEVO: Método para reiniciar el ScoreManager
    public void ResetScoreManager()
    {
        score = 0;
        combo = 0;
        health = maxHealth;
        maxCombo = 0;
        UpdateUI();
        Debug.Log("ScoreManager reiniciado");
    }

    // NUEVO: Método para obtener el multiplicador actual
    public int GetCurrentMultiplier()
    {
        return 1 + (combo / comboBonusDivider);
    }

    // NUEVO: Método para obtener el score con formato
    public string GetFormattedScore()
    {
        return score.ToString("N0");
    }

    // NUEVO: Método para obtener el combo con formato
    public string GetFormattedCombo()
    {
        return combo > 1 ? $"x{combo}" : "";
    }
}