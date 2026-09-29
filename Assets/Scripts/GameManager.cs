using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameOverPanel;
    public Button reiniciarButton;
    public Button menuButton;
    public CameraController cameraController;

    public GameObject victoriaPanel;

    private bool juegoTerminado = false; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if(victoriaPanel != null)
        {
            victoriaPanel.SetActive(false);
        }

        if(reiniciarButton != null)
        {
            reiniciarButton.onClick.AddListener(ReiniciarEscena);
        }
        if(menuButton != null)
        {
            menuButton.onClick.AddListener(IrAMenu);
        }

    }

    public void ReiniciarEscena()
    {
        Debug.Log("clickeo en reiniciar");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void IrAMenu()
    {
        Debug.Log("clickeo en menu");
        Time.timeScale = 1f;
        //puse esto para testear pero no funciona 
        SceneManager.LoadScene("LobbyTest");
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

public void GameOver()
    {
        if (juegoTerminado) return;
        finalizarPartida();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void Victoria()
    {
        if (juegoTerminado) return;
        finalizarPartida();
        if (victoriaPanel != null)
        {
            victoriaPanel.SetActive(true);
        }
    }

    private void finalizarPartida()
    {
        juegoTerminado = true;

        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}