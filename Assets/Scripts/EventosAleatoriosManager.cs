using UnityEngine;
using System.Collections;
using TMPro;

public enum TipoEvento { Apagon, CatnipFrenzy, LluviaFichas}

public class EventosAleatoriosManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private ApagonManager apagon;


    [Header("Variables")]
    [SerializeField] private float primerEventoMin = 15f;
    [SerializeField] private float primerEventoMax = 45f;
    [SerializeField] private float entreEventosMin = 20f;
    [SerializeField] private float entreEventosMax = 60f;

    [Header("Catnip Frenzy")]
    [SerializeField] private float duracionCatnip = 8f;
    [SerializeField] private float multiplicadorCatnip = 2f;

    [Header("Lluvia de fichas")]
    [SerializeField] private GameObject fichaPrefab;
    [SerializeField] private float duracionLluviaMin = 8f;
    [SerializeField] private float duracionLluviaMax = 15f;
    [SerializeField] private float intervaloSpawnFichas = 0.12f;
    [SerializeField] private float radioLluvia = 8f;
    [SerializeField] private float alturaLluvia = 12f;
    [SerializeField] private float tiempoVidaFicha = 20f;
    [SerializeField] private float fuerzaHorizontalFicha = 1.5f;
    [SerializeField] private string formatoAvisoLluvia = "¡Llluvia de fichas en {0}!";

    [Header("Aviso de evento")]
    [SerializeField] private TMP_Text textoAviso;
    [SerializeField] private float duracionAviso = 3f;
    [SerializeField] private string formatoAvisoCatnip = "¡Catnip Frenzy en {0}!";

    public bool EventoEnCurso { get; private set; }

    private Coroutine ciclo;
    private TipoEvento? ultimoEvento;

    void Start()
    {   
        ciclo = StartCoroutine(CicloEventos());
    }

    void OnDisable()
    {
        if (ciclo != null) StopCoroutine(ciclo);
        EventoEnCurso = false;
        if (PlayerMovement.Instance != null) PlayerMovement.Instance.multiplicadorEvento = 1f;
    }


    private IEnumerator CicloEventos()
    {
        Debug.Log("[Eventos] Ciclo iniciado");
        yield return new WaitForSeconds(Random.Range(primerEventoMin, primerEventoMax));

        while (true)
        {
            TipoEvento elegido = ElegirEvento();
            ultimoEvento = elegido;
            Debug.Log($"[Eventos] Ejecutando: {elegido}");

            EventoEnCurso = true;
            yield return EjecutarEvento(elegido);
            EventoEnCurso = false;
            Debug.Log("[Eventos] Evento terminado");

            yield return new WaitForSeconds(Random.Range(entreEventosMin, entreEventosMax));
        }
    }

    private TipoEvento ElegirEvento()
    {
        var valores = (TipoEvento[])System.Enum.GetValues(typeof(TipoEvento));
        TipoEvento candidato;
        do
        {
            candidato = valores[Random.Range(0, valores.Length)];
        }
        while (valores.Length > 1 && candidato == ultimoEvento);
        return candidato;
    }

    private IEnumerator EjecutarEvento(TipoEvento tipo)
    {
        switch (tipo)
        {
            case TipoEvento.Apagon:
                yield return apagon.EjecutarApagon();
                break;
            case TipoEvento.CatnipFrenzy:
                yield return CatnipFrenzy();
                break;
            case TipoEvento.LluviaFichas:
                yield return LluviaFichas();
                break;
        }
    }

    private IEnumerator AvisoCuentaRegresiva(string formato, float duracion)
    {
        if (textoAviso == null) yield break;

        textoAviso.gameObject.SetActive(true);
        float restante = duracion;

        while (restante > 0f)
        {
            textoAviso.text = string.Format(formato, Mathf.CeilToInt(restante));
            restante -= Time.deltaTime;
            yield return null;
        }

        textoAviso.gameObject.SetActive(false);
    }

    private IEnumerator CatnipFrenzy()
    {
        var jugador = PlayerMovement.Instance;
        if (jugador == null)
        {  
            yield break;
        }

        yield return AvisoCuentaRegresiva(formatoAvisoCatnip, duracionAviso);

        jugador.multiplicadorEvento = multiplicadorCatnip;
        Debug.Log("[Eventos] Catnip ON");
        yield return new WaitForSeconds(duracionCatnip);
        jugador.multiplicadorEvento = 1f;
        Debug.Log("[Eventos] Catnip OFF");
    }

    private IEnumerator LluviaFichas()
    {
        if (fichaPrefab == null)
        {
            Debug.LogWarning("[Eventos] No hay prefab asignado para la lluvia de fichas.");
            yield break;
        }

        yield return AvisoCuentaRegresiva(formatoAvisoLluvia, duracionAviso);

        float duracion = Random.Range(duracionLluviaMin, duracionLluviaMax);
        float restante = duracion;
        float siguienteSpawn = 0f;

        Debug.Log("[Eventos] Lluvia de fichas ON");

        while (restante > 0f)
        {
            siguienteSpawn -= Time.deltaTime;
            restante -= Time.deltaTime;

            if (siguienteSpawn <= 0f)
            {
                CrearFichaLluvia();
                siguienteSpawn = Mathf.Max(0.02f, intervaloSpawnFichas);
            }

            yield return null;
        }

        Debug.Log("[Eventos] Lluvia de fichas OFF");
    }

    private void CrearFichaLluvia()
    {
        if (PlayerMovement.Instance == null) return;

        Vector3 baseCentro = PlayerMovement.Instance.transform.position;

        Vector2 offset = Random.insideUnitCircle * radioLluvia;
        Vector3 posicion = baseCentro + new Vector3(offset.x, alturaLluvia, offset.y);

        GameObject ficha = Instantiate(fichaPrefab, posicion, Random.rotation);
        Rigidbody cuerpo = ficha.GetComponent<Rigidbody>();
        if (cuerpo == null)
            cuerpo = ficha.AddComponent<Rigidbody>();

        cuerpo.useGravity = true;
        cuerpo.AddForce(new Vector3(
            Random.Range(-fuerzaHorizontalFicha, fuerzaHorizontalFicha),
            0f,
            Random.Range(-fuerzaHorizontalFicha, fuerzaHorizontalFicha)), ForceMode.VelocityChange);

        StartCoroutine(EliminarFicha(ficha, tiempoVidaFicha));
    }

    private IEnumerator EliminarFicha(GameObject ficha, float espera)
    {
        if (ficha == null) yield break;

        yield return new WaitForSeconds(espera);
        if (ficha != null) Destroy(ficha);
    }




}
