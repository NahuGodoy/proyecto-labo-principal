using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialUI : MonoBehaviour
{
    public enum TipoTutorial
    {
        Movimiento,
        Salto,
        Correr
    }

    public TipoTutorial tipoTutorial;

    public TutorialUI tutorialPadre;
    public GameObject panel;

    private bool tutorialTerminado = false;

    private void Start()
    {
        panel.SetActive(false);

        if (tutorialPadre == null)
        {
            empezarTutorial();
        }
        else
        {
            StartCoroutine(EsperarTutorialPadre());
        }
    }

    IEnumerator EsperarTutorialPadre()
    {
        while (!tutorialPadre.tutorialTerminado)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3);
        empezarTutorial();
    }

    public void empezarTutorial()
    {
        panel.SetActive(true);
        StartCoroutine(EsperarInput());
    }

    IEnumerator EsperarInput()
    {
        yield return new WaitForSeconds(5);

        while (!CondicionCompletada())
        {
            yield return null;
        }

        panel.SetActive(false);
        
        tutorialTerminado = true;
    }

    bool CondicionCompletada()
    {
        switch (tipoTutorial)
{
            case TipoTutorial.Movimiento:
                bool teclado_Movimiento = Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed ||
                                        Keyboard.current.sKey.isPressed || Keyboard.current.dKey.isPressed;

                bool joystick_Movimiento = Gamepad.current != null &&
                                            Gamepad.current.leftStick.ReadValue().magnitude > 0.1f;

                return teclado_Movimiento || joystick_Movimiento;

            case TipoTutorial.Salto:
                bool teclado_Salto = Keyboard.current.spaceKey.isPressed;
                bool joystick_Salto = Gamepad.current != null && Gamepad.current.buttonSouth.isPressed;

                return teclado_Salto || joystick_Salto;

            case TipoTutorial.Correr:
                bool teclado_Correr = (Keyboard.current.wKey.isPressed && Keyboard.current.leftShiftKey.isPressed) ||
                                    (Keyboard.current.aKey.isPressed && Keyboard.current.leftShiftKey.isPressed) ||
                                    (Keyboard.current.sKey.isPressed && Keyboard.current.leftShiftKey.isPressed) ||
                                    (Keyboard.current.dKey.isPressed && Keyboard.current.leftShiftKey.isPressed);

                bool joystick_Correr = Gamepad.current != null &&
                                        Gamepad.current.leftStick.ReadValue().magnitude > 0.1f &&
                                        Gamepad.current.leftStickButton.isPressed;

            return teclado_Correr || joystick_Correr;

            default:
                return false;
}
    }
}