using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class Flashlight : MonoBehaviour
{

    [SerializeField] GameObject flashlightObject; // Reference to the flashlight GameObject
    [SerializeField] GameObject blacklightObject; // Reference to the blacklight GameObject
    bool blacklightOn = false; // Track the state of the blacklight

    ControllerForPlayer PlayerController; // Reference to the player's controller script
    InputForPlayer playerInput; // Reference to the player's input script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerController = ControllerForPlayer.Instance; // Get the instance of the InputForPlayer script
        //PlayerInput = InputForPlayer.Instance; // Get the instance of the ControllerForPlayer script
        PlayerController = GetComponent<ControllerForPlayer>(); // Get the instance of the InputForPlayer script\
        playerInput = GetComponent<InputForPlayer>();
    }

    void Update()
    {
        if (blacklightOn)
        {
            PlayerController.ChangeFov((PlayerController.originalFov * (70f / 100f))); // Change FOV to 40 when blacklight is active
        }
        else
        {
            PlayerController.ResetFov(); // Reset FOV to default when blacklight is not active
        }
    }

    void OnUseFlashlight(InputValue value)
    {
        if (value.isPressed && !blacklightObject.activeInHierarchy)
        {
            flashlightObject.SetActive(!flashlightObject.activeSelf);
        }
    }

    void OnUseBlacklight(InputValue value)
    {
        if (value.isPressed)
        {
            blacklightOn = true;
            flashlightObject.SetActive(false);
            blacklightObject.SetActive(true);
        }
    }
    void OnKillBlacklight(InputValue value)
    {
        if (!value.isPressed)
        {
            blacklightOn = false;
            blacklightObject.SetActive(false); // Ensure blacklight is turned off when the input is released
            PlayerController.ResetFov(); // Reset FOV to default when blacklight is turned off
            flashlightObject.SetActive(true);
        } 
    }
}
