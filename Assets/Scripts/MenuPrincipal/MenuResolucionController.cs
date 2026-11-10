using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuResolucionController : MonoBehaviour
{
    private const string AnchoGuardadoKey = "ResolucionAncho";
    private const string AltoGuardadoKey = "ResolucionAlto";
    private const int SegundosConfirmacion = 15;

    private readonly List<Resolution> resoluciones = new List<Resolution>();

    private TMP_Dropdown desplegable;
    private TMP_Text textoConfirmacion;
    private GameObject panelConfirmacion;
    private Coroutine cuentaRegresiva;
    private int indiceResolucionActual;
    private int anchoAnterior;
    private int altoAnterior;
    private FullScreenMode modoPantallaAnterior;

    private void Start()
    {
        desplegable = GetComponent<TMP_Dropdown>();
        if (desplegable == null)
        {
            Debug.LogError("MenuResolucionController necesita un TMP_Dropdown.", this);
            enabled = false;
            return;
        }

        if (GetComponentInParent<Canvas>() == null)
        {
            Debug.LogError("MenuResolucionController necesita estar dentro de un Canvas.", this);
            enabled = false;
            return;
        }

        CargarResoluciones();
        if (resoluciones.Count == 0)
        {
            Debug.LogError("No se encontraron resoluciones disponibles para este dispositivo.", this);
            enabled = false;
            return;
        }

        int indiceGuardado = AplicarResolucionGuardada();
        desplegable.ClearOptions();
        List<string> opciones = new List<string>();
        foreach (Resolution resolucion in resoluciones)
            opciones.Add($"{resolucion.width} x {resolucion.height}");

        desplegable.AddOptions(opciones);
        indiceResolucionActual = indiceGuardado >= 0
            ? indiceGuardado
            : Mathf.Max(0, BuscarResolucion(Screen.width, Screen.height));
        desplegable.SetValueWithoutNotify(indiceResolucionActual);
        desplegable.RefreshShownValue();
        desplegable.onValueChanged.AddListener(CambiarResolucion);
    }

    private void OnDestroy()
    {
        if (desplegable != null)
            desplegable.onValueChanged.RemoveListener(CambiarResolucion);
    }

    private void CargarResoluciones()
    {
        int[,] resolucionesComunes =
        {
            { 1280, 720 },
            { 1366, 768 },
            { 1600, 900 },
            { 1920, 1080 },
            { 2560, 1440 },
            { 3840, 2160 }
        };

        foreach (Resolution resolucion in Screen.resolutions)
        {
            for (int i = 0; i < resolucionesComunes.GetLength(0); i++)
            {
                int ancho = resolucionesComunes[i, 0];
                int alto = resolucionesComunes[i, 1];

                if (resolucion.width == ancho &&
                    resolucion.height == alto &&
                    BuscarResolucion(ancho, alto) < 0)
                {
                    resoluciones.Add(resolucion);
                    break;
                }
            }
        }

        if (BuscarResolucion(Screen.width, Screen.height) < 0)
        {
            resoluciones.Add(new Resolution
            {
                width = Screen.width,
                height = Screen.height
            });
        }
    }

    private int AplicarResolucionGuardada()
    {
        if (!PlayerPrefs.HasKey(AnchoGuardadoKey) || !PlayerPrefs.HasKey(AltoGuardadoKey))
            return -1;

        int ancho = PlayerPrefs.GetInt(AnchoGuardadoKey);
        int alto = PlayerPrefs.GetInt(AltoGuardadoKey);
        int indiceGuardado = BuscarResolucion(ancho, alto);
        if (indiceGuardado < 0)
            return -1;

        if (Screen.width != ancho || Screen.height != alto)
            Screen.SetResolution(ancho, alto, Screen.fullScreenMode);

        return indiceGuardado;
    }

    private int BuscarResolucion(int ancho, int alto)
    {
        for (int i = 0; i < resoluciones.Count; i++)
        {
            if (resoluciones[i].width == ancho && resoluciones[i].height == alto)
                return i;
        }

        return -1;
    }

    private void CambiarResolucion(int indice)
    {
        if (indice < 0 || indice >= resoluciones.Count)
            return;

        Resolution seleccionada = resoluciones[indice];
        if (seleccionada.width == Screen.width && seleccionada.height == Screen.height)
        {
            indiceResolucionActual = indice;
            return;
        }

        anchoAnterior = Screen.width;
        altoAnterior = Screen.height;
        modoPantallaAnterior = Screen.fullScreenMode;

        Screen.SetResolution(seleccionada.width, seleccionada.height, modoPantallaAnterior);
        CrearPanelConfirmacion(seleccionada.width, seleccionada.height);
        cuentaRegresiva = StartCoroutine(MostrarCuentaRegresiva(seleccionada.width, seleccionada.height));
    }

    private IEnumerator MostrarCuentaRegresiva(int ancho, int alto)
    {
        for (int segundos = SegundosConfirmacion; segundos > 0; segundos--)
        {
            textoConfirmacion.text =
                $"¿Deseas conservar la resolución {ancho} x {alto}?\nSe restaurará en {segundos} segundos.";
            yield return new WaitForSecondsRealtime(1f);
        }

        cuentaRegresiva = null;
        RevertirResolucion();
    }

    private void CrearPanelConfirmacion(int ancho, int alto)
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        panelConfirmacion = new GameObject(
            "ConfirmacionResolucion",
            typeof(RectTransform),
            typeof(Image));
        panelConfirmacion.transform.SetParent(canvas.transform, false);

        RectTransform fondo = panelConfirmacion.GetComponent<RectTransform>();
        fondo.anchorMin = Vector2.zero;
        fondo.anchorMax = Vector2.one;
        fondo.offsetMin = Vector2.zero;
        fondo.offsetMax = Vector2.zero;
        panelConfirmacion.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);

        GameObject tarjeta = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        tarjeta.transform.SetParent(panelConfirmacion.transform, false);
        RectTransform rectTarjeta = tarjeta.GetComponent<RectTransform>();
        rectTarjeta.anchorMin = new Vector2(0.5f, 0.5f);
        rectTarjeta.anchorMax = new Vector2(0.5f, 0.5f);
        rectTarjeta.sizeDelta = new Vector2(620f, 300f);
        tarjeta.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f, 1f);

        textoConfirmacion = CrearTexto(
            "Mensaje",
            tarjeta.transform,
            new Vector2(0f, 35f),
            new Vector2(560f, 160f),
            30f);
        textoConfirmacion.text = $"¿Deseas conservar la resolución {ancho} x {alto}?";

        CrearBoton("Mantener", tarjeta.transform, new Vector2(-140f, -95f), new Color(0.15f, 0.48f, 0.2f, 1f), ConfirmarResolucion);
        CrearBoton("Revertir", tarjeta.transform, new Vector2(140f, -95f), new Color(0.55f, 0.18f, 0.16f, 1f), RevertirResolucion);
    }

    private TMP_Text CrearTexto(string nombre, Transform padre, Vector2 posicion, Vector2 tamano, float fuente)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform), typeof(TextMeshProUGUI));
        objeto.transform.SetParent(padre, false);

        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = tamano;

        TextMeshProUGUI texto = objeto.GetComponent<TextMeshProUGUI>();
        texto.font = TMP_Settings.defaultFontAsset;
        texto.fontSize = fuente;
        texto.alignment = TextAlignmentOptions.Center;
        texto.color = Color.white;
        texto.raycastTarget = false;
        return texto;
    }

    private void CrearBoton(string etiqueta, Transform padre, Vector2 posicion, Color color, UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto = new GameObject("Boton" + etiqueta, typeof(RectTransform), typeof(Image), typeof(Button));
        objeto.transform.SetParent(padre, false);

        RectTransform rect = objeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = posicion;
        rect.sizeDelta = new Vector2(220f, 65f);

        objeto.GetComponent<Image>().color = color;
        objeto.GetComponent<Button>().onClick.AddListener(accion);

        TMP_Text texto = CrearTexto("Texto", objeto.transform, Vector2.zero, rect.sizeDelta, 26f);
        texto.text = etiqueta;
    }

    private void ConfirmarResolucion()
    {
        Resolution resolucion = resoluciones[desplegable.value];
        PlayerPrefs.SetInt(AnchoGuardadoKey, resolucion.width);
        PlayerPrefs.SetInt(AltoGuardadoKey, resolucion.height);
        PlayerPrefs.Save();

        indiceResolucionActual = desplegable.value;
        CerrarPanelConfirmacion();
    }

    private void RevertirResolucion()
    {
        Screen.SetResolution(anchoAnterior, altoAnterior, modoPantallaAnterior);
        desplegable.SetValueWithoutNotify(indiceResolucionActual);
        desplegable.RefreshShownValue();
        CerrarPanelConfirmacion();
    }

    private void CerrarPanelConfirmacion()
    {
        if (cuentaRegresiva != null)
        {
            StopCoroutine(cuentaRegresiva);
            cuentaRegresiva = null;
        }

        if (panelConfirmacion != null)
            Destroy(panelConfirmacion);
    }
}
