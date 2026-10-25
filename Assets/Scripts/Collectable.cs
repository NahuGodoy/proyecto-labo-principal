using UnityEngine;

public class Collectable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponentInParent<PlayerInventory>();
        if (playerInventory != null)
        {
            Recoger(playerInventory);
        }

    }

    public void Recoger (PlayerInventory playerInventory)
    {
        if (!gameObject.activeInHierarchy) return;

        playerInventory.AddFicha();
        gameObject.SetActive(false);
    }
}
