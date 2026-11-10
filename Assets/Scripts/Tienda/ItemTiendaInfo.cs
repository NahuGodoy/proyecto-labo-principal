using UnityEngine;


public enum CategoriaItem
{
    PowerUp,        
    DobleSalto,
    LibreDeDeuda
}

[CreateAssetMenu(menuName = "Tienda/Item")]
public class ItemTiendaInfo : ScriptableObject
{
    public string nombre;
    public Sprite icono;
    public int precio;

    public CategoriaItem categoria;

    [Tooltip("Solo se usa si la categoría es PowerUp")]
    public PlayerPowerUps.TipoPowerUp tipo;

}
