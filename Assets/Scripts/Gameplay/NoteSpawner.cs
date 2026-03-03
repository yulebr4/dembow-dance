using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject notePrefab;

    [Header("Lane Settings")]
    public Transform[] lanePositions; // 4 carriles - AHORA USA LOS HITZONES

    [Header("Spawn Settings")]
    public float spawnY = 6f;
    public float testSpawnInterval = 1f; // Para testing

    private Coroutine spawnCoroutine; // Guardar referencia a la coroutine

    private void Start()
    {
        // Buscar los HitZones automáticamente si no están asignados
        if (lanePositions == null || lanePositions.Length == 0)
        {
            FindHitZones();
        }

        // Iniciar spawning automático
        // StartSpawning(); // Comentado para que no empiece solo
    }

    // NUEVO: Buscar los HitZones automáticamente
    private void FindHitZones()
    {
        Transform[] foundZones = new Transform[4];

        // Buscar directamente por nombre en la jerarquía
        for (int i = 0; i < 4; i++)
        {
            // Buscar Lane_i/HitZone
            GameObject lane = GameObject.Find($"Lane_{i}");
            if (lane != null)
            {
                Transform hitZone = lane.transform.Find("HitZone");
                if (hitZone != null)
                {
                    foundZones[i] = hitZone;
                    Debug.Log($"HitZone Lane {i} encontrado en X = {hitZone.position.x}");
                }
            }
        }

        int count = 0;
        for (int i = 0; i < 4; i++)
            if (foundZones[i] != null) count++;

        if (count == 4)
        {
            lanePositions = foundZones;
            Debug.Log("¡Todos los HitZones encontrados automáticamente!");
        }
        else
        {
            Debug.LogWarning($"Solo se encontraron {count} HitZones.");
        }
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
        // Array con las X de los HitZones
        float[] laneX = { -4f, -1.5f, 1.5f, 4.2f };

        Vector3 spawnPos = new Vector3(
            laneX[laneIndex],
            spawnY,
            0
        );

        GameObject noteObj = Instantiate(notePrefab, spawnPos, Quaternion.identity);

        Note note = noteObj.GetComponent<Note>();
        if (note != null)
        {
            note.laneIndex = laneIndex;
            note.spawnTime = Time.time;
        }

        Debug.Log($"Nota spawneda en Lane {laneIndex} en X = {laneX[laneIndex]}");
    }

    private void OnDisable()
    {
        StopSpawning();
    }

    private void OnDestroy()
    {
        StopSpawning();
    }
}