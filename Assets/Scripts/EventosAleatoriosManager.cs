using UnityEngine;
using System.Collections;
using TMPro;

public enum TipoEvento { Apagon, CatnipFrenzy, /*LluviaFichas*/}

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




}
