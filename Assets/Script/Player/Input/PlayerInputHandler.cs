using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {

    public Vector2 rawMovementInput { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public bool jumpInput { get; private set; }
    public bool jumpInputStop { get; private set; }
    public Boolean dashInput { get; private set; }
    public bool dashInputStop { get; private set; }
    public bool crouchInput { get; private set; }

    [SerializeField] private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;
    private float dashInputStartTime;

    private void Update() {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }
    public void OnMoveInput(InputAction.CallbackContext context) {
        rawMovementInput = context.ReadValue<Vector2>();

        normInputX = (int) (rawMovementInput * Vector2.right).normalized.x;
        normInputY = (int) (rawMovementInput * Vector2.up).normalized.y;
    }   

    public void OnJumpInput(InputAction.CallbackContext context) {

        if (context.started) {
            jumpInput = true;
            jumpInputStop = false;  
            jumpInputStartTime = Time.time;
        }

        if(context.canceled) {
            jumpInputStop = true;
        } else {
            jumpInputStop = false;
        }

    }

    public void OnDashInput(InputAction.CallbackContext context) {
        if (context.started) {
            dashInput = true;
            dashInputStop = false;  
            dashInputStartTime = Time.time;
        }
        if(context.canceled) {
            dashInputStop = true;
        } else {
            dashInputStop = false;
        }
    }

    // Crouch is hold-based: true while pressed, false when released
    public void OnCrouchInput(InputAction.CallbackContext context) {
        if (context.started || context.performed) {
            crouchInput = true;
        }
        if (context.canceled) {
            crouchInput = false;
        }
    }

    public void UseJumpInput() => jumpInput = false;

    public void UseDashInput() => dashInput = false;

    private void CheckDashInputHoldTime() {
        if (Time.time >= dashInputStartTime + inputHoldTime) {
        dashInput = false;
        }
    }
    private void CheckJumpInputHoldTime() {
        if (Time.time >= jumpInputStartTime + inputHoldTime) {
        jumpInput = false;
        }
    }   

}
