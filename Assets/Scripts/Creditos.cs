using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    [Header("Movimiento")]
    public float scrollSpeed = 40f;
    public float multiplicadorAcelerar = 3f;
    public float posicionFinalY = 2500f; 

    public string escenaSiguiente = "PruebaApagon"; 

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        float velocidadActual = scrollSpeed;
        bool acelerarTeclado = Keyboard.current != null && (Keyboard.current.spaceKey.isPressed || Keyboard.current.eKey.isPressed);
        bool acelerarGamepad = Gamepad.current != null && Gamepad.current.buttonSouth.isPressed;

        if (acelerarTeclado || acelerarGamepad)
        {
            velocidadActual *= multiplicadorAcelerar;
        }

        bool salirTeclado = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;
        bool salirGamepad = Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame;

        if (salirTeclado || salirGamepad)
        {
            TerminarCreditos();
            return;
        }

        rectTransform.anchoredPosition += new Vector2(0f, velocidadActual * Time.deltaTime);

        if (rectTransform.anchoredPosition.y >= posicionFinalY)
        {
            TerminarCreditos();
        }
    }

    private void TerminarCreditos()
    {
        if (!string.IsNullOrEmpty(escenaSiguiente) && Application.CanStreamedLevelBeLoaded(escenaSiguiente))
        {
            SceneManager.LoadScene(escenaSiguiente);
        }
    }
}
