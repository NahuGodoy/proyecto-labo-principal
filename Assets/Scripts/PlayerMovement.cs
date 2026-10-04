using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    private Vector2 moveInput;
    public float gravity = -9.8f;
    private float verticalVelocity;
    private bool jumpHeld;
    private float jumpHoldTimer;
    private int jumpsUsed;

    public Transform modelTransform;
    public Transform cameraPivot;

    private Animator animator;

    private bool isMoving;
    private bool isRunning;
    private bool jumpRequested;
    private bool wasGrounded;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;
    public float maxJumpForce = 8f;
    public float maxJumpHoldTime = 0.25f;

    private void Start()
    {
        controller = GetComponent <CharacterController>();
        animator = GetComponent <Animator>();
    }

    public void TeleportTo(Vector3 position, Quaternion rotation)
    {
        if (controller != null)
        {
            controller.enabled = false;
        }

        transform.SetPositionAndRotation(position, rotation);
        verticalVelocity = 0f;
        wasGrounded = false;
        jumpsUsed = 0;

        if (controller != null)
        {
            controller.enabled = true;
        }
    }

    public void OnMove (InputAction.CallbackContext context)
    {
        moveInput=context.ReadValue<Vector2>();
    }

    private void Update()
    {
        float currentSpeed = (isRunning && isMoving) ? runSpeed : walkSpeed;
        bool isGrounded = controller.isGrounded || wasGrounded;

        if (isGrounded)
        {
            jumpsUsed = 0;
        }

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            if (jumpsUsed < 2 && (isGrounded || jumpsUsed > 0))
            {
                verticalVelocity = jumpForce;
                jumpsUsed++;
                jumpHoldTimer = 0f;
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
                Debug.Log("Salto aplicado");
            }
        }

        float holdDuration = Mathf.Max(0f, maxJumpHoldTime);
        float maximumJumpSpeed = Mathf.Max(jumpForce, maxJumpForce);
        if (jumpHeld && verticalVelocity > 0f && jumpHoldTimer < holdDuration)
        {
            float heldTime = Mathf.Min(Time.deltaTime, holdDuration - jumpHoldTimer);
            float holdAcceleration = holdDuration > 0f
                ? (maximumJumpSpeed - jumpForce) / holdDuration - gravity
                : 0f;
            verticalVelocity = Mathf.Min(maximumJumpSpeed, verticalVelocity + holdAcceleration * heldTime);
            jumpHoldTimer += heldTime;
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 forward = cameraPivot.forward;
        Vector3 right = cameraPivot.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDir = (forward * moveInput.y + right * moveInput.x);

        if (desiredMoveDir.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDir);
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, 15f * Time.deltaTime);
        }

        Vector3 velocity = desiredMoveDir * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
        wasGrounded = controller.isGrounded;

        updateAnimation();
    }

    public void OnSprint (InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnJump (InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpRequested = true;
            jumpHeld = true;
            Debug.Log("Salto solicitado");
        }
        else if (context.canceled)
        {
            jumpHeld = false;
        }
    }

    public void updateAnimation ()
    {
        isMoving = moveInput.magnitude > 0.1;

        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isRunning", isRunning && isMoving);
        animator.SetBool("Grounded", controller.isGrounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }
}