using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPowerUps : MonoBehaviour
{
    public enum TipoPowerUp
    {
        Invencibilidad,
        Velocidad,
        ImanFichas,
    }

    [Header("Invencibilidad")]
    [SerializeField] private float duracionInvencibilidad = 5f;

    [Header("Velocidad")]
    [SerializeField] private float duracionVelocidad = 5f;
    [SerializeField] private float multiplicadorVelocidad = 3f;

    [Header("Imán de fichas")]
    [SerializeField] private float duracionIman = 5f;
    [SerializeField] private float radioIman = 6f;
    [SerializeField] private float velocidadAtraccion = 12f;
    [SerializeField] private LayerMask capaFichas;
    [SerializeField] private float distanciaRecoleccion = 0.6f;
    private PlayerInventory playerInventory;


    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    private Coroutine corrutinaInvencibilidad;
    private Coroutine corrutinaVelocidad;
    private Coroutine corrutinaIman;

    private float walkBase, runBase;
    private readonly Collider[] buffer = new Collider[64];

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
        playerInventory = GetComponent<PlayerInventory>();

        if (playerMovement != null)
        {
            walkBase = playerMovement.walkSpeed;
            runBase = playerMovement.runSpeed;
        }
    }

    public void ActivarPowerUp(TipoPowerUp tipoPowerUp)
    {
        switch (tipoPowerUp)
        {
            case TipoPowerUp.Invencibilidad: Reiniciar(ref corrutinaInvencibilidad, InvencibilidadTemporal()); break;
       
            case TipoPowerUp.Velocidad: Reiniciar(ref corrutinaVelocidad, VelocidadTemporal()); break;

            case TipoPowerUp.ImanFichas: Reiniciar(ref corrutinaIman, ImanTemporal()); break;
        }
    }

    //esto es por si se agarra dos power ups, asi no se suman el tiempo de ambos :p 
    private void Reiniciar(ref Coroutine actual, IEnumerator nueva)
    {
        if (actual != null) StopCoroutine(actual);
        actual = StartCoroutine(nueva);
    }



    private IEnumerator InvencibilidadTemporal()
    {
        if(playerHealth != null)
        {
            playerHealth.invencible = true;
            Debug.Log("invencible");
            yield return new WaitForSeconds(duracionInvencibilidad);
            Debug.Log("no invencible");
            playerHealth.invencible = false;
        } else
        {
            Debug.Log("No hay playerHealth");
            yield break;
        }

    }
    
    private IEnumerator VelocidadTemporal()
    {
        if(playerMovement != null)
        {
            playerMovement.walkSpeed = walkBase * multiplicadorVelocidad;
            playerMovement.runSpeed = runBase * multiplicadorVelocidad;
            Debug.Log("veloz");
            yield return new WaitForSeconds(duracionVelocidad);
            Debug.Log("lenteja");
            playerMovement.walkSpeed = walkBase;
            playerMovement.runSpeed = runBase;     
        }
    }

    private IEnumerator ImanTemporal()
    {
        float tiempo = 0f;
        while (tiempo < duracionIman)
        {
            tiempo += Time.deltaTime;
            AtraerFichas();
            yield return null;
        }
        corrutinaIman = null;
    }
    
    private void AtraerFichas()
    {
        Vector3 destino = transform.position + Vector3.up;
        int n = Physics.OverlapSphereNonAlloc(transform.position, radioIman, buffer, capaFichas, QueryTriggerInteraction.Collide);

        for (int i = 0; i < n; i++)
        {
            Collider col = buffer[i];
            Transform ficha = buffer[i].transform;
            Vector3 centro = col.bounds.center;
            Vector3 offset = centro - ficha.position;

            ficha.position = Vector3.MoveTowards(ficha.position, destino - offset, velocidadAtraccion * Time.deltaTime);

            if ((col.bounds.center - destino).sqrMagnitude <= distanciaRecoleccion * distanciaRecoleccion)
            {
                Collectable collectable = col.GetComponent<Collectable>();
                if (collectable != null && playerInventory != null)
                    collectable.Recoger(playerInventory);
                }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioIman);
    }

}
