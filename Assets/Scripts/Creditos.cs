using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Creditos : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float multiplicadorAcelerar = 3f;
    public float posicionFinalY = 2500f; 

    public GameObject panelCreditos;   
    public GameObject panelMenuPausa; 

    private RectTransform rectTransform;
    private Vector2 posicionInicial;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        posicionInicial = rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = posicionInicial;
        }
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
            CerrarCreditos();
            return;
        }

        rectTransform.anchoredPosition += new Vector2(0f, velocidadActual * Time.unscaledDeltaTime);

        if (rectTransform.anchoredPosition.y >= posicionFinalY)
        {
            CerrarCreditos();
        }
    }

    public void CerrarCreditos()
    {
        if (panelCreditos != null)
        {
            panelCreditos.SetActive(false);
        }

        if (panelMenuPausa != null)
        {
            panelMenuPausa.SetActive(true);
        }
    }
}
