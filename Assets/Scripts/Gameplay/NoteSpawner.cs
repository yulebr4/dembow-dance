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

    private void Start()
    {
        // TEST: Spawning automático cada segundo
        StartCoroutine(TestSpawning());
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
}
