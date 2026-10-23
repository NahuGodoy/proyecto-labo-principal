using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TemporizadorContrarreloj : MonoBehaviour
{
    public static TemporizadorContrarreloj Instance { get; private set; }

    [Header("Referencias")]
    [SerializeField] private TMP_Text textoReloj;

    [Header("Modo")]
    [SerializeField] private bool iniciarAutomaticamente = true;
    [Tooltip("Tildado: cuenta hacia 0 (contrarreloj). Destildado: cuenta hacia arriba.")]
    [SerializeField] private bool cuentaRegresiva = true;
    [SerializeField] private float tiempoInicial = 120f; 

    [Header("Colores")]
    [SerializeField] private Color colorNormal = Color.white;
    [SerializeField] private Color colorCongelado = new Color(0.25f, 0.6f, 1f); 

    [Header("Eventos")]
    public UnityEvent onTiempoAgotado;

    public float TiempoActual { get; private set; }
    public bool Congelado => congelamientoRestante > 0f;

    private bool corriendo;
    private float congelamientoRestante;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (textoReloj != null)
            textoReloj.color = colorNormal;

        Reiniciar();

        if (iniciarAutomaticamente)
            Iniciar();
    }

    void Update()
    {
        if (!corriendo) return;

        if (congelamientoRestante > 0f)
        {
            congelamientoRestante -= Time.deltaTime;
            if (congelamientoRestante <= 0f)
            {
                congelamientoRestante = 0f;
                AplicarColor(colorNormal);
            }
            return;
        }

        TiempoActual += cuentaRegresiva ? -Time.deltaTime : Time.deltaTime;

        if (cuentaRegresiva && TiempoActual <= 0f)
        {
            TiempoActual = 0f;
            corriendo = false;
            MostrarTiempo();
            onTiempoAgotado?.Invoke();
            return;
        }

        MostrarTiempo();
    }

    public void Iniciar() => corriendo = true;
    public void Detener() => corriendo = false;

    public void Reiniciar()
    {
        TiempoActual = cuentaRegresiva ? tiempoInicial : 0f;
        congelamientoRestante = 0f;
        AplicarColor(colorNormal);
        MostrarTiempo();
    }

    public void CongelarPor(float segundos)
    {
        if (segundos <= 0f) return;

        congelamientoRestante = Mathf.Max(congelamientoRestante, segundos);
        AplicarColor(colorCongelado);
    }

    private void MostrarTiempo()
    {
        if (textoReloj == null) return;

        float t = Mathf.Max(0f, TiempoActual);
        int minutos = Mathf.Min(99, (int)(t / 60f));
        int segundos = (int)(t % 60f);
        int centesimas = (int)((t * 100f) % 100f);

        textoReloj.text = string.Format("{0:00}:{1:00}:{2:00}", minutos, segundos, centesimas);
    }

    private void AplicarColor(Color color)
    {
        if (textoReloj != null)
            textoReloj.color = color;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}