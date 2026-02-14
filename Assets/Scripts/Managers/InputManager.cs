using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Referencias")]
    public JudgementSystem judgementSystem;

    [Header("Configuración de Input")]
    public KeyCode[] laneKeys = new KeyCode[4]
    {
        KeyCode.LeftArrow,  // Lane 0
        KeyCode.DownArrow,  // Lane 1
        KeyCode.UpArrow,    // Lane 2
        KeyCode.RightArrow  // Lane 3
    };

    private void Update()
    {
        // Revisar cada tecla
        for (int i = 0; i < laneKeys.Length; i++)
        {
            if (Input.GetKeyDown(laneKeys[i]))
            {
                OnLanePressed(i);
            }
        }
    }

    private void OnLanePressed(int laneIndex)
    {
        Debug.Log($"Tecla presionada: Lane {laneIndex}");

        if (judgementSystem != null)
        {
            judgementSystem.CheckHit(laneIndex);
        }
    }
}
