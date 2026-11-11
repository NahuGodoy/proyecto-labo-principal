using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool JuegoEsPausado = false;
    public static bool UIBloqueaCursor = false; 

    public GameObject pauseMenuUI;
    public GameObject opcionesUI;
    [SerializeField] private Button botonControles;
    [SerializeField] private GameObject controlesUI;
    private bool volverAOpcionesDesdeControles;
    public GameObject creditosUI;

    [SerializeField] private string saveSelectionSceneName = "SaveSelection";
    [SerializeField] private AchievementMenuUI achievementMenu;
    public static string EscenaPreviaCreditos = "";

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.JuegoTerminado)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
    {
        if (controlesUI != null && controlesUI.activeSelf)
        {
            ControlsMenuUI controlsMenu = FindAnyObjectByType<ControlsMenuUI>(FindObjectsInactive.Include);
            if (controlsMenu != null && controlsMenu.CancelRebindingIfActive())
            {
                return;
            }

            CerrarControles();
        }
        else if (opcionesUI != null && opcionesUI.activeSelf)
        {
            CerrarOpciones();
        }
        else if (creditosUI != null && creditosUI.activeSelf)
            {
                CerrarCreditos();
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
        if (botonControles != null)
        {
            botonControles.onClick.AddListener(AbrirControles);
        }

        if (controlesUI != null)
        {
            controlesUI.SetActive(false);
        }
        if (pauseMenuUI != null)
        {
            AchievementSystem achievementSystem = AchievementSystem.Instance;
            if (achievementSystem == null)
            {
                achievementSystem = FindAnyObjectByType<AchievementSystem>();
            }

            if (achievementSystem == null)
            {
                achievementSystem = gameObject.AddComponent<AchievementSystem>();
            }

            if (achievementMenu == null)
            {
                achievementMenu = GetComponent<AchievementMenuUI>();
            }

            if (achievementMenu != null)
            {
                achievementMenu.Initialize(achievementSystem, this);
            }
            pauseMenuUI.SetActive(false);
        }

        Time.timeScale = 1f;
        JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDestroy()
    {
        if (botonControles != null)
        {
            botonControles.onClick.RemoveListener(AbrirControles);
        }
    }
    public void AbrirOpciones()
    {
        pauseMenuUI.SetActive(false);
        opcionesUI.SetActive(true);
    }
    public void AbrirControles()
    {
        if (controlesUI == null)
        {
            Debug.LogWarning("Asigna el panel de controles en PauseMenu.", this);
            return;
        }

        volverAOpcionesDesdeControles = opcionesUI != null && opcionesUI.activeSelf;
        if (volverAOpcionesDesdeControles)
        {
            opcionesUI.SetActive(false);
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        controlesUI.SetActive(true);
    }

    public void CerrarControles()
    {
        if (controlesUI != null)
        {
            controlesUI.SetActive(false);
        }

        if (volverAOpcionesDesdeControles && opcionesUI != null)
        {
            opcionesUI.SetActive(true);
        }
        else if (pauseMenuUI != null && JuegoEsPausado)
        {
            pauseMenuUI.SetActive(true);
        }

        volverAOpcionesDesdeControles = false;
    }
    public void CerrarOpciones()
    {
        opcionesUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void AbrirLogros()
    {
        achievementMenu?.Open();
    }

    public void CerrarLogros()
    {
        achievementMenu?.Close();
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

public void AbrirCreditos()
{
    if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
    if (creditosUI != null) creditosUI.SetActive(true);
}

public void CerrarCreditos()
{
    if (creditosUI != null) creditosUI.SetActive(false);
    if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
}


}