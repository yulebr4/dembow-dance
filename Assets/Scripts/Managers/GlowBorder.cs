using UnityEngine;
using UnityEngine.UI;


public class GlowBorder : MonoBehaviour
{
    public Image borderImage;
    public float pulseSpeed = 3f;
    private bool isActive = false;
    private Color baseColor;

    private void Start()
    {
        if (borderImage != null)
        {
            baseColor = borderImage.color;
            baseColor.a = 0f;
            borderImage.color = baseColor;
        }
    }

    private void Update()
    {
        if (borderImage == null) return;

        if (isActive)
        {
            float alpha = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            alpha = Mathf.Lerp(0.1f, 0.6f, alpha); // Más sutil
            Color c = borderImage.color;
            c.a = alpha;
            borderImage.color = c;
        }
        else
        {
            // Apagar suavemente
            Color c = borderImage.color;
            c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * 5f);
            borderImage.color = c;
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}
