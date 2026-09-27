using UnityEngine;
using UnityEngine.AI;

public class NPCEnemigo : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;

  
    public Transform[] puntosPatrulla;
    public float tiempoEsperaEnPunto = 2f;
    public float velocidadPatrulla = 2f;


    public float radioDeteccion = 10f;
    public bool requiereLineaDeVision = true;
    public LayerMask obstaculos;


    public float velocidadPersecucion = 4.5f;
    public float radioAbandonoPersecucion = 15f;


    private enum Estado { Patrullando, Persiguiendo }
    private Estado estadoActual = Estado.Patrullando;

    private int indicePuntoActual = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    
    public Transform centroZona;
    public float radioZonaMaximo = 10f;


    private Vector3 posicionCentro;
    private float tiempoSiguienteCalculo;





    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        if (centroZona != null)
        {
            posicionCentro = centroZona.position;
        }
        else
        {
            posicionCentro = transform.position;
        }

        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            playerTransform = jugador.transform;
        }
        else
        {
            Debug.LogWarning("NPCEnemigo: no se encontro 'Player'.");
        }
 
        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
        {
            IrAlPunto(indicePuntoActual);
        }

    }
 


    void Update()
    {
        if (playerTransform == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, playerTransform.position);
        
        switch (estadoActual)
        {
            case Estado.Patrullando:
                Patrullar();
                if (PuedeVerAlJugador(distanciaAlJugador))
                {
                    CambiarAPersecucion();
                }
                break;
            
            case Estado.Persiguiendo:
                if (DebeAbandonarPersecucion())
                {
                    CambiarAPatrullaje();
                }
                else
                {
                    if (Time.time >= tiempoSiguienteCalculo)
                    {
                       navMeshAgent.SetDestination(playerTransform.position);
                       tiempoSiguienteCalculo = Time.time + 0.1f;
                    }
                }
                
                break;
        }
        
    }

    private bool PuedeVerAlJugador(float distancia)
    {
        
        if (distancia > radioDeteccion) return false;

        if (!requiereLineaDeVision) return true;
        
 
        Vector3 origen = transform.position + Vector3.up * 1f;
        Vector3 direccion = (playerTransform.position - origen).normalized;
        Vector3 destino = playerTransform.position + Vector3.up * 1f;
 
        
        if (Physics.Raycast(origen, direccion, out RaycastHit hit, radioDeteccion, obstaculos))
        {
            return false;
        }
        return true;
    }

    private bool DebeAbandonarPersecucion()
    {
        float distanciaJugadorAZona = Vector3.Distance(playerTransform.position, posicionCentro);
        if (distanciaJugadorAZona > radioZonaMaximo) return true;
 
        if (requiereLineaDeVision)
        {
            Vector3 origen = transform.position + Vector3.up * 1f;
            Vector3 direccion = (playerTransform.position - origen).normalized;
            float distanciaActual = Vector3.Distance(origen, playerTransform.position);
            if (Physics.Raycast(origen, direccion, out RaycastHit hit, distanciaActual, obstaculos))
            {
                return true;
            }
        }
        return false;
    }


    private void CambiarAPersecucion()
    {
        estadoActual = Estado.Persiguiendo;
        navMeshAgent.speed = velocidadPersecucion;
        esperando = false;
    }
 
    private void CambiarAPatrullaje()
    {
        estadoActual = Estado.Patrullando;
        navMeshAgent.speed = velocidadPatrulla;
        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
        {
            IrAlPunto(indicePuntoActual);
        }
    }
 
    private void Patrullar()
    {
        if (puntosPatrulla == null || puntosPatrulla.Length == 0) return;
 
        if (esperando)
        {
            temporizadorEspera += Time.deltaTime;
            if (temporizadorEspera >= tiempoEsperaEnPunto)
            {
                esperando = false;
                indicePuntoActual = (indicePuntoActual + 1) % puntosPatrulla.Length;
                IrAlPunto(indicePuntoActual);
            }
            return;
        }
 
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            esperando = true;
            temporizadorEspera = 0f;
        }
    }
 
    private void IrAlPunto(int indice)
    {
        if(puntosPatrulla[indice] != null)
        {
            navMeshAgent.SetDestination(puntosPatrulla[indice].position);
        }
        
    }
 

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 centro = (centroZona != null) ? centroZona.position : transform.position;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radioAbandonoPersecucion);
    }















}
