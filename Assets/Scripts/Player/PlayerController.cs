using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Ruch")]
    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;
    public float jumpForce = 5f;
    public float gravityMultiplier = 2f;

    [Header("Kamera")]
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70f;
    public float BottomClamp = -30f;
    public float rotationSmoothTime = 0.1f;
    public float lookSensitivity = 2f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.25f;
    public LayerMask groundMask;
    public float coyoteTime = 0.15f;
    public float jumpBufferTime = 0.15f;

    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool sprintPressed;

    private GameObject mainCamera;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private float rotationVelocity;

    private bool grounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];

        mainCamera = Camera.main.gameObject;

        cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        cinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();
        sprintPressed = sprintAction.IsPressed();

        if (jumpAction.triggered)
            lastJumpPressedTime = Time.time;
    }

    private void FixedUpdate()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        ApplyExtraGravity();
        HandleCameraRotation();
    }

    private void HandleGroundCheck()
    {
        // SphereCast pod stopami
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (grounded)
            lastGroundedTime = Time.time;
    }

    private void HandleMovement()
    {
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
        Vector3 moveVelocity = moveDir.normalized * (sprintPressed ? sprintSpeed : walkSpeed);

        Vector3 targetVelocity = new(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        rb.velocity = targetVelocity;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            float targetRotation = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    private void HandleJump()
    {
        bool canJump = (Time.time - lastGroundedTime <= coyoteTime) &&
                       (Time.time - lastJumpPressedTime <= jumpBufferTime);

        if (canJump)
        {
            Vector3 v = rb.velocity;
            v.y = jumpForce;
            rb.velocity = v;
            lastJumpPressedTime = -10f;
        }
    }

    private void ApplyExtraGravity()
    {
        if (!grounded)
        {
            rb.AddForce(Physics.gravity * (gravityMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    private void HandleCameraRotation()
    {
        if (lookInput.sqrMagnitude > 0.01f)
        {
            cinemachineTargetYaw += lookInput.x * lookSensitivity;
            cinemachineTargetPitch += lookInput.y * lookSensitivity;
        }

        cinemachineTargetYaw = Mathf.Repeat(cinemachineTargetYaw, 360f);
        cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation =
            Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f);
    }
}
