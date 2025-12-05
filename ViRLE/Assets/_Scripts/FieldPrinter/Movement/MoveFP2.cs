using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public enum CommandType
{
    Move,
    Turn,
    Pen
}

[System.Serializable]
public struct RobotCommand
{
    public CommandType type;
    public float value; // Move=meters, Turn=degrees, Pen=1(Down/Draw), 0(Up/Stop)
}

public class MoveFP2 : MonoBehaviour
{
    [Header("Drawing Settings")]
    public GameObject linePrefab; // Drag your 'LineSegment' prefab here

    [Header("Speeds")]
    public float moveSpeed = 1f;
    public float turnSpeed = 90f;

    [Header("Path Commands")]
    public RobotCommand[] commands;

    [Header("Events")]
    public UnityEvent onPathComplete;

    private bool running = false;
    private LineRenderer currentLine; // The line we are currently drawing
    private bool isPenDown = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Run();
    }

    public void Run()
    {
        if (!running)
            StartCoroutine(RunPath());
    }

    IEnumerator RunPath()
    {
        running = true;

        foreach (RobotCommand cmd in commands)
        {
            if (cmd.type == CommandType.Move)
            {
                yield return StartCoroutine(MoveDistance(cmd.value));
            }
            else if (cmd.type == CommandType.Turn)
            {
                // If we turn, we just rotate. The line stays connected to our position.
                yield return StartCoroutine(TurnDegrees(cmd.value));
            }
            else if (cmd.type == CommandType.Pen)
            {
                bool shouldDraw = (cmd.value > 0);

                if (shouldDraw && !isPenDown)
                {
                    StartNewStroke(); // Pen went down -> New Line
                }
                else if (!shouldDraw && isPenDown)
                {
                    currentLine = null; // Pen went up -> Stop updating current line
                }

                isPenDown = shouldDraw;
                yield return null;
            }
        }

        running = false;

        onPathComplete?.Invoke();
    }

    void StartNewStroke()
    {
        // Create the line object
        GameObject go = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        currentLine = go.GetComponent<LineRenderer>();

        // Initialize with 2 points at the robot's current location
        // Point 0 is the start, Point 1 is the "moving tip"
        currentLine.positionCount = 2;
        currentLine.SetPosition(0, transform.position);
        currentLine.SetPosition(1, transform.position);
    }

    IEnumerator MoveDistance(float meters)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = transform.position + (transform.right * meters);

        // If we are starting a move and the pen is down, we need to add a new segment
        // so we don't drag the previous diagonal line if we just turned.
        if (isPenDown && currentLine != null)
        {
            currentLine.positionCount++; // Add a new vertex
            currentLine.SetPosition(currentLine.positionCount - 1, transform.position);
        }

        float duration = Mathf.Abs(meters) / moveSpeed;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / duration);

            // UPDATE THE DRAWING
            if (isPenDown && currentLine != null)
            {
                // Update the last point of the line to stick to the robot
                currentLine.SetPosition(currentLine.positionCount - 1, transform.position);
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // SNAP to exact finish
        transform.position = targetPos;

        // Final update of the line tip
        if (isPenDown && currentLine != null)
        {
            currentLine.SetPosition(currentLine.positionCount - 1, transform.position);
        }
    }

    IEnumerator TurnDegrees(float degrees)
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, 0, degrees);

        float duration = Mathf.Abs(degrees) / turnSpeed;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, timeElapsed / duration);
            // Note: We do NOT update the line while turning. 
            // This keeps the corner sharp at the pivot point.
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRot;
    }
}