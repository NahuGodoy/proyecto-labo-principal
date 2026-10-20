using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApagonManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup pantallaNegra;
    [SerializeField] private TMP_Text textoCuentaRegresiva;

    [Header("Apagon automatico (momento aleatorio de la partida)")]
    [SerializeField] private bool apagonAutomatico = true;
    [SerializeField] private float tiempoMinimo = 15f;
    [SerializeField] private float tiempoMaximo = 60f;

    [Header("Fases del apagon (segundos)")]
    [SerializeField] private float duracionCuentaRegresiva = 5f;
    [SerializeField] private float duracionOscurecimiento = 3f;
    [SerializeField] private float duracionApagonTotal = 5f;
    [SerializeField] private float duracionRecuperacion = 1.5f;
    [SerializeField] private string formatoCuentaRegresiva = "Apagón en {0}";

    [Header("Disparo manual (pruebas)")]
    [SerializeField] private Key teclaDisparoApagon = Key.O;

    private Coroutine rutina;
    private bool eventoEnCurso;

    public bool ApagonEnCurso => eventoEnCurso;

    void Start()
    {
        PrepararPantalla();

        if (apagonAutomatico)
            rutina = StartCoroutine(RutinaApagon(true));
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Keyboard.current != null && Keyboard.current[teclaDisparoApagon].wasPressedThisFrame)
            DispararApagon();
#endif
    }

    void OnDisable()
    {
        CancelarApagon();
    }

    public void DispararApagon()
    {
        if (eventoEnCurso) return; 

        if (rutina != null)        
            StopCoroutine(rutina);

        rutina = StartCoroutine(RutinaApagon(false));
    }

    private IEnumerator RutinaApagon(bool esperarAleatorio)
    {
        if (esperarAleatorio)
            yield return new WaitForSeconds(Random.Range(tiempoMinimo, tiempoMaximo));

        eventoEnCurso = true;

        yield return CuentaRegresiva();
        yield return Fundido(1f, duracionOscurecimiento);
        yield return new WaitForSeconds(duracionApagonTotal);
        yield return Fundido(0f, duracionRecuperacion);

        eventoEnCurso = false;
        rutina = null;
    }

    private IEnumerator CuentaRegresiva()
    {
        float restante = duracionCuentaRegresiva;

        if (textoCuentaRegresiva != null)
            textoCuentaRegresiva.gameObject.SetActive(true);

        while (restante > 0f)
        {
            if (textoCuentaRegresiva != null)
                textoCuentaRegresiva.text = string.Format(formatoCuentaRegresiva, Mathf.CeilToInt(restante));

            restante -= Time.deltaTime;
            yield return null;
        }

        if (textoCuentaRegresiva != null)
            textoCuentaRegresiva.gameObject.SetActive(false);
    }

    private IEnumerator Fundido(float alphaObjetivo, float duracion)
    {
        if (duracion <= 0f)
        {
            pantallaNegra.alpha = alphaObjetivo;
            yield break;
        }

        float alphaInicial = pantallaNegra.alpha;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duracion;
            pantallaNegra.alpha = Mathf.Lerp(alphaInicial, alphaObjetivo, t);
            yield return null;
        }

        pantallaNegra.alpha = alphaObjetivo;
    }

    private void PrepararPantalla()
    {
        pantallaNegra.alpha = 0f;
        pantallaNegra.blocksRaycasts = false;
        pantallaNegra.interactable = false;

        if (textoCuentaRegresiva != null)
            textoCuentaRegresiva.gameObject.SetActive(false);
    }

    public void CancelarApagon()
    {
        if (rutina != null)
        {
            StopCoroutine(rutina);
            rutina = null;
        }

        eventoEnCurso = false;

        if (pantallaNegra != null)
            pantallaNegra.alpha = 0f;

        if (textoCuentaRegresiva != null)
            textoCuentaRegresiva.gameObject.SetActive(false);
    }

    [ContextMenu("Probar apagon ahora")]
    private void ProbarApagon()
    {
        if (Application.isPlaying)
            DispararApagon();
    }
}