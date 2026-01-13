using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Mobile Controls (Optional)")]
    [SerializeField] private VirtualJoystick moveJoystick;
    [SerializeField] private LookJoystick lookJoystick;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Jump & Gravity")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Look")]
    [SerializeField] private float mouseLookSensitivity = 2.5f;
    [SerializeField] private float mobileLookSensitivity = 180f;
    [SerializeField] private float maxLookAngle = 80f;

    // Components
    private CharacterController controller;
    private Camera mainCamera;

    // Input System
    private InputActionMap playerMap;
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

    private LampSwitch currentLampSwitch;

    // ================= LIFECYCLE =================

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;

        // Cursor solo depende de si hay mouse
        if (Mouse.current != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            Debug.LogError("PlayerController: InputActionAsset no asignado.");
            return;
        }

        playerControls.Enable();

        playerMap = playerControls.FindActionMap("Player", true);

        moveAction = playerMap.FindAction("Move", true);
        lookAction = playerMap.FindAction("Look", true);
        jumpAction = playerMap.FindAction("Jump", true);
        sprintAction = playerMap.FindAction("Sprint", true);
        interactAction = playerMap.FindAction("Interact", true);

        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
        playerControls.Disable();
    }

    private void Update()
    {
        ReadInput();
        HandleMovement();
        HandleLook();
    }

    // ================= INPUT =================

    private void ReadInput()
    {
        // PRIORIDAD: teclado / gamepad
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        // FALLBACK: controles táctiles
        if (moveInput == Vector2.zero && moveJoystick != null)
            moveInput = moveJoystick.Direction;

        if (lookInput == Vector2.zero && lookJoystick != null)
            lookInput = lookJoystick.Direction;
    }

    // ================= MOVEMENT =================

    private void HandleMovement()
    {
        float sprint =
            sprintAction != null && sprintAction.IsPressed()
            ? sprintMultiplier
            : 1f;

        Vector3 move =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        if (controller.isGrounded)
        {
            velocity.y = -2f;

            if (jumpAction != null && jumpAction.WasPressedThisFrame())
                velocity.y = jumpForce;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(
            (move * walkSpeed * sprint + Vector3.up * velocity.y)
            * Time.deltaTime
        );
    }

    // ================= LOOK =================

    private void HandleLook()
    {
        if (lookInput == Vector2.zero)
            return;

        bool usingMouse = Mouse.current != null && lookAction.activeControl?.device is Mouse;

        float sensitivity = usingMouse
            ? mouseLookSensitivity
            : mobileLookSensitivity * Time.deltaTime;

        float yaw = lookInput.x * sensitivity;
        float pitch = lookInput.y * sensitivity;

        transform.Rotate(Vector3.up * yaw);

        verticalRotation -= pitch;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

        mainCamera.transform.localRotation =
            Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // ================= INTERACTION =================

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (currentLampSwitch != null)
            currentLampSwitch.Interact();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out LampSwitch lamp))
            currentLampSwitch = lamp;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out LampSwitch lamp) && lamp == currentLampSwitch)
            currentLampSwitch = null;
    }
}
