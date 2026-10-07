using System.Linq;
using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private float velocidad = 12f;
    [SerializeField] private float tiempoVida = 4f;
    [SerializeField] private GameObject vfxImpacto;

    private Vector3 direccion;
    private Transform origen;

    public void Inicializar(Vector3 direccionDisparo, Transform quienDispara)
    {
        direccion = direccionDisparo.normalized;
        origen = quienDispara;
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        float deltaTimeSeguro = Mathf.Min(Time.deltaTime, 0.05f); // evita saltos en frames lentos
        float distanciaFrame = velocidad * deltaTimeSeguro;

        RaycastHit[] impactos = Physics.RaycastAll(transform.position, direccion, distanciaFrame)
            .OrderBy(h => h.distance)
            .ToArray();

        foreach (RaycastHit hit in impactos)
        {
            if (origen != null && hit.transform.IsChildOf(origen)) continue;
            if (hit.transform.GetComponent<ProyectilEnemigo>() != null) continue;

            ProcesarImpacto(hit);
            return;
        }

        transform.position += direccion * distanciaFrame;
    }

    private void ProcesarImpacto(RaycastHit hit)
    {
        PlayerHealth vidaJugador = hit.transform.GetComponentInParent<PlayerHealth>();
        if (vidaJugador != null)
            vidaJugador.perderVida();

        if (vfxImpacto != null)
            Instantiate(vfxImpacto, hit.point, Quaternion.LookRotation(hit.normal));

        Destroy(gameObject);
    }
}