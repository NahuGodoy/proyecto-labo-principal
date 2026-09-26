using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI fichasTexto;
    void Start()
    {
        fichasTexto = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateFichasTexto(PlayerInventory playerInventory)
    {
        fichasTexto.text = playerInventory.cantFichas.ToString();
    }

}
