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
    public LevelTimer levelTimer;
    [SerializeField] private string lobbySceneName = "LobbyTest";
    public TemporizadorContrarreloj temporizador;
    [SerializeField] private TMP_Text textoMotivoDerrota; // opcional
    [SerializeField] private string mensajeTiempoAgotado = "¡Se acabó el tiempo!";

    public bool JuegoTerminado => juegoTerminado;

    private bool juegoTerminado = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SharedPlayerStats.GetOrCreate().SetFichas(0);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoriaPanel != null)
            victoriaPanel.SetActive(false);

        if (reiniciarButton != null)
            reiniciarButton.onClick.AddListener(ReiniciarEscena);

        if (menuButton != null)
            menuButton.onClick.AddListener(IrAMenu);

        if (levelTimer == null)
            levelTimer = FindAnyObjectByType<LevelTimer>();

        if (levelTimer != null)
            levelTimer.TimeUp.AddListener(TiempoAgotado);
    }

    void OnDestroy()
    {
        if (levelTimer != null)
            levelTimer.TimeUp.RemoveListener(TiempoAgotado);
    }

    public void TiempoAgotado()
    {
        if (juegoTerminado) return; 

        if (textoMotivoDerrota != null)
            textoMotivoDerrota.text = mensajeTiempoAgotado;

        GameOver();
    }

    public void ReiniciarEscena()
    {
        Debug.Log("clickeo en reiniciar");
        Time.timeScale = 1f;
        PauseMenu.UIBloqueaCursor = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void IrAMenu()
    {
        Debug.Log("clickeo en menu");
        Time.timeScale = 1f;
        PauseMenu.UIBloqueaCursor = false;
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
            PauseMenu.UIBloqueaCursor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Victoria()
    {
        if (juegoTerminado) return;

        if (levelTimer == null)
        {
            levelTimer = FindAnyObjectByType<LevelTimer>();
        }

        if (levelTimer != null)
        {
            levelTimer.CompleteLevel();
        }

        if (SaveManager.Instance != null && SaveManager.Instance.ActiveSave != null && SharedPlayerStats.Instance != null)
        {
            SaveManager.Instance.AddFichas(SharedPlayerStats.Instance.cantFichas);
        }

        finalizarPartida();
        if (victoriaPanel != null)
        {
            victoriaPanel.SetActive(true);
            PauseMenu.UIBloqueaCursor = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ContinuarAlLobby()
    {
        Time.timeScale = 1f;
        PauseMenu.JuegoEsPausado = false;
        PauseMenu.UIBloqueaCursor = false;
        SceneManager.LoadScene(lobbySceneName);
    }

    private void finalizarPartida()
    {
        juegoTerminado = true;

        if (temporizador != null)
            temporizador.Detener();

        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        if (cameraController == null)
            cameraController = FindAnyObjectByType<CameraController>();

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void SkipTutorial() 
    {
        if (levelTimer == null)
        {
            levelTimer = FindAnyObjectByType<LevelTimer>();
        }

        if (levelTimer != null)
        {
            levelTimer.CompleteLevelWithoutRecordingTime();
        }
        ContinuarAlLobby();
    }
}