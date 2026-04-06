using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    [Header("Backgrounds jugables")]
    public Sprite[] scenarioSprites; // Los 2 backgrounds
    public string[] scenarioNames = {
    "NOCHE DEMBOW",
    "CALLES DOMINICANAS",
    "AZOTEA DOMINICANA" 
};
    [Header("Referencias")]
    public SpriteRenderer gameplayBackgroundRenderer; // El SpriteRenderer del GameplayBackground

    private int currentScenarioIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentScenarioIndex = PlayerPrefs.GetInt("ScenarioIndex", 0);
    }

    public int GetCurrentIndex() => currentScenarioIndex;
    public string GetCurrentName() => scenarioNames[currentScenarioIndex];
    public Sprite GetCurrentSprite() => scenarioSprites[currentScenarioIndex];
    public int GetCount() => scenarioSprites.Length;

    public void SetScenario(int index)
    {
        currentScenarioIndex = index;
    }

    public void SaveScenario()
    {
        PlayerPrefs.SetInt("ScenarioIndex", currentScenarioIndex);
        PlayerPrefs.Save();
    }

    public void ApplyScenario()
    {
        if (gameplayBackgroundRenderer != null && scenarioSprites.Length > currentScenarioIndex)
            gameplayBackgroundRenderer.sprite = scenarioSprites[currentScenarioIndex];
    }
}