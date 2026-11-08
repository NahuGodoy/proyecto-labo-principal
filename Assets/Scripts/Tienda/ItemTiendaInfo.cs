using UnityEngine;

[CreateAssetMenu(menuName = "Tienda/Item")]
public class ItemTiendaInfo : ScriptableObject
{
    public string nombre;
    public Sprite icono;
    public int precio;

    public PlayerPowerUps.TipoPowerUp tipo;

}
