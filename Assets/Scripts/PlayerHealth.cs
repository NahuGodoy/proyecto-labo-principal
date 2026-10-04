using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int cantVidas { get; private set; }
    public UnityEvent<PlayerHealth> onDamageTaken;

    public bool invencible = false;

    public void perderVida()
    {
        if (invencible)
        {
            return;
        }
        
        if (cantVidas <= 0)
        {
            return;
        }
        cantVidas--;
        onDamageTaken?.Invoke(this);

        if (cantVidas == 0)
            if (GameManager.Instance != null)
            {
            GameManager.Instance.GameOver();
            }
    }

    void Start()
    {
        cantVidas = 5;
    }
}
