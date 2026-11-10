using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTiendaUI : MonoBehaviour
{
    public Image icono;
    public TMP_Text nombre;
    public TMP_Text precio;
    public Button botonComprar;

    public void Configurar(ItemTiendaInfo data, Tienda tienda)
    {
        icono.sprite = data.icono;
        nombre.text = data.nombre;
        precio.text = data.precio.ToString();
        botonComprar.onClick.AddListener(() => tienda.Comprar(data));
    }
}