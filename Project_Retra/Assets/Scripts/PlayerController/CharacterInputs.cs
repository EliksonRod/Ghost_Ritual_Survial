using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Interaction")]
    public bool interact;
    public bool stopInteract;
    public bool useFlashlight;
    public bool useBlacklight;


    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    // Move Input
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    // Look Input
    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }
    // Jump Input
    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }
    // Sprint Input
    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }
    // Interaction Inputs
    public void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }
    // Stop Interaction Input
    public void OnStopInteract(InputValue value)
    {
        StopInteractInput(value.isPressed);
    }
    // Flashlight Input
    public void ToggleFlashlight(InputValue value)
    {
        UseFlashlightInput(value.isPressed);
    }
    // Blacklight Input
    public void OnUseBlacklight(InputValue value)
    {
        UseBlacklightInput(value.isPressed);
    }



    /*private void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }*/
    private void MoveInput(Vector2 newMoveDirection) => move = newMoveDirection;
    private void LookInput(Vector2 newLookDirection) => look = newLookDirection;
    private void JumpInput(bool newJumpState) => jump = newJumpState;
    public void SprintInput(bool newSprintState) => sprint = newSprintState;
    private void InteractInput(bool interactInput) => interact = interactInput;
    private void StopInteractInput(bool stopInteractInput) => stopInteract = stopInteractInput;
    private void UseFlashlightInput(bool useFlashlightInput) => useFlashlight = useFlashlightInput;
    private void UseBlacklightInput(bool useBlacklightInput) => useBlacklight = useBlacklightInput;


    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
