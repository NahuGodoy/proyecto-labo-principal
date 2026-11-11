using UnityEngine;
using UnityEngine.UI;

public class MenuPrincipalController : MonoBehaviour
{
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private Toggle toggleMostrarFps;

    private const string MostrarFpsKey = "MostrarFPS";

    private void Start()
    {
        if (toggleMostrarFps == null)
        {
            Debug.LogError("Asigna el checkbox de FPS en MenuPrincipalController.", this);
            return;
        }

        bool mostrarFps = PlayerPrefs.GetInt(MostrarFpsKey, 0) == 1;
        toggleMostrarFps.SetIsOnWithoutNotify(mostrarFps);
        ContadorFps.SetVisible(mostrarFps);
        toggleMostrarFps.onValueChanged.AddListener(CambiarVisibilidadFps);
    }

    private void OnDestroy()
    {
        if (toggleMostrarFps != null)
            toggleMostrarFps.onValueChanged.RemoveListener(CambiarVisibilidadFps);
    }

    public void AbrirOpciones()
    {
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    private void CambiarVisibilidadFps(bool mostrar)
    {
        ContadorFps.SetVisible(mostrar);
    }
}

public class ContadorFps : MonoBehaviour
{
    private const string MostrarFpsKey = "MostrarFPS";
    private static ContadorFps instancia;
    private static GUIStyle estiloContador;

    private bool visible;
    private int fpsActual;
    private int framesContados;
    private float tiempoTranscurrido;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarInstancia()
    {
        instancia = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CrearInstancia()
    {
        if (instancia == null)
            new GameObject(nameof(ContadorFps)).AddComponent<ContadorFps>();
    }

    private void Awake()
    {
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);
        visible = PlayerPrefs.GetInt(MostrarFpsKey, 0) == 1;
    }

    private void Update()
    {
        if (!visible)
            return;

        framesContados++;
        tiempoTranscurrido += Time.unscaledDeltaTime;
        if (tiempoTranscurrido >= 0.25f)
        {
            fpsActual = Mathf.RoundToInt(framesContados / tiempoTranscurrido);
            framesContados = 0;
            tiempoTranscurrido = 0f;
        }
    }

    private void OnGUI()
    {
        if (!visible)
            return;

        if (estiloContador == null)
        {
            estiloContador = new GUIStyle(GUI.skin.label)
            {
                fontSize = 24,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };
        }

        string texto = $"{fpsActual} FPS";
        estiloContador.normal.textColor = Color.black;
        GUI.Label(new Rect(13f, 13f, 150f, 38f), texto, estiloContador);
        estiloContador.normal.textColor = Color.white;
        GUI.Label(new Rect(11f, 11f, 150f, 38f), texto, estiloContador);
    }

    public static void SetVisible(bool mostrar)
    {
        if (instancia == null)
            new GameObject(nameof(ContadorFps)).AddComponent<ContadorFps>();

        instancia.visible = mostrar;
        PlayerPrefs.SetInt(MostrarFpsKey, mostrar ? 1 : 0);
        PlayerPrefs.Save();
    }
}
