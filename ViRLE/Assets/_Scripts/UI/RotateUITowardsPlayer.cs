using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotateTowardsPlayer : MonoBehaviour {
    private Transform playerHead;  // Reference to the player's head (camera)

    void Start() {
        playerHead = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    void Update() {
        // Calculate the direction from the UI to the player's head
        Vector3 directionToPlayer = playerHead.position - transform.position;

        // Calculate the rotation to face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // Apply the rotation to the UI
        transform.rotation = targetRotation;
    }
}
