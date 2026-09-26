using UnityEngine;

public class CajaFichas : MonoBehaviour
{
    public GameObject efectoDestruccion; 
    public GameObject fichaPrefab;       

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                RomperCaja(inventory);
            }
        }
    }

    private void RomperCaja(PlayerInventory inventory)
    {
        inventory.AddFicha();

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