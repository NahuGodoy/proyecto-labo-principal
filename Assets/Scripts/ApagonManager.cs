using System.Collections;
using UnityEngine;

public class ApagonManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup pantallaNegra;

    [Header("Momento aleatorio (segundos desde que empieza la partida)")]
    [SerializeField] private float tiempoMinimo = 15f;
    [SerializeField] private float tiempoMaximo = 60f;

    [Header("Apagon")]
    [SerializeField] private float duracionApagon = 5f;
    [SerializeField] private float duracionTransicion = 0.2f;

    public bool ApagonActivo { get; private set; }

    private Coroutine rutina;

    void Start()
    {
        PrepararPantalla();
        rutina = StartCoroutine(RutinaApagon());
    }

    void OnDisable()
    {
        CancelarApagon();
    }

    private IEnumerator RutinaApagon()
    {
        float espera = Random.Range(tiempoMinimo, tiempoMaximo);
        yield return new WaitForSeconds(espera);

        ApagonActivo = true;

        yield return Fundido(1f);

        yield return new WaitForSeconds(duracionApagon);

        yield return Fundido(0f);

        ApagonActivo = false;
        rutina = null;
    }

    private IEnumerator Fundido(float alphaObjetivo)
    {
        float velocidad = duracionTransicion > 0f ? 1f / duracionTransicion : float.MaxValue;
        while (!Mathf.Approximately(pantallaNegra.alpha, alphaObjetivo))
        {
            pantallaNegra.alpha = Mathf.MoveTowards(pantallaNegra.alpha, alphaObjetivo, velocidad * Time.deltaTime);
            yield return null;
        }
    }

    private void PrepararPantalla()
    {
        pantallaNegra.alpha = 0f;
        pantallaNegra.blocksRaycasts = false; // no bloquea clics en la UI
        pantallaNegra.interactable = false;
    }

    public void CancelarApagon()
    {
        if (rutina != null)
        {
            StopCoroutine(rutina);
            rutina = null;
        }

        ApagonActivo = false;

        if (pantallaNegra != null)
            pantallaNegra.alpha = 0f;
    }

    [ContextMenu("Probar apagon ahora")]
    private void ProbarApagon()
    {
        CancelarApagon();
        tiempoMinimo = tiempoMaximo = 0f;
        rutina = StartCoroutine(RutinaApagon());
    }
}