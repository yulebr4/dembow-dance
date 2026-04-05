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
        transform.position += Vector3.down * speed * Time.deltaTime;

        if (transform.position.y < -6f)
        {
            // Avisar Miss antes de destruirse
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