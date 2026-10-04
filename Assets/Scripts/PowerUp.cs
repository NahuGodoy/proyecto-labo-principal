using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PlayerPowerUps.TipoPowerUp tipoPowerUp;

    private void OnTriggerEnter(Collider other)
    {
        PlayerPowerUps playerPowerUps = other.GetComponent<PlayerPowerUps>();
        if (playerPowerUps != null)
        {
            playerPowerUps.ActivarPowerUp(tipoPowerUp);
            Destroy(gameObject);
        }
    }
}
