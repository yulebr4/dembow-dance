using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    public static ScenarioManager Instance;

    [Header("Backgrounds jugables")]
    public Sprite[] scenarioSprites;
    public string[] scenarioNames = {
        "NOCHE DEMBOW",
        "CALLES DOMINICANAS",
        "AZOTEA DOMINICANA"
    };

    [Header("Referencias")]
    public SpriteRenderer gameplayBackgroundRenderer;

    [Header("HitZones")]
    public Transform[] hitZones; // Los 4 HitZones

    [Header("Posiciones X por escenario")]
    public float[] scenarioLaneX_0 = { -4f, -1.5f, 1.5f, 4.2f };   // Noche Dembow
    public float[] scenarioLaneX_1 = { -3.5f, -1.2f, 1.2f, 3.5f }; // Calles
    public float[] scenarioLaneX_2 = { -3f, -1f, 1f, 3f };          // Azotea

    private int currentScenarioIndex = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        currentScenarioIndex = PlayerPrefs.GetInt("ScenarioIndex", 0);
        ApplyScenario();
    }

    public int GetCurrentIndex() => currentScenarioIndex;
    public string GetCurrentName() => scenarioNames[currentScenarioIndex];
    public Sprite GetCurrentSprite() => scenarioSprites[currentScenarioIndex];
    public int GetCount() => scenarioSprites.Length;

    public float[] GetCurrentLanePositions()
    {
        switch (currentScenarioIndex)
        {
            case 1: return scenarioLaneX_1;
            case 2: return scenarioLaneX_2;
            default: return scenarioLaneX_0;
        }
    }

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
        // Cambiar sprite sin tocar la escala
        if (gameplayBackgroundRenderer != null && scenarioSprites.Length > currentScenarioIndex)
            gameplayBackgroundRenderer.sprite = scenarioSprites[currentScenarioIndex];

        // Mover HitZones a las posiciones del escenario
        float[] laneX = GetCurrentLanePositions();
        if (hitZones != null)
        {
            for (int i = 0; i < hitZones.Length && i < laneX.Length; i++)
            {
                if (hitZones[i] != null)
                {
                    Vector3 pos = hitZones[i].position;
                    pos.x = laneX[i];
                    hitZones[i].position = pos;
                }
            }
        }
    }
}