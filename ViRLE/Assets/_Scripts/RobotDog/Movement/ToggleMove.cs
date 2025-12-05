using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMove : MonoBehaviour
{
    public float moveAmount = 2f; // Amount to move up
    public KeyCode toggleKey = KeyCode.Space; // Key to press

    private Vector3 originalPosition;
    private bool isMovedUp = false;

    void Start() {
        originalPosition = transform.position;
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey)) {
            if (isMovedUp) {
                transform.position = originalPosition;
            }
            else {
                transform.position = originalPosition + Vector3.up * moveAmount;
            }

            isMovedUp = !isMovedUp; // Toggle state
        }
    }
}
