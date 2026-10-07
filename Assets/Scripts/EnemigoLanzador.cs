/*
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
*/
using UnityEngine;

public class EnemigoLanzador : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject proyectilPrefab;

    [Header("Configuracion de disparo")]
    [SerializeField] private float rangoDeteccion = 15f;
    [SerializeField] private float cadenciaDisparo = 1.5f;
    [SerializeField] private bool requiereLineaDeVision = true;
    [SerializeField] private LayerMask capasVision; // que capas puede "ver" el raycast (jugador + obstaculos)

    private Transform jugador;
    private float temporizadorDisparo;

    void Start()
    {
        if (PlayerMovement.Instance != null)
            jugador = PlayerMovement.Instance.transform;
    }

    void Update()
    {
        if (jugador == null)
        {
            if (PlayerMovement.Instance != null)
                jugador = PlayerMovement.Instance.transform;
            return;
        }

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= rangoDeteccion && TieneLineaDeVision())
        {
            MirarAlJugador();

            temporizadorDisparo -= Time.deltaTime;
            if (temporizadorDisparo <= 0f)
            {
                Disparar();
                temporizadorDisparo = cadenciaDisparo;
            }
        }
    }

    private bool TieneLineaDeVision()
    {
        if (!requiereLineaDeVision) return true;

        Vector3 origen = spawnPoint != null ? spawnPoint.position : transform.position;
        Vector3 direccion = (jugador.position - origen).normalized;
        float distancia = Vector3.Distance(origen, jugador.position);

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, distancia, capasVision))
        {
            // Si lo primero que toca no es el jugador, hay un obstaculo tapando
            return hit.transform.GetComponentInParent<PlayerHealth>() != null;
        }

        return true;
    }

    private void MirarAlJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    private void Disparar()
    {
        if (proyectilPrefab == null) return;

        Vector3 origen = spawnPoint != null ? spawnPoint.position : transform.position;
        Vector3 direccion = (jugador.position - origen).normalized;

        GameObject proyectilObj = Instantiate(proyectilPrefab, origen, Quaternion.LookRotation(direccion));
        ProyectilEnemigo proyectil = proyectilObj.GetComponent<ProyectilEnemigo>();
        if (proyectil != null)
            proyectil.Inicializar(direccion, transform);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangoDeteccion);
    }
}