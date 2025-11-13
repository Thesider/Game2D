using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour {
    public PlayerInput playerInput { get; private set; }
    public Vector2 rawMovementInput { get; private set; }
    public int normInputX { get; private set; }
    public int normInputY { get; private set; }
    public bool jumpInput { get; private set; }
    public bool jumpInputStop { get; private set; }
    public Boolean dashInput { get; private set; }
    public bool dashInputStop { get; private set; }
    public bool[] attackInputs { get; private set; }

    [SerializeField] private float inputHoldTime = 0.2f;
    private float jumpInputStartTime;
    private float dashInputStartTime;


    private void Start() {
        playerInput = GetComponent<PlayerInput>();

        int count = Enum.GetValues(typeof(CombatInputs)).Length;
        attackInputs = new bool[count];

    }
    private void Update() {
        CheckJumpInputHoldTime();
        CheckDashInputHoldTime();
    }

    public void OnPrimaryAttackInput(InputAction.CallbackContext context) {
        if(context.started) {
            attackInputs[(int)CombatInputs.primary] = true;
        }
        if(context.canceled) {
            attackInputs[(int)CombatInputs.primary] = false;
        }

    }
    public void OnSecondaryAttackInput(InputAction.CallbackContext context) {
        if (context.started) {
            attackInputs[(int)CombatInputs.secondary] = true;
        }
        if (context.canceled) {
            attackInputs[(int)CombatInputs.secondary] = false;
        }

    }


    public void OnMoveInput(InputAction.CallbackContext context) {
        rawMovementInput = context.ReadValue<Vector2>();

        normInputX = Mathf.RoundToInt(rawMovementInput.x);
        normInputY = Mathf.RoundToInt(rawMovementInput.y);


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
            jumpInputStartTime = Time.time;
        }
        if(context.canceled) {
            dashInputStop = true;
        } else {
            dashInputStop = false;
        }
    }

    public void UseJumpInput() => jumpInput = false;

    public void UseDashInput() => dashInput = false;

    private void CheckJumpInputHoldTime() {
        if (Time.time >= jumpInputStartTime + inputHoldTime) {
        jumpInput = false;
        }
    }
    private void CheckDashInputHoldTime() {
        if (Time.time >= dashInputStartTime + inputHoldTime) {
            dashInput = false;
        }
    }

}
public enum CombatInputs {
    primary,
    secondary
}