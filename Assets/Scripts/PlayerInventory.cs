using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    public int cantFichas { get; private set; }

    public void AddFicha()
    {
        cantFichas++;
    }

}
