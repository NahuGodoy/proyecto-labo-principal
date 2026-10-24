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

        playerPowerUps.ActivarPowerUp(tipoPowerUp);
        Destroy(gameObject);
    }
}
