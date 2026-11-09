using TMPro;
using UnityEngine;

public class VictoryPanelUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dificultadText;
    [SerializeField] private TMP_Text tiempoPartidaText;
    [SerializeField] private TMP_Text recordTiempoText;
    [SerializeField] private TMP_Text estadoRecordText;
    [SerializeField] private TMP_Text fichasRecolectadasText;
    [SerializeField] private TMP_Text multiplicadorText;
    [SerializeField] private TMP_Text fichasAcreditadasText;

    public void Show(
        string dificultad,
        bool esEscenario,
        float tiempoPartida,
        float mejorTiempo,
        bool nuevoRecord,
        int fichasRecolectadas,
        int multiplicador,
        int fichasAcreditadas)
    {
        if (dificultadText != null)
        {
            dificultadText.text = esEscenario ? $"Dificultad: {dificultad}" : dificultad;
        }

        if (tiempoPartidaText != null)
        {
            tiempoPartidaText.text = $"Tu tiempo: {FormatTime(tiempoPartida)}";
        }

        if (recordTiempoText != null)
        {
            recordTiempoText.text = mejorTiempo < 0f
                ? "Récord: sin tiempo"
                : $"Récord: {FormatTime(mejorTiempo)}";
        }

        if (estadoRecordText != null)
        {
            estadoRecordText.text = nuevoRecord
                ? "¡Nuevo récord!"
                : mejorTiempo < 0f ? "Sin récord registrado" : "Récord vigente";
        }

        if (fichasRecolectadasText != null)
        {
            fichasRecolectadasText.text = $"Fichas recolectadas: {fichasRecolectadas}";
        }

        if (multiplicadorText != null)
        {
            multiplicadorText.text = $"Multiplicador: x{multiplicador}";
        }

        if (fichasAcreditadasText != null)
        {
            fichasAcreditadasText.text = $"Fichas acreditadas: {fichasAcreditadas}";
        }
    }

    private static string FormatTime(float seconds)
    {
        return $"{Mathf.Max(0f, seconds):F2} s";
    }
}