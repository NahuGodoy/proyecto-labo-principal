
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
    [SerializeField] private LayerMask capasVision;

    [Header("Alturas")]
    [SerializeField] private float alturaDisparo = 1f;   
    [SerializeField] private float alturaObjetivo = 0.8f; 

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

    private Vector3 ObtenerOrigen()
    {
        return spawnPoint != null
            ? spawnPoint.position
            : transform.position + Vector3.up * alturaDisparo;
    }

    private Vector3 ObtenerPuntoObjetivo()
    {
        return jugador.position + Vector3.up * alturaObjetivo;
    }

    private bool TieneLineaDeVision()
    {
        if (!requiereLineaDeVision) return true;

        Vector3 origen = ObtenerOrigen();
        Vector3 destino = ObtenerPuntoObjetivo();
        Vector3 direccion = (destino - origen).normalized;
        float distancia = Vector3.Distance(origen, destino);

        if (Physics.Raycast(origen, direccion, out RaycastHit hit, distancia, capasVision))
            return hit.transform.GetComponentInParent<PlayerHealth>() != null;

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

        Vector3 origen = ObtenerOrigen();
        Vector3 direccion = (ObtenerPuntoObjetivo() - origen).normalized;

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