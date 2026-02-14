using System.Collections.Generic;
using UnityEngine;

public class JudgementSystem : MonoBehaviour
{
    [Header("Referencias")]
    public Transform[] hitZones; // Las 4 zonas de golpeo

    [Header("Timing Windows (en segundos)")]
    public float perfectWindow = 0.05f;  // ±50ms
    public float goodWindow = 0.1f;      // ±100ms
    public float missWindow = 0.15f;     // ±150ms

    [Header("Debug")]
    public bool showDebugMessages = true;

    private void Start()
    {
        if (hitZones.Length != 4)
        {
            Debug.LogError("JudgementSystem necesita 4 HitZones!");
        }
    }

    public void CheckHit(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= hitZones.Length)
            return;

        // Buscar la nota más cercana en este carril
        Note closestNote = FindClosestNoteInLane(laneIndex);

        if (closestNote == null)
        {
            if (showDebugMessages)
                Debug.Log($"Miss - No hay nota en Lane {laneIndex}");
            return;
        }

        // Calcular distancia a la HitZone
        float distance = Mathf.Abs(closestNote.transform.position.y - hitZones[laneIndex].position.y);

        // Evaluar timing
        string judgement = EvaluateTiming(distance);

        if (showDebugMessages)
            Debug.Log($"{judgement}! Lane {laneIndex} - Distancia: {distance:F3}");

        // Destruir la nota si acertó
        if (judgement != "Miss")
        {
            Destroy(closestNote.gameObject);
        }
    }

    private Note FindClosestNoteInLane(int laneIndex)
    {
        Note[] allNotes = FindObjectsOfType<Note>();
        Note closestNote = null;
        float closestDistance = float.MaxValue;

        foreach (Note note in allNotes)
        {
            // Solo buscar en el carril correcto
            if (note.laneIndex != laneIndex)
                continue;

            float distance = Mathf.Abs(note.transform.position.y - hitZones[laneIndex].position.y);

            // Solo considerar notas dentro del rango de miss
            if (distance < missWindow * 5f && distance < closestDistance)
            {
                closestDistance = distance;
                closestNote = note;
            }
        }

        return closestNote;
    }

    private string EvaluateTiming(float distance)
    {
        // Convertir distancia a tiempo aproximado (asumiendo velocidad de nota = 5)
        float timeOffset = distance / 5f;

        if (timeOffset <= perfectWindow)
            return "Perfect";
        else if (timeOffset <= goodWindow)
            return "Good";
        else if (timeOffset <= missWindow)
            return "OK";
        else
            return "Miss";
    }
}