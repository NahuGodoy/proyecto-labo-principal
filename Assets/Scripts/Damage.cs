using UnityEngine;

public class Damage : MonoBehaviour
{
    public int cantidadDano = 1;
    public float tiempoInvulnerabilidad = 1.5f; // Tiempo en segundos antes de poder volver a hacer daño

    private float ultimoDanoDado = 0f;

    // Se ejecuta SOLAMENTE una vez cuando el jugador entra en el collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IntentarHacerDano(other.gameObject);
        }
    }

    // Si tu collider NO es Trigger, usa OnCollisionEnter en su lugar:
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IntentarHacerDano(collision.gameObject);
        }
    }

    private void IntentarHacerDano(GameObject playerObject)
    {
        // Verifica si ya pasó el tiempo de invulnerabilidad desde el último golpe
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