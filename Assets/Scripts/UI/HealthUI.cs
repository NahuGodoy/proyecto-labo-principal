using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    private TextMeshProUGUI vidasTexto;
    private SharedPlayerStats sharedStats;

    private void Awake()
    {
        vidasTexto = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        sharedStats = SharedPlayerStats.GetOrCreate();
        sharedStats.VidasChanged += SetVidasTexto;
        SetVidasTexto(sharedStats.cantVidas);
    }

    private void Start()
    {
        if (sharedStats != null)
        {
            SetVidasTexto(sharedStats.cantVidas);
        }
    }

    private void OnDisable()
    {
        if (sharedStats != null)
        {
            sharedStats.VidasChanged -= SetVidasTexto;
        }
    }

    public void UpdateVidasTexto(PlayerHealth playerHealth)
    {
        if (playerHealth != null)
        {
            SetVidasTexto(playerHealth.cantVidas);
        }
    }

    private void SetVidasTexto(int cantVidas)
    {
        if (vidasTexto != null)
        {
            vidasTexto.text = cantVidas.ToString();
        }
    }
}
