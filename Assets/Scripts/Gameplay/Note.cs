
using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public int laneIndex; // 0=Left, 1=Down, 2=Up, 3=Right

    [Header("Note Data")]
    public float spawnTime; // Cuándo debe aparecer
    public float hitTime;   // Cuándo debe ser golpeada

    private void Update()
    {
        // Mover hacia abajo
        transform.position += Vector3.down * speed * Time.deltaTime;

        // Destruir si sale de pantalla
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    // Para visualizar en editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
    }
}
