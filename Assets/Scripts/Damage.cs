using UnityEngine;

public class Damage : MonoBehaviour
{
    public int cantidadDano = 1;
    public float tiempoInvulnerabilidad = 1.5f; 

    private float ultimoDanoDado = 0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IntentarHacerDano(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IntentarHacerDano(collision.gameObject);
        }
    }

    private void IntentarHacerDano(GameObject playerObject)
    {
        if (Time.time >= ultimoDanoDado + tiempoInvulnerabilidad)
        {
            PlayerHealth playerHealth = playerObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.perderVida();
                ultimoDanoDado = Time.time;
            }
        }
    }
}