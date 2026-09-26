using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int cantVidas { get; private set; }
    public UnityEvent<PlayerHealth> onDamageTaken;

    public void perderVida()
    {
        if (cantVidas <= 0)
        {
            return;
        }
        cantVidas--;
        onDamageTaken?.Invoke(this);
    }

    void Start()
    {
        cantVidas = 5;
    }
}
