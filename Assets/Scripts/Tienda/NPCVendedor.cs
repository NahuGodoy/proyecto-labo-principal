using UnityEngine;
using UnityEngine.InputSystem;


public class NPCVendedor : MonoBehaviour
{
    public Tienda tienda;
    public GameObject avisoInteractuar;
    public float distanciaInteraccion = 2.5f;
    private bool jugadorCerca;

    // me duele todo

    void Update()
    {
        jugadorCerca = EstaCerca();

        bool mostrarAviso = jugadorCerca && !tienda.estaAbierta;
        if (avisoInteractuar != null && avisoInteractuar.activeSelf != mostrarAviso) avisoInteractuar.SetActive(mostrarAviso);
        
        if(!jugadorCerca) 
        {
            if (tienda.estaAbierta) tienda.CerrarTienda();
            return;
        }

        bool presiono = (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) || 
                (Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame);

        if (presiono )
        {
            if(tienda.estaAbierta) tienda.CerrarTienda();
            else tienda.AbrirTienda();
        }
    }
    
    bool EstaCerca()
    {
        PlayerMovement jugador = PlayerMovement.Instance;
        if (jugador == null) return false;

        Vector3 a = transform.position;
        Vector3 b = jugador.transform.position;
        a.y = 0f;
        b.y = 0f;
        return (a - b).sqrMagnitude <= distanciaInteraccion * distanciaInteraccion;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaInteraccion);
    }


}
