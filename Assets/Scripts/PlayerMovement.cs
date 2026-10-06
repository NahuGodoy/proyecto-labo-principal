using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    private Vector2 moveInput;
    public float gravity = -9.8f;
    private float verticalVelocity;
    private bool jumpHeld;
    private float jumpHoldTimer;
    private int jumpsUsed;

    public Transform modelTransform;
    public Transform cameraPivot;

    private bool isMoving;
    private bool isRunning;
    private bool jumpRequested;
    private bool wasGrounded;

    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpForce = 5f;
    public float maxJumpForce = 8f;
    public float maxJumpHoldTime = 0.25f;

    private MovimientoPlataforma plataformaActual;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        float currentSpeed = (isRunning && isMoving) ? runSpeed : walkSpeed;
        bool isGrounded = controller.isGrounded || wasGrounded;

        // Si estamos en el aire, dejamos de considerar la plataforma
        if (!isGrounded)
        {
            plataformaActual = null;
        }

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
                plataformaActual = null; // Liberamos la plataforma al saltar

                jumpsUsed++;
                jumpHoldTimer = 0f;
                if (animator != null)
                {
                    animator.SetTrigger("Jump");
                }
            }
        }

        // Aplicar gravedad continua
        verticalVelocity += gravity * Time.deltaTime;

        // Calcular dirección según cámara
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

        // Vector final de velocidad
        Vector3 finalVelocity = desiredMoveDir * currentSpeed;
        finalVelocity.y = verticalVelocity;

        // Un solo movimiento por fotograma
        controller.Move(finalVelocity * Time.deltaTime);
        wasGrounded = controller.isGrounded;

        updateAnimation();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpRequested = true;
        }
        else if (context.canceled)
        {
            jumpHeld = false;
        }
    }

    public void updateAnimation()
    {
        isMoving = moveInput.magnitude > 0.1f;

        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
            animator.SetBool("isRunning", isRunning && isMoving);
            animator.SetBool("Grounded", controller.isGrounded);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
    }

    // Método corregido: Asigna la fuerza a la variable real de velocidad vertical
    public void AplicarRebote(float fuerza)
    {
        verticalVelocity = fuerza;
        if (animator != null)
        {
            animator.SetTrigger("Jump");
        }
    }

    
}