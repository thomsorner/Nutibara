using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Actions (PC)")]
    [SerializeField] private InputActionAsset PlayerControls;

    [Header("Mobile Controls")]
    [SerializeField] private VirtualJoystick moveJoystick; // Stick izquierdo
    [SerializeField] private LookJoystick lookJoystick;    // Stick derecho

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float mouseLookSensitivity = 2f;
    [SerializeField] private float mobileLookSensitivity = 120f;
    [SerializeField] private float maxLookAngle = 80f;

    [Header("Footsteps")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;
    [SerializeField] private float velocityThreshold = 2f;

    // Components
    private CharacterController controller;
    private Camera mainCamera;
    private PlayerAnimator playerAnimator;

    // Input (PC)
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction interactAction;

    // State
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation;
    private bool isMoving;
    private float nextStepTime;
    private int lastPlayedIndex = -1;

    private bool isMobile;

    // Interaction
    private LampSwitch currentLampSwitch;

    // ================= LIFECYCLE =================

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        mainCamera = Camera.main;

#if UNITY_ANDROID || UNITY_IOS
        isMobile = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
#else
        isMobile = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
#endif
    }

    private void OnEnable()
    {
        if (PlayerControls == null) return;

        var map = PlayerControls.FindActionMap("Player");

        moveAction = map.FindAction("Move");
        lookAction = map.FindAction("Look");
        jumpAction = map.FindAction("Jump");
        sprintAction = map.FindAction("Sprint");
        interactAction = map.FindAction("Interact");

        moveAction.Enable();
        lookAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();
        interactAction.Enable();

        interactAction.performed += _ => Interact();
    }

    private void OnDisable()
    {
        interactAction.performed -= _ => Interact();

        moveAction.Disable();
        lookAction.Disable();
        jumpAction.Disable();
        sprintAction.Disable();
        interactAction.Disable();
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        HandleLook();
        HandleFootsteps();
    }

    // ================= INPUT =================

    private void ReadInput()
    {
        if (isMobile)
        {
            if (moveJoystick != null)
                moveInput = moveJoystick.Direction;

            if (lookJoystick != null)
                lookInput = lookJoystick.Direction;
        }
        else
        {
            moveInput = moveAction.ReadValue<Vector2>();
            lookInput = lookAction.ReadValue<Vector2>();
        }
    }

    // ================= MOVEMENT =================

    private void HandleMovement()
    {
        float sprint =
            (!isMobile && sprintAction.ReadValue<float>() > 0)
            ? sprintMultiplier
            : 1f;

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        if (controller.isGrounded)
        {
            velocity.y = -1f;

            if (!isMobile && jumpAction.triggered)
                velocity.y = jumpForce;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        Vector3 finalMove =
            move * walkSpeed * sprint +
            Vector3.up * velocity.y;

        controller.Move(finalMove * Time.deltaTime);

        isMoving = moveInput.magnitude > 0.1f;
        if (playerAnimator != null)
            playerAnimator.UpdateMovement(moveInput.magnitude);
    }

    // ================= LOOK =================

    private void HandleLook()
    {
        float sensitivity = isMobile ? mobileLookSensitivity : mouseLookSensitivity;

        float yaw = lookInput.x * sensitivity * Time.deltaTime;
        float pitch = lookInput.y * sensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * yaw);

        verticalRotation -= pitch;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        mainCamera.transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // ================= INTERACTION =================

    public void Interact()
    {
        if (currentLampSwitch != null)
            currentLampSwitch.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        LampSwitch lamp = other.GetComponent<LampSwitch>();
        if (lamp != null)
            currentLampSwitch = lamp;
    }

    private void OnTriggerExit(Collider other)
    {
        LampSwitch lamp = other.GetComponent<LampSwitch>();
        if (lamp != null && lamp == currentLampSwitch)
            currentLampSwitch = null;
    }

    // ================= FOOTSTEPS =================

    private void HandleFootsteps()
    {
        if (!controller.isGrounded) return;
        if (!isMoving) return;
        if (controller.velocity.magnitude < velocityThreshold) return;
        if (Time.time < nextStepTime) return;

        float interval =
            (!isMobile && sprintAction.ReadValue<float>() > 0)
            ? sprintStepInterval
            : walkStepInterval;

        PlayFootstep();
        nextStepTime = Time.time + interval;
    }

    private void PlayFootstep()
    {
        if (footstepSource == null || footstepSounds.Length == 0) return;

        int index = Random.Range(0, footstepSounds.Length);
        if (index == lastPlayedIndex)
            index = (index + 1) % footstepSounds.Length;

        lastPlayedIndex = index;
        footstepSource.clip = footstepSounds[index];
        footstepSource.Play();
    }
}
