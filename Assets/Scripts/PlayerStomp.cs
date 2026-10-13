using UnityEngine;

public class PlayerStomp : MonoBehaviour
{
    [Header("Configuración de Rebote")]
    public float fuerzaRebote = 10f; 
    public int dañoPisotón = 1;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Detectar si impactamos con el enemigo o su punto débil
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("EnemyHead"))
        {
            if (hit.point.y < transform.position.y + 0.3f)
            {
                EnemyHealth enemy = hit.gameObject.GetComponentInParent<EnemyHealth>();

                if (enemy != null)
                {
                    if (playerMovement != null)
                    {
                        playerMovement.AplicarRebote(fuerzaRebote);
                    }

                    Collider[] enemyColliders = enemy.GetComponentsInChildren<Collider>();
                    foreach (Collider col in enemyColliders)
                    {
                        col.enabled = false;
                    }

                    enemy.TakeDamage(dañoPisotón);
                }
            }
        }
    }
}