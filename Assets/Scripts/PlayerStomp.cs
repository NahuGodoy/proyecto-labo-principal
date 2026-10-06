using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    [Header("Configuración de Rebase/Salto")]
    public float fuerzaRebote = 8f;
    public int dañoPisotón = 1;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Se ejecuta automáticamente cuando el Character Controller choca con algo
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 1. Verifica si el objeto tocado es el enemigo o la cabeza
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("EnemyHead"))
        {
            // 2. Comprueba que el punto de impacto esté por DEBAJO del centro del jugador (pisando desde arriba)
            if (hit.point.y < transform.position.y + 0.3f)
            {
                // Busca la salud en el objeto golpeado o en sus padres
                EnemyHealth enemy = hit.gameObject.GetComponentInParent<EnemyHealth>();

                if (enemy != null)
                {
                    enemy.TakeDamage(dañoPisotón);
                    Rebotar();
                }
            }
        }
    }

    void Rebotar()
    {
        // Si usas CharacterController (que está en tu GamblerCat):
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            // Llama o resetea la velocidad Y del script de movimiento si tienes una variable pública
        }

        // Si usas Rigidbody:
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, fuerzaRebote, rb.linearVelocity.z);
        }
    }
}