using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
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
    void Update()
    {
        visual.Rotate(Vector3.up, velocidadGiro * Time.deltaTime, Space.World);
        visual.localPosition = posicionInicial + Vector3.up * (Mathf.Sin(Time.time * velocidadFlote) * alturaFlote);
    }
}
