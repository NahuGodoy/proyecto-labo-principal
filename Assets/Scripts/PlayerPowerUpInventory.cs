using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerPowerUpInventory : MonoBehaviour
{

    private readonly Dictionary<PlayerPowerUps.TipoPowerUp, int> cantidades = new();
    private readonly List<PlayerPowerUps.TipoPowerUp> disponibles = new();
    private int indiceSeleccionado = 0;

    private PlayerPowerUps playerPowerUps;

    public event System.Action OnCambio;

    public bool TieneAlgo => disponibles.Count > 0;
    public PlayerPowerUps.TipoPowerUp Seleccionado => disponibles[indiceSeleccionado];
    public int CantidadSeleccionado => cantidades[Seleccionado];
    

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame) UsarSeleccionado();
        if (Keyboard.current.qKey.wasPressedThisFrame) Rotar();
    }



    private void Awake()
    {
        playerPowerUps = GetComponent<PlayerPowerUps>();
    }

    public void Agregar(PlayerPowerUps.TipoPowerUp tipo, int cantidad = 1)
    {
        cantidades.TryGetValue(tipo, out int actual);
        cantidades[tipo] = actual + cantidad;

        if (!disponibles.Contains(tipo)) disponibles.Add(tipo);
        OnCambio?.Invoke();
    }

    public void Rotar()
    {
        if (disponibles.Count == 0) return;
        indiceSeleccionado = (indiceSeleccionado + 1) % disponibles.Count;
        OnCambio?.Invoke();
    }

    public void UsarSeleccionado()
    {
        if (disponibles.Count == 0) return;

        var tipo = disponibles[indiceSeleccionado];
        playerPowerUps.ActivarPowerUp(tipo);

        cantidades[tipo]--;
        if (cantidades[tipo] <= 0)
        {
            cantidades.Remove(tipo);
            disponibles.RemoveAt(indiceSeleccionado);
            if (indiceSeleccionado >= disponibles.Count) indiceSeleccionado = 0;
        }
        OnCambio?.Invoke();
    }


    // teto estuvo por aqui


    
}
