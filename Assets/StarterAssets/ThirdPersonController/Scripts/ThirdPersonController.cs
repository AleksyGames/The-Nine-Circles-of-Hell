using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Ustawienia ruchu")]
        public float walkSpeed = 2f;
        public float runSpeed = 5f;
        public float jumpHeight = 1.2f;
        public float gravity = -15f;
        public float rotationSmoothTime = 0.1f;
        public LayerMask groundLayers;

        [Header("Kamera")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70f;
        public float BottomClamp = -30f;

        private CharacterController controller;
        private StarterAssetsInputs input;
        private PlayerInput playerInput;
        private Animator animator;
        private GameObject mainCamera;

        private float rotationVelocity;
        private float verticalVelocity;
        private float cinemachineTargetYaw;
        private float cinemachineTargetPitch;
        private bool grounded;

        private readonly float groundedCheckOffset = -0.3f;
        private readonly float groundedCheckRadius = 0.4f;

        private float lastNonStrafeTargetAngle = 0f;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            input = GetComponent<StarterAssetsInputs>();
            playerInput = GetComponent<PlayerInput>();
            animator = GetComponent<Animator>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
        }

        private void Update()
        {
            GroundedCheck();
            HandleMovement();
            HandleJump();
            HandleAttack();
        }

        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePos = new Vector3(transform.position.x, transform.position.y + groundedCheckOffset, transform.position.z);
            grounded = Physics.CheckSphere(spherePos, groundedCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
            animator.SetBool("Grounded", grounded);
        }

        private void HandleMovement()
        {
            float targetSpeed = input.sprint ? runSpeed : walkSpeed;
            float inputMag = input.move.magnitude;
            if (inputMag < 0.01f) targetSpeed = 0f;

            // 🔹 Kierunek względem kamery
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 🔹 Kierunek ruchu
            Vector3 moveDir = camForward * input.move.y + camRight * input.move.x;
            if (moveDir.sqrMagnitude > 0.0001f)
                moveDir.Normalize();

            // 🔹 Ruch postaci
            controller.Move(moveDir * targetSpeed * Time.deltaTime + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);

            // 🔹 Obrót postaci zawsze w stronę kamery
            Vector3 lookDir = camForward;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                float targetAngle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0f, rotation, 0f);
            }

            // 🔹 Animacje
            animator.SetFloat("Speed", targetSpeed * inputMag, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveX", input.move.x, 0.1f, Time.deltaTime);
            animator.SetFloat("MoveY", input.move.y, 0.1f, Time.deltaTime);
        }
    
    

        private void HandleJump()
        {
            if (grounded && input.jump)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetTrigger("Jump");
                input.jump = false;
            }

            if (!grounded)
                verticalVelocity += gravity * Time.deltaTime;
            else if (verticalVelocity < 0)
                verticalVelocity = -2f;
        }

        private void HandleAttack()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                animator.SetTrigger("Attack");
            }
        }

        private void HandleCameraRotation()
        {
            if (input.look.sqrMagnitude > 0.01f)
            {
                cinemachineTargetYaw += input.look.x;
                cinemachineTargetPitch += input.look.y;
            }

            cinemachineTargetYaw = Mathf.Repeat(cinemachineTargetYaw, 360f);
            cinemachineTargetPitch = Mathf.Clamp(cinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(cinemachineTargetPitch, cinemachineTargetYaw, 0f);
        }
    }
}
