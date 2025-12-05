using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fastMultiplier = 3f;
    public float lookSensitivity = 180f;

    // Changed default to false so you can use UI immediately
    public bool lockCursor = false;

    float yaw, pitch;

    void Start()
    {
        Vector3 e = transform.eulerAngles;
        yaw = e.y; pitch = e.x;

        // UPDATED: Logic flipped. We want cursor VISIBLE by default for UI.
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void Update()
    {
        // --- UPDATED: ROTATION LOGIC ---
        // Only look around if Right Mouse Button (1) is held down
        if (Input.GetMouseButton(1))
        {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * lookSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch - my * lookSensitivity * Time.deltaTime, -89f, 89f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        // --- MOVE LOGIC (Unchanged) ---
        float mult = Input.GetKey(KeyCode.LeftShift) ? fastMultiplier : 1f;
        Vector3 dir = new Vector3(
            Input.GetAxisRaw("Horizontal"),             // A/D
            (Input.GetKey(KeyCode.E) ? 1 : 0) - (Input.GetKey(KeyCode.Q) ? 1 : 0), // Q/E down/up
            Input.GetAxisRaw("Vertical")                // W/S
        );
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        transform.position += transform.TransformDirection(dir) * moveSpeed * mult * Time.deltaTime;

        // Optional: Toggle cursor lock with Esc if you ever need to debug
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}