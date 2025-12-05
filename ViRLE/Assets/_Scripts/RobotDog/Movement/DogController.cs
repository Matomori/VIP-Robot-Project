using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Handles Robot Dog Movement
/// </summary>
public class DogController : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader = default;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotSpeed;

    [SerializeField] private MeshRenderer camView;
    [SerializeField] private Material frontView;
    [SerializeField] private Material topView;
    private bool isFront = true;

    private Rigidbody rb;
    private Transform t;
    private Animator animator;
    private Vector2 moveDir;
    private float rotationInput;

    private bool isMoving = false;
    private float movementThresh = 0.01f;

    public Vector3 Movement {  get; private set; }

    void Awake() {
        rb = GetComponent<Rigidbody>();
        t = transform;
        animator = GetComponent<Animator>();
        animator.speed = 0f;
    }

    void FixedUpdate() {
        // Compute movement direction
        Movement = transform.forward * moveDir.y * moveSpeed * Time.fixedDeltaTime +
                           transform.right * moveDir.x * moveSpeed * Time.fixedDeltaTime;

        // Apply movement using MovePosition (preserving physics interactions)
        rb.MovePosition(rb.position + Movement);

        // apply rotation
        if (rotationInput != 0) {
            Quaternion deltaRot = Quaternion.Euler(0f, rotationInput * rotSpeed * Time.fixedDeltaTime, 0f);
            rb.MoveRotation(rb.rotation * deltaRot);
        }

        // Determine if the character is moving
        bool wasMoving = isMoving;
        isMoving = moveDir.sqrMagnitude > movementThresh || Mathf.Abs(rotationInput) > 0;


        // Pause/unpause animation instead of restarting it
        if (isMoving && !wasMoving) {
            animator.speed = 1f; // Resume animation
        }
        else if (!isMoving && wasMoving) {
            animator.speed = 0f; // Pause animation at the current frame
        }

        //animator.SetBool("isWalking", isMoving);
    }

    private void SwitchCamView() {
        isFront = !isFront;

        camView.material = isFront ? frontView : topView;
    }

    void OnEnable() {
        _inputReader.EnableWalkInputs();
        _inputReader.moveEvent += OnMove;
        _inputReader.rotateEvent += OnRotate;
        _inputReader.stopRotateEvent += StopRotate;
        _inputReader.cameraSwitchEvent += SwitchCamView;
    }

    void OnDisable() {
        _inputReader.moveEvent -= OnMove;
        _inputReader.rotateEvent -= OnRotate;
        _inputReader.stopRotateEvent -= StopRotate;
        _inputReader.cameraSwitchEvent -= SwitchCamView;
    }

    // EVENT LISTNEERS ---
    private void OnMove(Vector2 move) {
        moveDir = move;
    }

    private void OnRotate(float rot) {
        rotationInput = rot;
    }

    private void StopRotate(float rot) {
        rotationInput = 0;
    }
}
