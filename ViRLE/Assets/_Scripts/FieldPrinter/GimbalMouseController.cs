using UnityEngine;

public class GimbalMouseController : MonoBehaviour
{
    [Header("Assign pivots (required)")]
    public Transform yawPivot;    // rotates around WORLD/LOCAL Y to face target left/right
    public Transform pitchPivot;  // child of yawPivot; rotates around LOCAL X (up/down)

    [Header("Target to look at (required)")]
    public Transform target;      // e.g., your viewer camera transform

    [Header("Optional: the piece that should aim at target")]
    public Transform lensTransform; // if null, we assume pitchPivot.forward is your aim axis

    [Header("Axis options")]
    public Vector3 worldUp = Vector3.up; // yaw plane up; keep as Vector3.up for robots
    public bool invertPitch = false;     // flip if up/down feels backwards

    void LateUpdate()
    {
        if (!yawPivot || !pitchPivot || !target) return;

        // ---------- 1) YAW in world space (no Euler drift) ----------
        Vector3 toTarget = target.position - yawPivot.position;

        // Project onto yaw plane (XZ plane by default)
        Vector3 toTargetXZ = Vector3.ProjectOnPlane(toTarget, worldUp);
        if (toTargetXZ.sqrMagnitude > 1e-10f)
        {
            // Face target on the horizontal plane using a stable LookRotation
            Quaternion yawWorldRot = Quaternion.LookRotation(toTargetXZ.normalized, worldUp);
            yawPivot.rotation = yawWorldRot;  // set WORLD rotation (only yaw component effectively)
        }

        // ---------- 2) PITCH in yawed local frame ----------
        // Recompute direction from pitchPivot after yaw has been applied
        Vector3 toTargetFromPitch = target.position - pitchPivot.position;

        // Express this direction in PITCH PIVOT'S LOCAL SPACE
        Vector3 localDir = pitchPivot.parent != null
            ? pitchPivot.parent.InverseTransformDirection(toTargetFromPitch) // relative to yaw frame
            : toTargetFromPitch; // fallback

        // Desired pitch is the tilt around local X so that local forward (Z) points at the target
        // Angle between forward(Z) and the target direction in the YZ plane:
        float desiredPitchDeg = Mathf.Atan2(localDir.y, localDir.z) * Mathf.Rad2Deg;
        if (invertPitch) desiredPitchDeg = -desiredPitchDeg;

        // Apply ONLY around local X — keep Y/Z as they are
        // (We explicitly rebuild the local rotation so there's no roll)
        pitchPivot.localRotation = Quaternion.Euler(desiredPitchDeg, 0f, 0f);

        // ---------- 3) If your mesh/lens forward isn't aligned with +Z ----------
        if (lensTransform != null)
        {
            // Keep lensTransform aligned with pitchPivot (no additional offset),
            // but if your lens needs a fixed offset (e.g., aims down +Z after a 90° model rotation),
            // make that offset by rotating lensTransform in EDIT mode once.
            // At runtime, we just keep it zeroed:
            lensTransform.localPosition = lensTransform.localPosition; // no-op; left here for clarity
            // (If you had to rotate lensTransform to match the lens direction, keep that edit-time rotation.)
        }
    }
}
