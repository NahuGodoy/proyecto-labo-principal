using UnityEngine;
using UnityEngine.AI;

public class NPCEnemigoVolador : MonoBehaviour
{
    [Header("Referencias")]
    private Transform playerTransform;
    private Animator animator;

    [Header("OrientaciÃ³n")]
    public Vector3 correccionRotacion = Vector3.zero;

    [Header("Vuelo en circulos")]
    public Transform centroCirculo;
    public float radioCirculo = 10f;
    public float velocidadVuelo = 40f; //grados por segundo (?)
    public float velocidadSubida = 6f;


    [Header("Deteccion y embestida")]
    public float radioDeteccion = 10f;
    public float velocidadEmbestida = 12f;
    public float alturaSuelo = 0.5f; //la distancia del piso por la que va a empezar a subir
    public float tiempoMaxEmbestida = 3f;
    public float tiempoEnfriamiento = 2f;

    [Header ("Deteccion de Paredes")]
    public LayerMask capasObstaculos;
    public float radioColision = 0.5f;   
    public float alturaObjetivo = 1f;

    private enum Estado { Volando, Embistiendo, Subiendo}
    private Estado estadoActual = Estado.Volando;

    private Vector3 centro;
    private float angulo = 0f;
    
    private Vector3 objetivoEmbestida;
    private float temporizadorEmbestida;
    private float temporizadorEnfriamiento;
    private float sentido = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null) playerTransform = jugador.transform;
        else Debug.LogWarning("NPCEnemigoFijo: no se encontro 'Player'.");

        centro = (centroCirculo != null) ? centroCirculo.position : transform.position;

    }


    void Update()
    {   
        if (temporizadorEnfriamiento > 0f) temporizadorEnfriamiento -= Time.deltaTime;
        
        switch (estadoActual)
        {
            case Estado.Volando: Volar(); break;
            case Estado.Embistiendo: Embestir(); break;
            case Estado.Subiendo: Subir(); break;
        }
    }

    private void Volar()
    {
        float anguloSiguiente = angulo + velocidadVuelo * sentido * Time.deltaTime;
        Vector3 siguiente = PuntoDelCirculo(anguloSiguiente);
        Vector3 dir = siguiente - transform.position;

        if (dir.sqrMagnitude > 0.0001f &&
            Physics.SphereCast(transform.position, radioColision, dir.normalized, out _,
                           dir.magnitude + 0.1f, capasObstaculos, QueryTriggerInteraction.Ignore))
        {
            sentido = -sentido;   
        }
        else
        {
            angulo = anguloSiguiente;
            Mirar(dir);
            transform.position = siguiente;
        }


        if (playerTransform == null || temporizadorEnfriamiento > 0f) return;


        if (Vector3.Distance(transform.position, playerTransform.position) <= radioDeteccion)
        {
            Vector3 puntoJugador = playerTransform.position + Vector3.up * alturaObjetivo;

            if (!HayObstaculoEntre(transform.position, puntoJugador))
            {
                objetivoEmbestida = puntoJugador;
                temporizadorEmbestida = 0f;
                estadoActual = Estado.Embistiendo;

                if (animator != null) animator.SetTrigger("Embestir");
            }
        }
        
    }

    private bool HayObstaculoEntre(Vector3 origen, Vector3 destino)
    {
        Vector3 dir = destino - origen;
        return Physics.Raycast(origen, dir.normalized, dir.magnitude, capasObstaculos, QueryTriggerInteraction.Ignore);
    }

    private Vector3 PuntoDelCirculo() => PuntoDelCirculo(angulo);

    private Vector3 PuntoDelCirculo(float grados)
    {
        float rad = angulo * Mathf.Deg2Rad;
        Vector3 punto = centro + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radioCirculo;
        punto.y = centro.y;
        return punto; 
    }

    private void Mirar(Vector3 direccion)
    {
        direccion.y = 0f;
        if (direccion.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direccion);
    }

    private void Embestir()
    {
        temporizadorEmbestida += Time.deltaTime;
        Vector3 direccionMovimiento = (objetivoEmbestida - transform.position).normalized;
        Mirar(direccionMovimiento);

        float paso = velocidadEmbestida * Time.deltaTime;
        
        if (Physics.SphereCast(transform.position, radioColision, direccionMovimiento, out RaycastHit hit,
                           paso + 0.05f, capasObstaculos, QueryTriggerInteraction.Ignore))
        {
            transform.position += direccionMovimiento * Mathf.Max(0f, hit.distance - 0.05f);
            IniciarSubida();
            return;
        }


        transform.position = Vector3.MoveTowards(transform.position, objetivoEmbestida, velocidadEmbestida * Time.deltaTime);

        if (Vector3.Distance(transform.position, objetivoEmbestida) < 0.5f || 
            transform.position.y <= alturaSuelo || 
            temporizadorEmbestida >= tiempoMaxEmbestida)
        {
            IniciarSubida();
        }
    }


    private void Subir()
    {
        Vector3 destino = PuntoDelCirculo();
        Vector3 objetivo = destino;

        if (transform.position.y < destino.y - 0.1f)
        objetivo = new Vector3(transform.position.x, destino.y, transform.position.z);

        Mirar(destino - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, objetivo, velocidadSubida * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.1f) estadoActual = Estado.Volando;
        
    }

    private void IniciarSubida()
    {
        estadoActual = Estado.Subiendo;
        temporizadorEnfriamiento = tiempoEnfriamiento;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (estadoActual == Estado.Embistiendo && other.CompareTag("Player"))
        {
            IniciarSubida();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioDeteccion);
        Gizmos.color = Color.cyan;
        Vector3 c = (centroCirculo != null) ? centroCirculo.position : transform.position;
        Gizmos.DrawWireSphere(c, radioCirculo);
    }



}
