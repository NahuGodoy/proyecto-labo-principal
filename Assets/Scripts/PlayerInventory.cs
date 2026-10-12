using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    private SharedPlayerStats sharedStats;

    public int cantFichas => sharedStats != null ? sharedStats.cantFichas : 0;

    public UnityEvent<PlayerInventory> onFichasCollected;

    public GameObject fichaPopUpPrefab; 
    public Transform puntoEfecto;       

    private void Awake()
    {
        sharedStats = SharedPlayerStats.GetOrCreate();
    }

    public void AddFicha()
    {
        sharedStats.AddFicha();
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
