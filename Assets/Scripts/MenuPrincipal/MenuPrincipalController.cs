using UnityEngine;

public class MenuPrincipalController : MonoBehaviour
{
    [SerializeField] private GameObject panelPrincipal;
    [SerializeField] private GameObject panelOpciones;

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
}
