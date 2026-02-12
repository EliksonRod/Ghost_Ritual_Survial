using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;



[RequireComponent(typeof(CharacterController))]
//#if ENABLE_INPUT_SYSTEM
[RequireComponent(typeof(PlayerInput))]
//#endif
public class PlayerController : MonoBehaviour
{
    //public GameObject CinemachineCameraTarget;
    [Header("Movement Settings")]
    public float walkSpeed = 4.0f;
    public float sprintSpeed = 7.0f;
    public float rotationSpeed = 1.0f;
    public float SpeedChangeRate = 10.0f;
    public float accelerationRate = 10.0f;
    public float decelerationRate = 10f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.2f;
    public float gravity = -40.0f;
    public float jumpCooldown = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float fallTimeout = 0.15f;

    [Header("Grounded Settings")]
    public bool grounded = true;
    [Tooltip("Useful for rough ground")]
    public float groundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.5f;
    public LayerMask groundLayers;

    [Header("Camera Settings")]
    //public GameObject CinemachineCameraTarget;
    public CinemachineCamera virtualCamera;
    public float maxCameraPitch = 70f;
    public float minCameraPitch = -70f;

    [Header("Headbob Settings")]
    public CinemachineBasicMultiChannelPerlin headBob;
    public float headBobAcceleration = 10f;
    public float idleBobAmp = .5f;
    public float idleBobFreq = 1f;
    public float walkBobAmp = 3f;
    public float walkBobFreq = 1f;
    public float sprintBobAmp = 4f;
    public float sprintBobFreq = 3f;

    [Header("Interact Settings")]
    public bool isInteracting = false;

    private CharacterController characterController;
    private PlayerInput playerInput;
    private CharacterInputs _input;

    private Vector3 velocity;
    private float jumpCooldownTimer;
    private float cameraPitch;


    // player
    private float _speed;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float jumpCooldownDelta;
    private float fallTimeoutDelta;


    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        _input = GetComponent<CharacterInputs>();
    }

    private void Start()
    {
        //if (virtualCamera == null)
        //{
        // Debug.LogError("Virtual Camera is not assigned.", this);
        //}

        // reset our timeouts on start
        jumpCooldownDelta = jumpCooldown;
        fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        //JumpAndGravity();
        HandleJumping();
        HandleGravity();
        GroundedCheck();
        //MovementHandler();
        HandleMovement();
        Debug.Log(grounded);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void CameraRotation()
    {
        if (isInteracting) { return; }

        Vector2 lookInput = _input.look;
        cameraPitch += lookInput.y * rotationSpeed;
        cameraPitch = Mathf.Clamp(cameraPitch, minCameraPitch, maxCameraPitch);

        virtualCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0, 0);
        transform.Rotate(Vector3.up * lookInput.x * rotationSpeed);
    }
    private void HandleMovement()
    {
        if (isInteracting)
        {
            _input.move = Vector2.zero;
            velocity = Vector3.zero;

            headBob.AmplitudeGain = idleBobAmp;
            headBob.FrequencyGain = idleBobFreq;
            return;
        }

        HeadBob();

        Vector2 input = _input.move;
        Vector3 moveDirection = transform.right * input.x + transform.forward * input.y;

        float targetSpeed = _input.sprint ? sprintSpeed : walkSpeed;

        if (moveDirection != Vector3.zero)
        {
            velocity.x = Mathf.Lerp(velocity.x, targetSpeed * moveDirection.x, Time.deltaTime * accelerationRate);
            velocity.z = Mathf.Lerp(velocity.z, targetSpeed * moveDirection.z, Time.deltaTime * accelerationRate);
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, Time.deltaTime * decelerationRate);
            velocity.z = Mathf.Lerp(velocity.z, 0, Time.deltaTime * decelerationRate);
        }

        characterController.Move(new Vector3(velocity.x, 0, velocity.z) * Time.deltaTime);
    }
    private void MovementHandler()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? sprintSpeed : walkSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            // move
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        // move the player
        characterController.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void JumpAndGravity()
    {
        if (grounded)
        {
            // reset the fall timeout timer
            fallTimeoutDelta = fallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (velocity.y < 0.0f)
            {
                velocity.y = -2f;
            }

            // Jump
            if (_input.jump && jumpCooldownDelta <= 0.0f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // jump timeout
            if (jumpCooldownDelta >= 0.0f)
            {
                jumpCooldownDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            jumpCooldownDelta = jumpCooldown;

            // fall timeout
            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= Time.deltaTime;
            }

            // if we are not grounded, do not jump
            _input.jump = false;
        }

        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (velocity.y < _terminalVelocity)
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    private void HandleGravity()
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(Vector3.up * velocity.y * Time.deltaTime);
    }

    private void HandleJumping()
    {
        if (jumpCooldownTimer > 0)
        {
            jumpCooldownTimer -= Time.deltaTime;
        }

        if (grounded)
        {
            if (_input.jump && jumpCooldownTimer <= 0)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpCooldownTimer = jumpCooldown;
            }
        }
        else
        {
            _input.jump = false;
        }
    }


    private void HeadBob()
    {
        float moveMagnitude = _input.move.magnitude;
        float targetAmp = moveMagnitude > 0 ? (_input.sprint ? sprintBobAmp : walkBobAmp) : idleBobAmp;
        float targetFreq = moveMagnitude > 0 ? (_input.sprint ? sprintBobFreq : walkBobFreq) : idleBobFreq;

        headBob.AmplitudeGain = Mathf.Lerp(headBob.AmplitudeGain, targetAmp, Time.deltaTime * headBobAcceleration);
        headBob.FrequencyGain = Mathf.Lerp(headBob.FrequencyGain, targetFreq, Time.deltaTime * headBobAcceleration);
    }
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
    }
}
