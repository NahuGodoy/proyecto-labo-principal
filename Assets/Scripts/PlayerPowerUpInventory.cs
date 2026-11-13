using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerPowerUpInventory : MonoBehaviour
{
    private sealed class EstadoPersistente
    {
        public readonly Dictionary<PlayerPowerUps.TipoPowerUp, int> cantidades = new();
        public readonly List<PlayerPowerUps.TipoPowerUp> disponibles = new();
        public int indiceSeleccionado;
    }

    private static readonly Dictionary<int, EstadoPersistente> estadosPersistentes = new();

    private readonly Dictionary<PlayerPowerUps.TipoPowerUp, int> cantidades = new();
    private readonly List<PlayerPowerUps.TipoPowerUp> disponibles = new();
    private int indiceSeleccionado = 0;
    private int indiceJugador;

    private PlayerPowerUps playerPowerUps;

    public event System.Action OnCambio;

    public bool TieneAlgo => disponibles.Count > 0;
    public PlayerPowerUps.TipoPowerUp Seleccionado => disponibles[indiceSeleccionado];
    public int CantidadSeleccionado => cantidades[Seleccionado];
    public int TiposDisponibles => disponibles.Count;
    
    // Para el teclado 
    private InputAction usarAccion;
    private InputAction rotarAccion;


    private void Awake()
    {
        playerPowerUps = GetComponent<PlayerPowerUps>();

        usarAccion = new InputAction("UsarPowerUp", InputActionType.Button);
        usarAccion.AddBinding("<Keyboard>/q");
        usarAccion.AddBinding("<Gamepad>/buttonWest");

        rotarAccion = new InputAction("RotarPowerUp", InputActionType.Button);
        rotarAccion.AddBinding("<Keyboard>/r");
        rotarAccion.AddBinding("<Gamepad>/rightShoulder");

    }

    private void Start()
    {
        PlayerInput playerInput = GetComponentInParent<PlayerInput>();
        indiceJugador = playerInput != null && playerInput.playerIndex >= 0
            ? playerInput.playerIndex
            : 0;

        if (estadosPersistentes.TryGetValue(indiceJugador, out EstadoPersistente estado))
        {
            cantidades.Clear();
            disponibles.Clear();

            foreach (var tipo in estado.disponibles)
            {
                if (estado.cantidades.TryGetValue(tipo, out int cantidad) && cantidad > 0)
                {
                    disponibles.Add(tipo);
                    cantidades[tipo] = cantidad;
                }
            }

            indiceSeleccionado = disponibles.Count == 0
                ? 0
                : Mathf.Clamp(estado.indiceSeleccionado, 0, disponibles.Count - 1);
            OnCambio?.Invoke();
        }
        else
        {
            GuardarEstadoPersistente();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ReiniciarEstadosPersistentes()
    {
        estadosPersistentes.Clear();
    }

    public static void LimpiarEstadosPersistentes()
    {
        estadosPersistentes.Clear();
    }

    private void GuardarEstadoPersistente()
    {
        if (!estadosPersistentes.TryGetValue(indiceJugador, out EstadoPersistente estado))
        {
            estado = new EstadoPersistente();
            estadosPersistentes[indiceJugador] = estado;
        }

        estado.cantidades.Clear();
        estado.disponibles.Clear();
        foreach (var tipo in disponibles)
        {
            estado.disponibles.Add(tipo);
            estado.cantidades[tipo] = cantidades[tipo];
        }
        estado.indiceSeleccionado = indiceSeleccionado;
    }

    private void OnEnable()
    {
        usarAccion.performed += OnUsar;
        rotarAccion.performed += OnRotar;
        usarAccion.Enable();
        rotarAccion.Enable();
    }

    private void OnDisable()
    {
        usarAccion.performed -= OnUsar;
        rotarAccion.performed -= OnRotar;
        usarAccion.Disable();
        rotarAccion.Disable();
    }

    //esto es por si el juego esta pausado
    private void OnUsar(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0f) return;
        UsarSeleccionado();
    }

    private void OnRotar(InputAction.CallbackContext ctx)
    {
        if (Time.timeScale == 0f) return;
        Rotar();
    }

        // teto estuvo por aca

    public void Agregar(PlayerPowerUps.TipoPowerUp tipo, int cantidad = 1)
    {
        cantidades.TryGetValue(tipo, out int actual);
        cantidades[tipo] = actual + cantidad;

        if (!disponibles.Contains(tipo)) disponibles.Add(tipo);
        GuardarEstadoPersistente();
        OnCambio?.Invoke();
    }

    public void Rotar()
    {
        if (disponibles.Count == 0) return;
        indiceSeleccionado = (indiceSeleccionado + 1) % disponibles.Count;
        GuardarEstadoPersistente();
        OnCambio?.Invoke();
    }

    public void UsarSeleccionado()
    {
        if (disponibles.Count == 0) return;
        if (playerPowerUps.HayPowerUpActivo) return;

        var tipo = disponibles[indiceSeleccionado];
        playerPowerUps.ActivarPowerUp(tipo);

        cantidades[tipo]--;
        if (cantidades[tipo] <= 0)
        {
            cantidades.Remove(tipo);
            disponibles.RemoveAt(indiceSeleccionado);
            if (indiceSeleccionado >= disponibles.Count) indiceSeleccionado = 0;
        }
        GuardarEstadoPersistente();
        OnCambio?.Invoke();
    }

    
}
