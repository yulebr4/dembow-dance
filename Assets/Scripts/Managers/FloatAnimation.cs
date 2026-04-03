using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [Header("Float Settings")]
    public float amplitude = 8f;   // Qué tan alto/bajo flota
    public float speed = 1.2f;     // Qué tan rápido flota

    [Header("Glow Settings")]
    public bool enableGlow = true;
    public float glowSpeed = 2f;

    private Vector3 startPos;
    private UnityEngine.UI.Image panelImage;
    private Color baseColor;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition; // cambia esto
        panelImage = GetComponent<UnityEngine.UI.Image>();
        if (panelImage != null)
            baseColor = panelImage.color;
    }

    private void Update()
    {
        // Cambiamos Time.time por Time.unscaledTime
        float newY = startPos.y + Mathf.Sin(Time.unscaledTime * speed) * amplitude;
        rectTransform.anchoredPosition = new Vector2(startPos.x, newY);

        if (enableGlow && panelImage != null)
        {
            // También aquí para el brillo
            float glow = (Mathf.Sin(Time.unscaledTime * glowSpeed) + 1f) / 2f;
            panelImage.color = Color.Lerp(baseColor, baseColor * 1.3f, glow);
        }
    }
}