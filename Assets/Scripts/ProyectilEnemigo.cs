using UnityEngine;

public class Proyectil : MonoBehaviour
{
    public float velocidad = 10f;
    public int daño = 1;
    public float tiempoVida = 5f; // Para que se destruya solo si no golpea nada

    private Vector3 direccion;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    public void Inicializar(Vector3 dir)
    {
        direccion = dir.normalized;
    }

    void Update()
    {
        transform.position += direccion * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // CA3: Si el jugador está realizando un giro / ataque, se destruye el proyectil
        if (other.CompareTag("Player"))
        {
            // Verificamos si el jugador está atacando o rodando
            // Ajusta "PlayerAttack" o el nombre de tu script de ataque si usas uno
            PlayerStomp stomp = other.GetComponent<PlayerStomp>();

            // Si el jugador recibe el impacto de lleno sin defenderse:
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.perderVida();
            }

            Destroy(gameObject);
        }
    }

    // CA3: Permite que el ataque/giro del jugador destruya el proyectil
    public void DestruirPorGiro()
    {
        Destroy(gameObject);
    }
}