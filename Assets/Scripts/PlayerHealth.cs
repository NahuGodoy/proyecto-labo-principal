using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int totalVidas { get; private set; }

    public void perderVida()
    {
        totalVidas--;
    }

    void Start()
    {
        totalVidas = 5;
    }
}
