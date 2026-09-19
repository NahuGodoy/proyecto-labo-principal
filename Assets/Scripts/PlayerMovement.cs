using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    private Vector2 moveInput;
    public float gravity = -9.8f;
    private float verticalVelocity;

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

    private void Start()
    {
        controller = GetComponent <CharacterController>();
        animator = GetComponent <Animator>();
    }

    public void OnMove (InputAction.CallbackContext context)
    {
        moveInput=context.ReadValue<Vector2>();
    }

    private void Update()
    {
        float currentSpeed = (isRunning && isMoving) ? runSpeed : walkSpeed;
        bool isGrounded = controller.isGrounded || wasGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        if (jumpRequested)
        {
            jumpRequested = false;
            if (isGrounded)
            {
                verticalVelocity = jumpForce;
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
                Debug.Log("Salto aplicado");
            }
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
            Debug.Log("Salto solicitado");
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