using System.Collections;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject notePrefab;

    [Header("Lane Settings")]
    public Transform[] lanePositions; // 4 carriles

    [Header("Spawn Settings")]
    public float spawnY = 6f;
    public float testSpawnInterval = 1f; // Para testing

    private Coroutine spawnCoroutine; // Guardar referencia a la coroutine

    private void Start()
    {
        // Iniciar spawning automático
        StartSpawning();
    }

    // Método para iniciar el spawning
    public void StartSpawning()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(TestSpawning());
            Debug.Log("Spawning iniciado");
        }
    }

    // Método para detener el spawning
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
            Debug.Log("Spawning detenido");
        }
    }

    IEnumerator TestSpawning()
    {
        while (true)
        {
            yield return new WaitForSeconds(testSpawnInterval);

            // Spawn en carril aleatorio
            int randomLane = Random.Range(0, 4);
            SpawnNote(randomLane);
        }
    }

    public void SpawnNote(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= lanePositions.Length)
            return;

        Vector3 spawnPos = new Vector3(
            lanePositions[laneIndex].position.x,
            spawnY,
            0
        );

        GameObject noteObj = Instantiate(notePrefab, spawnPos, Quaternion.identity);
        Note note = noteObj.GetComponent<Note>();
        note.laneIndex = laneIndex;
        note.spawnTime = Time.time;
    }

    // Se llama automáticamente cuando el script se desactiva
    private void OnDisable()
    {
        StopSpawning();
    }

    // Se llama automáticamente cuando el objeto se destruye
    private void OnDestroy()
    {
        StopSpawning();
    }
}
