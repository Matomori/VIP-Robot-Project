using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attatched to the 3D Exclamation mark pointer, and makes it rotate, float up and down, or both.
/// </summary>
public class GameMarkerMovement : MonoBehaviour
{
    [SerializeField] private bool willRotate = true;
    [SerializeField] private bool willFloat = true;

    [SerializeField] private float floatAmplitude = 0.5f;   // height of up/down movement
    [SerializeField] private float floatFrequency = 1f;     // speed of the up/down movement
    [SerializeField] private float spinSpeed = 90f;         // in degrees per second

    private Vector3 startPos;
    private Transform t;

    void Start() { 
        t = transform;
        startPos = t.position; 
    }

    void Update() {
        Float();
        Rotate();
    }

    private void Float() {
        if (!willFloat) { return; }
        float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        t.position = new Vector3(startPos.x, newY, startPos.z);
    }

    private void Rotate() {
        if (!willRotate) { return; }
        t.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
    }
}
