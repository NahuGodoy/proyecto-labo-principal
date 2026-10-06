using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 1;
    private int currentHealth;

    [Header("Efectos")]
    public GameObject muerteEfectoPrefab; // Partículas opcionales al morir

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (muerteEfectoPrefab != null)
        {
            Instantiate(muerteEfectoPrefab, transform.position + Vector3.up, Quaternion.identity);
        }

        // Destruye al enemigo
        Destroy(gameObject);
    }
}
