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

    private MovimientoPlataforma plataformaActual;

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

        // Si estamos en el aire, dejamos de considerar la plataforma
        if (!isGrounded)
        {
            plataformaActual = null;
        }

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
                plataformaActual = null; // Liberamos la plataforma al saltar

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

        // Calculamos el desplazamiento base del personaje
        Vector3 moveDelta = velocity * Time.deltaTime;

        // Si estamos sobre una plataforma móvil, sumamos su desplazamiento de cuadro
        if (plataformaActual != null && isGrounded)
        {
            moveDelta += plataformaActual.DeltaMovimiento;
        }

        controller.Move(moveDelta);
        wasGrounded = controller.isGrounded;

        updateAnimation();
    }
        private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Detectamos si la colisión proviene del suelo (debajo de los pies)
        if (hit.normal.y > 0.5f)
        {
            MovimientoPlataforma plataforma = hit.gameObject.GetComponent<MovimientoPlataforma>();
            if (plataforma != null)
            {
                plataformaActual = plataforma;
                return;
            }
        }

        // Si pisamos una superficie regular que no sea la plataforma móvil
        if (hit.normal.y > 0.5f)
        {
            plataformaActual = null;
        }
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