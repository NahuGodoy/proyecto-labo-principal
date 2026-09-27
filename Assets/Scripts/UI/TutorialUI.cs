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
                return Keyboard.current.wKey.wasPressedThisFrame ||
                       Keyboard.current.aKey.wasPressedThisFrame ||
                       Keyboard.current.sKey.wasPressedThisFrame ||
                       Keyboard.current.dKey.wasPressedThisFrame;

            case TipoTutorial.Salto:
                return Keyboard.current.spaceKey.wasPressedThisFrame;

            case TipoTutorial.Correr:
                return Keyboard.current.leftShiftKey.wasPressedThisFrame;

            default:
                return false;
        }
    }
}