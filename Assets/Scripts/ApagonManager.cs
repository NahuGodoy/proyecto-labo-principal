using System.Collections;
using TMPro;
using UnityEngine;

public class ApagonManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private CanvasGroup pantallaNegra;
    [SerializeField] private TMP_Text textoCuentaRegresiva;

    [Header("Apagones aleatorios")]
    [SerializeField] private bool apagonAutomatico = true;

    [Tooltip("Espera antes del PRIMER apagon (segundos desde que empieza la partida)")]
    [SerializeField] private float primerApagonMin = 15f;
    [SerializeField] private float primerApagonMax = 60f;

    [Tooltip("Espera entre un apagon y el siguiente (cuenta desde que vuelve la luz)")]
    [SerializeField] private float entreApagonesMin = 20f;
    [SerializeField] private float entreApagonesMax = 60f;

    [Header("Fases del apagon (segundos)")]
    [SerializeField] private float duracionCuentaRegresiva = 3f;
    [SerializeField] private float duracionOscurecimiento = 3f;
    [SerializeField] private float duracionApagonTotal = 5f;
    [SerializeField] private float duracionRecuperacion = 1.5f;
    [SerializeField] private string formatoCuentaRegresiva = "Apagón en {0}";

    public bool ApagonEnCurso { get; private set; }

    private Coroutine rutina;

    void Start()
    {
        PrepararCanvas();

        if (apagonAutomatico)
            rutina = StartCoroutine(CicloApagones());
    }

    void OnDisable()
    {
        CancelarApagon();
    }

    private IEnumerator CicloApagones()
    {
        yield return new WaitForSeconds(Random.Range(primerApagonMin, primerApagonMax));

        while (true)
        {
            yield return EjecutarApagon();
            yield return new WaitForSeconds(Random.Range(entreApagonesMin, entreApagonesMax));
        }
    }

    public IEnumerator EjecutarApagon()
    {
        ApagonEnCurso = true;

        yield return CuentaRegresiva();
        yield return Fundido(1f, duracionOscurecimiento);
        yield return new WaitForSeconds(duracionApagonTotal);
        yield return Fundido(0f, duracionRecuperacion);

        ApagonEnCurso = false;
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

    private void PrepararCanvas()
    {
        // La pantalla negra ocupa toda la pantalla (stretch con margenes en 0)
        RectTransform rect = pantallaNegra.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        pantallaNegra.alpha = 0f;
        pantallaNegra.blocksRaycasts = false; // no bloquea clics en la UI
        pantallaNegra.interactable = false;

        Canvas canvas = pantallaNegra.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
        }

        if (textoCuentaRegresiva != null)
        {
            RectTransform rectTexto = textoCuentaRegresiva.rectTransform;
            rectTexto.anchorMin = rectTexto.anchorMax = new Vector2(0.5f, 0.5f);
            rectTexto.pivot = new Vector2(0.5f, 0.5f);
            rectTexto.anchoredPosition = Vector2.zero;
            rectTexto.SetAsLastSibling();

            textoCuentaRegresiva.alignment = TextAlignmentOptions.Center;
            textoCuentaRegresiva.textWrappingMode = TextWrappingModes.NoWrap;
            textoCuentaRegresiva.raycastTarget = false;
            textoCuentaRegresiva.gameObject.SetActive(false);
        }
    }

    public void CancelarApagon()
    {
        if (rutina != null)
        {
            StopCoroutine(rutina);
            rutina = null;
        }

        ApagonEnCurso = false;

        if (pantallaNegra != null)
            pantallaNegra.alpha = 0f;

        if (textoCuentaRegresiva != null)
            textoCuentaRegresiva.gameObject.SetActive(false);
    }
}