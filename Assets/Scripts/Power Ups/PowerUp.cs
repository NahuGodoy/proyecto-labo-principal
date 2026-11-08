using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PlayerPowerUps.TipoPowerUp tipoPowerUp;

    [Header("Animacion")]
    private Transform visual;
    [SerializeField] private float velocidadGiro = 90f;
    [SerializeField] private float alturaFlote = 0.25f;
    [SerializeField] private float velocidadFlote = 2f;
    
    private Vector3 posicionInicial;

    // para el inventario nyejeje
    [SerializeField] private bool irAlInventario = true;

    private void Awake()
    {
        if (visual == null) visual = transform;
    }

    private void Start()
    {
        posicionInicial = visual.localPosition;
    }

    private void Update()
    {
        visual.Rotate(Vector3.up, velocidadGiro * Time.deltaTime, Space.World);
        visual.localPosition = posicionInicial + Vector3.up * (Mathf.Sin(Time.time * velocidadFlote) * alturaFlote);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PowerUp] Trigger con: {other.name}");

        PlayerPowerUps playerPowerUps = other.GetComponent<PlayerPowerUps>();
        
        if (playerPowerUps == null)
        {
            Debug.LogWarning($"[PowerUp] {other.name} no tiene PlayerPowerUps");
            return;
        }

        if (irAlInventario)
        {
            PlayerPowerUpInventory inv = other.GetComponent<PlayerPowerUpInventory>();
            if (inv != null) inv.Agregar(tipoPowerUp);
            else Debug.LogWarning($"[PowerUp] {other.name} no tiene PlayerPowerUpInventory");
        }
        else
        {
            playerPowerUps.ActivarPowerUp(tipoPowerUp);
        }
        Destroy(gameObject);
    }
}
