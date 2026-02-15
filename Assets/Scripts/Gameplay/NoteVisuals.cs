using UnityEngine;

public class NoteVisuals : MonoBehaviour
{
    [Header("Sprites Disponibles")]
    public Sprite[] noteSprites;

    private void Start()
    {
        // Elegir sprite aleatorio
        if (noteSprites != null && noteSprites.Length > 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = noteSprites[Random.Range(0, noteSprites.Length)];
            }
        }
    }
}