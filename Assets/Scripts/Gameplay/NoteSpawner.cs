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

        // Coordenadas exactas de los HitZones
        float[] expectedX = { -1f, -0.4f, 0.57f, 1.5f };

        // Buscar todos los objetos con componente SpriteRenderer (las flechas)
        SpriteRenderer[] allSprites = FindObjectsOfType<SpriteRenderer>();

        foreach (SpriteRenderer sr in allSprites)
        {
            // Buscar por nombre o por posición
            if (sr.gameObject.name.Contains("HitZone"))
            {
                float posX = sr.transform.position.x;

                // Determinar qué lane es según su X
                for (int i = 0; i < 4; i++)
                {
                    if (Mathf.Abs(posX - expectedX[i]) < 0.1f) // Tolerancia de 0.1
                    {
                        foundZones[i] = sr.transform;
                        Debug.Log($"HitZone Lane {i} encontrado en X = {posX}");
                        break;
                    }
                }
            }
        }

        // Verificar si encontramos todos
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (foundZones[i] != null)
                count++;
        }

        if (count == 4)
        {
            lanePositions = foundZones;
            Debug.Log("¡Todos los HitZones encontrados automáticamente!");

            // Mostrar las X asignadas
            for (int i = 0; i < 4; i++)
            {
                Debug.Log($"Lane {i} asignado con X = {lanePositions[i].position.x}");
            }
        }
        else
        {
            Debug.LogWarning($"Solo se encontraron {count} HitZones. Asigna manualmente en el Inspector.");
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
        float[] laneX = { -1f, -0.4f, 0.57f, 1.5f };

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