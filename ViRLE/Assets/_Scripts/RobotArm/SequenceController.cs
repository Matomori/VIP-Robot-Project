using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotSequenceController : MonoBehaviour
{
    [Header("Joint References")]
    [Tooltip("The base of the robot (Low Joint). Rotates on Y.")]
    public Transform lowJoint;
    [Tooltip("The middle arm segment (Mid Joint). Rotates on Z.")]
    public Transform midJoint;
    [Tooltip("The top claw/hand (Top Joint). Rotates on Z.")]
    public Transform topJoint;

    [Header("Global Settings")]
    [Tooltip("Repeat the sequence forever?")]
    public bool loopSequence = true;
    [Tooltip("Default speed for all moves (degrees per second).")]
    public float globalSpeed = 40f;

    // -- DEFINITIONS --
    public enum RobotPart { Base, Arm, Hand }
    public enum MoveMode { GoToAngle, AdjustBy }

    [System.Serializable]
    public class RobotMove
    {
        [Tooltip("Which part of the robot to move.")]
        public RobotPart part;

        [Tooltip("GoToAngle: Moves to a specific angle (e.g. 90).\nAdjustBy: Adds to current angle (e.g. +10 or -10).")]
        public MoveMode mode = MoveMode.GoToAngle;

        [Tooltip("The target angle or amount to add.")]
        [Range(-180, 180)]
        public float value;

        [Tooltip("How long to wait after this move finishes.")]
        public float waitTime = 0.5f;

        [Header("Optional Overrides")]
        public bool overrideSpeed = false;
        public float customSpeed = 40f;
    }

    [Header("The Plan")]
    public List<RobotMove> sequence = new List<RobotMove>();

    private void Start()
    {
        // 1. Auto-Assign Joints if missing
        if (lowJoint == null) lowJoint = this.transform;

        // Find children by name if not manually assigned
        if (midJoint == null) midJoint = RecursiveFind(lowJoint, "midJoint");
        if (topJoint == null) midJoint = RecursiveFind(lowJoint, "topJoint");

        // Fallback: If names are different, try to find *any* child if specific names fail
        // (This helps if you renamed things slightly)
        if (midJoint == null && lowJoint.childCount > 0) midJoint = lowJoint.GetChild(0);
        if (topJoint == null && midJoint != null && midJoint.childCount > 0) topJoint = midJoint.GetChild(0);

        if (midJoint == null || topJoint == null)
        {
            Debug.LogError("RobotSequenceController: Could not find all joints! Please assign them manually in the Inspector.");
            return;
        }

        // 2. Start the animation loop
        StartCoroutine(RunSequence());
    }

    // Helper to find deep children just in case hierarchy changed
    Transform RecursiveFind(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = RecursiveFind(child, name);
            if (result != null) return result;
        }
        return null;
    }

    IEnumerator RunSequence()
    {
        do
        {
            foreach (var move in sequence)
            {
                // 1. Setup the move
                Transform targetTransform = GetTransform(move.part);
                float moveSpeed = move.overrideSpeed ? move.customSpeed : globalSpeed;

                // 2. Calculate Target Rotation
                Vector3 currentEuler = targetTransform.localEulerAngles;
                float currentAngleOnAxis = GetCurrentAngleOnAxis(move.part, currentEuler);
                float targetAngleOnAxis = 0f;

                if (move.mode == MoveMode.GoToAngle)
                {
                    targetAngleOnAxis = move.value;
                }
                else // AdjustBy (Relative)
                {
                    // Convert Unity's 0-360 angle to -180 to 180 for easier math, then add value
                    float angle = (currentAngleOnAxis > 180) ? currentAngleOnAxis - 360 : currentAngleOnAxis;
                    targetAngleOnAxis = angle + move.value;
                }

                Quaternion targetRot = CalculateTargetRotation(move.part, currentEuler, targetAngleOnAxis);

                // 3. Execute Rotation
                while (Quaternion.Angle(targetTransform.localRotation, targetRot) > 0.1f)
                {
                    targetTransform.localRotation = Quaternion.RotateTowards(
                        targetTransform.localRotation,
                        targetRot,
                        moveSpeed * Time.deltaTime
                    );
                    yield return null;
                }

                // Snap to final exact rotation to prevent drift
                targetTransform.localRotation = targetRot;

                // 4. Wait
                if (move.waitTime > 0)
                    yield return new WaitForSeconds(move.waitTime);
            }

        } while (loopSequence);
    }

    // -- HELPERS --

    Transform GetTransform(RobotPart part)
    {
        switch (part)
        {
            case RobotPart.Base: return lowJoint;
            case RobotPart.Arm: return midJoint;
            case RobotPart.Hand: return topJoint;
            default: return null;
        }
    }

    float GetCurrentAngleOnAxis(RobotPart part, Vector3 euler)
    {
        // Base rotates on Y, others on Z
        return (part == RobotPart.Base) ? euler.y : euler.z;
    }

    Quaternion CalculateTargetRotation(RobotPart part, Vector3 currentEuler, float targetAngle)
    {
        if (part == RobotPart.Base)
            return Quaternion.Euler(currentEuler.x, targetAngle, currentEuler.z);
        else
            return Quaternion.Euler(currentEuler.x, currentEuler.y, targetAngle);
    }
}