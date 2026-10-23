using UnityEngine;
using UnityEngine.UI;

public class OpcionHUD : MonoBehaviour
{
    [SerializeField] private GameObject[] elementosHUD;
    [SerializeField] private Toggle casillaHUD;

    private const string Clave = "MostrarHUD";

    void Start()
    {
        bool mostrar = PlayerPrefs.GetInt(Clave, 1) == 1;
        casillaHUD.SetIsOnWithoutNotify(mostrar);
        AplicarHUD(mostrar);
        casillaHUD.onValueChanged.AddListener(CambiarHUD);
    }

    void CambiarHUD(bool mostrar)
    {
        AplicarHUD(mostrar);
        PlayerPrefs.SetInt(Clave, mostrar ? 1 : 0);
    }

    void AplicarHUD(bool mostrar)
    {
        foreach (GameObject elemento in elementosHUD)
        {
            if (elemento != null)
                elemento.SetActive(mostrar);
        }
    }
}
