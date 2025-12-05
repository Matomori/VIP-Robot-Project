using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "SOs/Input Reader")]
public class InputReader : ScriptableObject, DogControls.IDogWalkActions 
{   
    // Movement
    public event System.Action<Vector2> moveEvent = delegate { };
    public event System.Action<float> rotateEvent = delegate { };
    public event System.Action<float> stopRotateEvent = delegate { };
    public event System.Action cameraSwitchEvent = delegate { };

    private DogControls _dogControls;

    void OnEnable() {
        _dogControls = new DogControls();
        _dogControls.DogWalk.SetCallbacks(this);
    }

    void OnDisable() { DisableAllInputs(); }

    /// <summary>
    /// Moves spot forward, back, strafe left, strafe right
    /// </summary>
    public void OnLeftStick(InputAction.CallbackContext context) {
        moveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    /// <summary>
    /// Rotates spot left or right relative to its head
    /// </summary>
    public void OnRightStick(InputAction.CallbackContext context) {
        if (context.performed) {
            rotateEvent?.Invoke(context.ReadValue<float>());
        }

        if (context.canceled) {
            //Debug.Log("reset val");
            stopRotateEvent?.Invoke(context.ReadValue<float>());
        }  
    }

    /// <summary>
    /// Swap Controller Camera View
    /// </summary>
    public void OnSwapCamView(InputAction.CallbackContext context)
    {
        cameraSwitchEvent?.Invoke();
    }

    /// <summary>
    /// For normal spot overworld movment
    /// </summary>
    public void EnableWalkInputs() {
        _dogControls.DogWalk.Enable();
    }

    public void DisableAllInputs() {
        _dogControls.DogWalk.Disable();
    }
}
