using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public int laneIndex;

    [Header("Note Data")]
    public float spawnTime;
    public float hitTime;

    private void Start()
    {
        // Leer velocidad según dificultad guardada
        if (OptionsManager.Instance != null)
            speed = OptionsManager.Instance.GetNoteSpeed();
    }

    private void Update()
    {
        // Si es automática, actualizar velocidad cada frame
        if (PlayerPrefs.GetInt("Difficulty", 1) == 3)
        {
            if (OptionsManager.Instance != null)
                speed = OptionsManager.Instance.GetNoteSpeed();
        }

        transform.position += Vector3.down * speed * Time.deltaTime;

        if (transform.position.y < -6f)
        {
            if (GameManager.Instance != null && GameManager.Instance.isPlaying)
            {
                ScoreManager sm = FindObjectOfType<ScoreManager>();
                if (sm != null)
                    sm.AddScore("Miss");
            }
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
    }
}