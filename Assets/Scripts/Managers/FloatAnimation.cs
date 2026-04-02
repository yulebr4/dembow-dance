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
        float newY = startPos.y + Mathf.Sin(Time.time * speed) * amplitude;
        rectTransform.anchoredPosition = new Vector2(startPos.x, newY); // y esto

        if (enableGlow && panelImage != null)
        {
            float glow = (Mathf.Sin(Time.time * glowSpeed) + 1f) / 2f;
            panelImage.color = Color.Lerp(baseColor, baseColor * 1.3f, glow);
        }
    }
}