using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpUI : MonoBehaviour
{
    [System.Serializable]
    public struct IconoPowerUp
    {
        public PlayerPowerUps.TipoPowerUp tipo;
        public Sprite sprite;
    }

    [SerializeField] private PlayerPowerUpInventory inventario;
    [SerializeField] private IconoPowerUp[] iconos;
    [SerializeField] private Image imagenIcono;
    [SerializeField] private TMP_Text textoCantidad;
    [SerializeField] private GameObject iconoUsar;
    [SerializeField] private GameObject iconoRotar;

    private void OnEnable()
    {
        inventario.OnCambio += Redibujar;
        Redibujar();
    }

    private void OnDisable()
    {
        inventario.OnCambio -= Redibujar;
    }

    private void Redibujar()
    {
        bool hayAlgo = inventario.TieneAlgo;

        iconoUsar.SetActive(hayAlgo);
        iconoRotar.SetActive(inventario.TiposDisponibles > 1);

        if (!hayAlgo)
        {
            imagenIcono.enabled = false;
            textoCantidad.text = "";
            return;
        }

        imagenIcono.enabled = true;
        imagenIcono.sprite = BuscarSprite(inventario.Seleccionado);
        textoCantidad.text = "x" + inventario.CantidadSeleccionado;
    }

    private Sprite BuscarSprite(PlayerPowerUps.TipoPowerUp tipo)
    {
        foreach (var i in iconos)
            if (i.tipo == tipo) return i.sprite;
        return null;
    }



}
