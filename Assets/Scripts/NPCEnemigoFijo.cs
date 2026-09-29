using UnityEngine;
using UnityEngine.AI;

public class NPCEnemigoFijo : MonoBehaviour
{

    [Header("Referencias")]
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Animator animator;

    [Header("Patrullaje")]
    public Transform[] puntosPatrulla;
    public float tiempoEsperaEnPunto = 2f;
    public float velocidadPatrulla = 2f;

    [Header("Ataque")]
    public float distanciaAtaque = 1.5f;
    public float tiempoEntreAtaques = 1.5f;

    private enum Estado { Patrullando, Atacando }
    private Estado estadoActual = Estado.Patrullando;

    private int indicePuntoActual = 0;
    private float temporizadorEspera = 0f;
    private bool esperando = false;
    private float temporizadorAtaque = 0f;
    

        void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
 
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            playerTransform = jugador.transform;
        }
        else
        {
            Debug.LogWarning("NPCEnemigoFijo: no se encontro 'Player'.");
        }
 
        if (puntosPatrulla != null && puntosPatrulla.Length > 0)
        {
            IrAlPunto(indicePuntoActual);
        }
    }

        void Update()
    {
        if (temporizadorAtaque > 0f)
        {
            temporizadorAtaque -= Time.deltaTime;
        }
 
        float distanciaAlJugador = (playerTransform != null)
            ? Vector3.Distance(transform.position, playerTransform.position)
            : Mathf.Infinity;
 
        switch (estadoActual)
        {
            case Estado.Patrullando:
                Patrullar();
                if (distanciaAlJugador <= distanciaAtaque)
                {
                    CambiarAAtaque();
                }
                break;
 
            case Estado.Atacando:
                if (distanciaAlJugador > distanciaAtaque)
                {
                    CambiarAPatrullaje();
                }
                else
                {
                    Atacar();
                }
                break;
        }
    }

        private void CambiarAAtaque()
    {
        estadoActual = Estado.Atacando;
        navMeshAgent.ResetPath();
        esperando = false; 
    }
 
    private void Atacar()
    {
        animator.SetInteger("Estado", 0);
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
 
        if (temporizadorAtaque <= 0f)
        {
            animator.SetTrigger("Atacar");
            temporizadorAtaque = tiempoEntreAtaques;
        }
    }
 
    private void CambiarAPatrullaje()
    {
        estadoActual = Estado.Patrullando;
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
            animator.SetInteger("Estado", 0);
            temporizadorEspera += Time.deltaTime;
            if (temporizadorEspera >= tiempoEsperaEnPunto)
            {
                esperando = false;
                indicePuntoActual = (indicePuntoActual + 1) % puntosPatrulla.Length;
                IrAlPunto(indicePuntoActual);
            }
            return;
        }
 
        animator.SetInteger("Estado", 1);
 
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            esperando = true;
            temporizadorEspera = 0f;
        }
    }
 
    private void IrAlPunto(int indice)
    {
        if (puntosPatrulla[indice] != null)
        {
            navMeshAgent.speed = velocidadPatrulla;
            navMeshAgent.SetDestination(puntosPatrulla[indice].position);
        }
    }
 
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }


}
