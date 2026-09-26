using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    private TextMeshProUGUI vidasTexto;
    void Start()
    {
        vidasTexto = GetComponent<TextMeshProUGUI>();
    }
    public void UpdateVidasTexto(PlayerHealth playerHealth)
    {
        vidasTexto.text = playerHealth.cantVidas.ToString();
    }
}
