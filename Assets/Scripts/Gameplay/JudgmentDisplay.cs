using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JudgmentDisplay : MonoBehaviour
{
    public static JudgmentDisplay Instance;

    [Header("UI")]
    public Image displayImage;

    [Header("Sprites")]
    public Sprite perfectSprite;
    public Sprite goodSprite;
    public Sprite okSprite;
    public Sprite missSprite;

    [Header("Animacion")]
    public float displayDuration = 0.8f;
    public float scaleUpTime = 0.1f;
    public float scaleDownTime = 0.2f;
    public float maxScale = 1.2f;

    private Coroutine currentCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (displayImage != null)
        {
            Color c = displayImage.color;
            c.a = 0f;
            displayImage.color = c;
        }
    }

    public void ShowJudgment(string judgement)
    {
        Sprite sprite = null;

        switch (judgement)
        {
            case "Perfect": sprite = perfectSprite; break;
            case "Good": sprite = goodSprite; break;
            case "OK": sprite = okSprite; break;
            case "Miss": sprite = missSprite; break;
        }

        if (sprite == null || displayImage == null) return;

        this.gameObject.SetActive(true);

        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(AnimateJudgment(sprite));
    }

    private IEnumerator AnimateJudgment(Sprite sprite)
    {
        displayImage.sprite = sprite;
        displayImage.transform.localScale = Vector3.one;

        // Aparecer con scale up
        float elapsed = 0f;
        while (elapsed < scaleUpTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleUpTime;
            float scale = Mathf.Lerp(0.5f, maxScale, t);
            displayImage.transform.localScale = Vector3.one * scale;

            Color c = displayImage.color;
            c.a = t;
            displayImage.color = c;
            yield return null;
        }

        // Volver a escala normal
        displayImage.transform.localScale = Vector3.one;
        Color full = displayImage.color;
        full.a = 1f;
        displayImage.color = full;

        // Mantener visible
        yield return new WaitForSeconds(displayDuration);

        // Desaparecer con scale down
        elapsed = 0f;
        Vector3 startScale = displayImage.transform.localScale;
        while (elapsed < scaleDownTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / scaleDownTime;

            displayImage.transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.5f, t);

            Color c = displayImage.color;
            c.a = Mathf.Lerp(1f, 0f, t);
            displayImage.color = c;
            yield return null;
        }

        Color final = displayImage.color;
        final.a = 0f;
        displayImage.color = final;
        displayImage.transform.localScale = Vector3.one;

        currentCoroutine = null;
    }
}