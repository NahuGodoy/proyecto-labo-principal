using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checkpoint tomado");
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetCheckpoint(respawnPoint != null ? respawnPoint : transform);
        }
    }
}