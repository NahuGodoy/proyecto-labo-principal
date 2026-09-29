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
        Debug.Log("entro?");
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
                Debug.Log("llego al punto B y canmbia al A");
                destinoActual = PuntoA;
            }
            else
            {
                Debug.Log("llego al punto A y cambia al B");
                destinoActual = PuntoB;
            }
        }
        navMeshAgent.SetDestination(destinoActual.position);
    }
}



