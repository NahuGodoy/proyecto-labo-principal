using UnityEngine;

public class Damage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.perderVida();
        }

    }
}
