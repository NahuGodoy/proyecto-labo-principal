using UnityEngine;
using UnityEngine.Events;
public class PlayerInventory : MonoBehaviour
{

    public int cantFichas { get; private set; }

    public UnityEvent<PlayerInventory> onFichasCollected;

    public void AddFicha()
    {
        cantFichas++;
        onFichasCollected?.Invoke(this);
    }

}
