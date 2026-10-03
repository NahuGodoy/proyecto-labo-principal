using UnityEngine;

public class EnemigoLanzador : MonoBehaviour
{
    [Header("Configuración de Disparo (CA2)")]
    public GameObject proyectilPrefab;
    public Transform puntoDisparo;
    public float cadenciaDisparo = 2f; // Dispara cada 2 segundos

    [Header("Configuración de IA")]
    public float velocidadRotacion = 5f;

    private Transform jugadorTarget;
    private float tiempoSiguienteDisparo = 0f;
    private bool jugadorEnRango = false;

    void Update()
    {
        if (jugadorEnRango && jugadorTarget != null)
        {
            // 1. Apuntar hacia el jugador en el eje Y (para no inclinarse hacia arriba/abajo de forma extraña)
            Vector3 direccion = (jugadorTarget.position - transform.position).normalized;
            direccion.y = 0;
            if (direccion != Vector3.zero)
            {
                Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * velocidadRotacion);
            }

            // 2. Disparar cada 2 segundos (CA2)
            if (Time.time >= tiempoSiguienteDisparo)
            {
                Disparar();
                tiempoSiguienteDisparo = Time.time + cadenciaDisparo;
            }
        }
    }

    void Disparar()
    {
        if (proyectilPrefab == null || puntoDisparo == null) return;

        GameObject nuevoProyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);

        // Calcular dirección hacia la posición del jugador
        Vector3 dirJugador = (jugadorTarget.position + Vector3.up * 0.5f) - puntoDisparo.position;

        Proyectil scriptProyectil = nuevoProyectil.GetComponent<Proyectil>();
        if (scriptProyectil != null)
        {
            scriptProyectil.Inicializar(dirJugador);
        }
    }

    // CA1: Sensor de Proximidad
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorTarget = other.transform;
            jugadorEnRango = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnRango = false;
            jugadorTarget = null;
        }
    }
}