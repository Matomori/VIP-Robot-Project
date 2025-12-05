using UnityEngine;

public class GimbalController : MonoBehaviour
{
    public Transform yawPivot;    // assign in Inspector
    public Transform pitchPivot;  // assign in Inspector

    [Header("Limits (degrees)")]
    public float yawMin = -90f, yawMax = 90f;
    public float pitchMin = -30f, pitchMax = 45f;

    [Header("Speeds (degrees/sec)")]
    public float yawSpeed = 180f;
    public float pitchSpeed = 180f;

    [Header("Mouse Input (for testing)")]
    public float mouseYawSensitivity = 120f;
    public float mousePitchSensitivity = 120f;

    float targetYaw, targetPitch;

    void Start()
    {
        targetYaw = Normalize(yawPivot.localEulerAngles.y);
        targetPitch = Normalize(pitchPivot.localEulerAngles.x);
    }

    void Update()
    {
        // TEMP: mouse input to test
        float dyaw = Input.GetAxis("Mouse X") * mouseYawSensitivity * Time.deltaTime;
        float dpitch = -Input.GetAxis("Mouse Y") * mousePitchSensitivity * Time.deltaTime;

        targetYaw = Mathf.Clamp(targetYaw + dyaw, yawMin, yawMax);
        targetPitch = Mathf.Clamp(targetPitch + dpitch, pitchMin, pitchMax);

        float newYaw = Mathf.MoveTowardsAngle(Normalize(yawPivot.localEulerAngles.y), targetYaw, yawSpeed * Time.deltaTime);
        float newPitch = Mathf.MoveTowardsAngle(Normalize(pitchPivot.localEulerAngles.x), targetPitch, pitchSpeed * Time.deltaTime);

        var y = yawPivot.localEulerAngles;
        y.y = newYaw;
        yawPivot.localEulerAngles = y;

        var p = pitchPivot.localEulerAngles;
        p.x = newPitch;
        pitchPivot.localEulerAngles = p;
    }

    static float Normalize(float a) => (a > 180f) ? a - 360f : a;
}
