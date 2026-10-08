using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
