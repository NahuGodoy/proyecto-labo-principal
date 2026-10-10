using UnityEngine;
using UnityEngine.AI;

public class NPCEnemigoVolador : MonoBehaviour
{
    [Header("Referencias")]
    private Transform playerTransform;
    private Animator animator;

    [Header("Orientación")]
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


    private enum Estado { Volando, Embistiendo, Subiendo}
    private Estado estadoActual = Estado.Volando;

    private Vector3 centro;
    private float angulo = 0f;
    
    private Vector3 objetivoEmbestida;
    private float temporizadorEmbestida;
    private float temporizadorEnfriamiento;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null) playerTransform = jugador.transform;
        else Debug.LogWarning("NPCEnemigoFijo: no se encontro 'Player'.");

        centro = (centroCirculo != null) ? centroCirculo.position
                 : transform.position;

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
        angulo += velocidadVuelo * Time.deltaTime;
        Vector3 destino = PuntoDelCirculo();
        Mirar(destino - transform.position);
        transform.position = destino;

        if (playerTransform != null && temporizadorEnfriamiento <= 0f &&
            Vector3.Distance(transform.position, playerTransform.position) <= radioDeteccion)
        {
            objetivoEmbestida = playerTransform.position;
            temporizadorEmbestida = 0f;
            estadoActual = Estado.Embistiendo;
        }
    }

    private Vector3 PuntoDelCirculo()
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
        Mirar(destino - transform.position);
        transform.position = Vector3.MoveTowards(transform.position, destino, velocidadSubida * Time.deltaTime);

        if (Vector3.Distance(transform.position, destino) < 0.1f)
        {
            estadoActual = Estado.Volando;
        }
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
