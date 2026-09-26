using UnityEngine;

public class FichaPopUp : MonoBehaviour
{
    public float velocidadSubida = 2.5f;
    public float tiempoVida = 0.8f;

    void Start()
    {
        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(Vector3.up * velocidadSubida * Time.deltaTime, Space.World);
    }
}
