using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoNPC : MonoBehaviour
{
private NavMeshAgent navMeshAgent;

[SerializeField] private Transform PuntoA;
[SerializeField] private Transform PuntoB;

private Transform destinoActual;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

       if (PuntoA != null)
       {
            destinoActual = PuntoA;
            navMeshAgent.SetDestination(destinoActual.position);
       }

    }

    void Update()
    {
        if (destinoActual == null || navMeshAgent == null)
        {
            return;
        }

      if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (destinoActual == PuntoB)
            {
                destinoActual = PuntoA;
            }
            else
            {
                destinoActual = PuntoB;
            }
        }
        navMeshAgent.SetDestination(destinoActual.position);
    }
}



