using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PixelHealthBar : MonoBehaviour
{
    [Header("Componentes UI")]
    [SerializeField] private Image healthBarFill;      // La imagen del relleno (HealthBarFill)
    [SerializeField] private Image heartIcon;          // El ícono del corazon
    [SerializeField] private RectTransform barContainer; // El contenedor de la barra (opcional para shake)

    [Header("Sprites por nivel")]
    [SerializeField] private Sprite fillHighSprite;    // health_fill_high (verde)
    [SerializeField] private Sprite fillMediumSprite;  // health_fill_medium (amarillo)
    [SerializeField] private Sprite fillLowSprite;     // health_fill_low (rojo)
    [SerializeField] private Sprite heartNormalSprite; // health_heart
    [SerializeField] private Sprite heartDangerSprite; // health_heart_danger

    [Header("Configuración")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float dangerThreshold = 0.3f;  // 30%
    [SerializeField] private float mediumThreshold = 0.6f;  // 60%
    [SerializeField] private float animationSpeed = 5f;     // Velocidad de animacion
    [SerializeField] private float shakeAmount = 5f;        // Intensidad del shake

    [Header("Audio (opcional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;
    [SerializeField] private AudioClip lowHealthSound;

    // Variables internas
    private float currentFillAmount = 1f;
    private float targetFillAmount = 1f;
    private bool isLowHealth = false;
    private Vector3 originalBarPosition;
    private bool damageFlashTriggered = false;


    void Start()
    {
        // Guardar posicion original para el shake
        if (barContainer != null)
            originalBarPosition = barContainer.localPosition;

        // Inicializar
        if (healthBarFill != null)
        {
            // Asegurar que sea tipo Filled
            healthBarFill.type = Image.Type.Filled;
            healthBarFill.fillMethod = Image.FillMethod.Horizontal;
            healthBarFill.fillOrigin = 0; // Left
            currentFillAmount = 1f;
            targetFillAmount = 1f;
            healthBarFill.fillAmount = 1f;
        }

        // Buscar ScoreManager automaticamente
        StartCoroutine(FindScoreManager());
    }

    IEnumerator FindScoreManager()
    {
        // Esperar un frame para asegurar que todo este cargado
        yield return null;

        // Buscar el ScoreManager en la escena
        ScoreManager scoreManager = FindObjectOfType<ScoreManager>();
        if (scoreManager != null)
        {
            Debug.Log("PixelHealthBar: ScoreManager encontrado!");
            // Suscribirse al evento de daño (si existe)
            // Por ahora, actualizamos cada frame
        }
        else
        {
            Debug.LogWarning("PixelHealthBar: No se encontró ScoreManager. La barra no se actualizará automáticamente.");
        }
    }

    void Update()
    {
        // Suavizar la animacion de la barra
        if (healthBarFill != null && !Mathf.Approximately(currentFillAmount, targetFillAmount))
        {
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);
            healthBarFill.fillAmount = currentFillAmount;
            UpdateHealthAppearance(currentFillAmount);
        }

        // Buscar ScoreManager cada frame como respaldo
        if (GameObject.FindObjectOfType<ScoreManager>() != null)
        {
            ScoreManager sm = GameObject.FindObjectOfType<ScoreManager>();
            int health = (int)typeof(ScoreManager).GetField("health")?.GetValue(sm);
            if (health > 0)
                UpdateHealth(health);
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthBarFill == null) return;

        float healthPercent = (float)currentHealth / maxHealth;
        float newTarget = Mathf.Clamp01(healthPercent);

        // Solo disparar flash si realmente bajó la vida
        if (newTarget < targetFillAmount && !damageFlashTriggered)
        {
            damageFlashTriggered = true;
            OnDamageReceived();
            Invoke(nameof(ResetDamageFlag), 0.5f);
        }

        targetFillAmount = newTarget;
    }

    private void ResetDamageFlag()
    {
        damageFlashTriggered = false;
    }

    // Metodo para cuando recibe daño
    public void OnDamageReceived()
    {
        // Efecto de shake (ya tienes esto)
        if (barContainer != null)
            StartCoroutine(ShakeBar());

        // Reproducir sonido de daño (ya tienes esto)
        if (audioSource != null && damageSound != null)
            audioSource.PlayOneShot(damageSound);

        // NUEVO: Flash en el fondo
        DynamicBackground dynBG = FindObjectOfType<DynamicBackground>();
        if (dynBG != null)
            dynBG.TriggerDamageFlash();
    }

    // Efecto de shake
    IEnumerator ShakeBar()
    {
        float elapsed = 0f;
        float duration = 0.2f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeAmount;
            float y = Random.Range(-1f, 1f) * shakeAmount;

            barContainer.localPosition = originalBarPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        barContainer.localPosition = originalBarPosition;
    }

    // Actualizar sprites segun nivel de vida
    void UpdateHealthAppearance(float healthPercent)
    {
        // Actualizar sprite del fill
        if (healthBarFill != null)
        {
            if (healthPercent <= dangerThreshold)
            {
                healthBarFill.sprite = fillLowSprite;
                if (fillLowSprite != null) healthBarFill.color = Color.white;

                // Corazon en peligro
                if (heartIcon != null && heartDangerSprite != null)
                {
                    heartIcon.sprite = heartDangerSprite;

                    // Latido rapido cuando esta en peligro
                    if (!isLowHealth)
                    {
                        isLowHealth = true;
                        StartCoroutine(HeartbeatEffect());
                    }
                }
            }
            else if (healthPercent <= mediumThreshold)
            {
                healthBarFill.sprite = fillMediumSprite;
                if (fillMediumSprite != null) healthBarFill.color = Color.white;
                isLowHealth = false;

                // Restaurar corazon normal
                if (heartIcon != null && heartNormalSprite != null)
                    heartIcon.sprite = heartNormalSprite;
            }
            else
            {
                healthBarFill.sprite = fillHighSprite;
                if (fillHighSprite != null) healthBarFill.color = Color.white;
                isLowHealth = false;

                // Restaurar corazon normal
                if (heartIcon != null && heartNormalSprite != null)
                    heartIcon.sprite = heartNormalSprite;
            }
        }
    }

    // Efecto de latido para el corazón (VERSIÓN CORREGIDA - sin crecimiento excesivo)
    IEnumerator HeartbeatEffect()
    {
        // Guardar la escala original UNA VEZ al inicio
        Vector3 originalScale = heartIcon.transform.localScale;
        float pulseSize = 0.1f; // 10% de aumento (mucho más pequeño)
        float pulseSpeed = 8f;   // Velocidad del latido

        while (isLowHealth && heartIcon != null)
        {
            // Escala más grande (solo 10% más grande, no 20%)
            Vector3 targetScale = originalScale * (1f + pulseSize);

            // Agrandar suavemente
            float t = 0;
            while (t < 1f && isLowHealth)
            {
                t += Time.deltaTime * pulseSpeed;
                heartIcon.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
                yield return null;
            }

            // Encoger suavemente
            t = 0;
            while (t < 1f && isLowHealth)
            {
                t += Time.deltaTime * pulseSpeed;
                heartIcon.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
                yield return null;
            }

            // Pequeña pausa entre latidos
            yield return new WaitForSeconds(0.2f);
        }

        // Restaurar escala original suavemente
        if (heartIcon != null)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 5f;
                heartIcon.transform.localScale = Vector3.Lerp(heartIcon.transform.localScale, originalScale, t);
                yield return null;
            }
            heartIcon.transform.localScale = originalScale;
        }
    }

    // Metodo para conectar con GameManager
    public void ConnectToGameManager()
    {
        // Este metodo puede ser llamado desde GameManager
        Debug.Log("PixelHealthBar conectada al GameManager");
    }
}