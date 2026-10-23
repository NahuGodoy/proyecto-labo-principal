using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool JuegoEsPausado = false;
    public static bool UIBloqueaCursor = false; 

    public GameObject pauseMenuUI;
    public GameObject opcionesUI;

    [SerializeField] private string saveSelectionSceneName = "SaveSelection";

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.JuegoTerminado)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (opcionesUI.activeSelf)
            {
                CerrarOpciones();
            }
            else if (JuegoEsPausado)
            {
                Reanudar();
            }
            else
            {
                Pausa();
            }
        }

        if (!JuegoEsPausado && !UIBloqueaCursor && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void AbrirOpciones()
    {
        pauseMenuUI.SetActive(false);
        opcionesUI.SetActive(true);
    }

    public void CerrarOpciones()
    {
        opcionesUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void Reanudar()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    void Pausa()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        JuegoEsPausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void SalirDelJuego()
    {
        VolverASeleccion();
    }

    private void VolverASeleccion()
    {
        Time.timeScale = 1f;
        JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(saveSelectionSceneName);
    }

    


}