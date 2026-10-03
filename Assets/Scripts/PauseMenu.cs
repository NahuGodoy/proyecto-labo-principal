using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool JuegoEsPausado = false;

    public GameObject pauseMenuUI;
    [SerializeField] private string saveSelectionSceneName = "SaveSelection";

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (JuegoEsPausado)
            {
                Reanudar();
            }
            else
            {
                Pausa();
            }
        }
    }

    void Start()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        JuegoEsPausado = false;
    }

    public void Reanudar()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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