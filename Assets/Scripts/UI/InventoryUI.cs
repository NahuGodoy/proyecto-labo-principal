using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI fichasTexto;
    private SharedPlayerStats sharedStats;

    private void Awake()
    {
        fichasTexto = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        sharedStats = SharedPlayerStats.GetOrCreate();
        sharedStats.FichasChanged += SetFichasTexto;
        SetFichasTexto(sharedStats.cantFichas);
    }

    private void Start()
    {
        if (sharedStats != null)
        {
            SetFichasTexto(sharedStats.cantFichas);
        }
    }

    private void OnDisable()
    {
        if (sharedStats != null)
        {
            sharedStats.FichasChanged -= SetFichasTexto;
        }
    }

    public void UpdateFichasTexto(PlayerInventory playerInventory)
    {
        if (playerInventory != null)
        {
            SetFichasTexto(playerInventory.cantFichas);
        }
    }

    private void SetFichasTexto(int cantFichas)
    {
        if (fichasTexto != null)
        {
            fichasTexto.text = cantFichas.ToString();
        }
    }
}
