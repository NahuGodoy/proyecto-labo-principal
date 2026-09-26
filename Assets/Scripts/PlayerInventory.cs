using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public int cantFichas { get; private set; }

    public UnityEvent<PlayerInventory> onFichasCollected;

    public GameObject fichaPopUpPrefab; 
    public Transform puntoEfecto;       

    public void AddFicha()
    {
        cantFichas++;
        onFichasCollected?.Invoke(this);

        MostrarEfectoFicha();
    }

    private void MostrarEfectoFicha()
    {
        if (fichaPopUpPrefab != null)
        {
            Vector3 spawnPos = (puntoEfecto != null) ? puntoEfecto.position : transform.position + Vector3.up * 2f;
            Instantiate(fichaPopUpPrefab, spawnPos, Quaternion.identity);
        }
    }
}
