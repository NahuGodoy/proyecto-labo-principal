using UnityEngine;
using UnityEngine.InputSystem;

public class CajaFichas : MonoBehaviour
{
    public GameObject efectoDestruccion; 
    public GameObject fichaPrefab;       

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            RomperCaja(inventory);
        }

    }

    private void RomperCaja(PlayerInventory inventory)
    {
        for (int i=0; i<9; i++)
        {
            inventory.AddFicha();
        }

        if (fichaPrefab != null)
        {
            Instantiate(fichaPrefab, transform.position, Quaternion.identity);
        }

        if (efectoDestruccion != null)
        {
            Instantiate(efectoDestruccion, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}