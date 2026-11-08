using UnityEngine;
using UnityEngine.InputSystem;

public class Tienda : MonoBehaviour
{
    public GameObject panelTienda;
    public bool estaAbierta => panelTienda.activeSelf;
    public KeyCode teclaAbrir = KeyCode.E;

    private PlayerPowerUpInventory inventario;

    private float timeScaleAnterior = 1f;
    private bool pausaAnterior;
    private CursorLockMode lockAnterior;
    private bool cursorVisibleAnterior;

    public static bool HayTiendaAbierta;
    public static int FrameCierre = -1;

    [Header("Catálogo")]
    public ItemTiendaInfo[] items;
    public ItemTiendaUI prefabItem;
    public Transform contenedorItems;


    void Start()
    {
        foreach (var data in items)
            Instantiate(prefabItem, contenedorItems).Configurar(data, this);
    }

    void Update()
    {
        if (!estaAbierta) return;

        bool salir = (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
             (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame);

        if (salir) CerrarTienda();
    }


    public void AbrirTienda()
    {
        if (estaAbierta) return;
        panelTienda.SetActive(true);
        HayTiendaAbierta = true;

        timeScaleAnterior = Time.timeScale;
        pausaAnterior = PauseMenu.JuegoEsPausado;
        lockAnterior = Cursor.lockState;
        cursorVisibleAnterior = Cursor.visible;

        Time.timeScale = 0f;
        PauseMenu.JuegoEsPausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }


    public void CerrarTienda()
    {
        if (!estaAbierta) return;
        panelTienda.SetActive(false);
        HayTiendaAbierta = false;
        FrameCierre = Time.frameCount;


        
        Time.timeScale = timeScaleAnterior;
        PauseMenu.JuegoEsPausado = pausaAnterior;
        Cursor.lockState = lockAnterior;
        Cursor.visible = cursorVisibleAnterior;
    }
    
    // un aumento por fis

    // lo vuelvo a pausar, esto pq si aprieto E se sale el timeScale=0
    void LateUpdate()
    {
        if (!estaAbierta) return;

        
        if (Time.timeScale != 0f || !PauseMenu.JuegoEsPausado)
        

        Time.timeScale = 0f;
        PauseMenu.JuegoEsPausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Comprar(ItemTiendaInfo data)
    {

        if (inventario == null) inventario = FindAnyObjectByType<PlayerPowerUpInventory>();

        if (inventario == null)
        {
            Debug.LogWarning("[Tienda] No se encontro el PlayerPowerUpInventory del jugador t.t");
            return;
        }

        #if UNITY_EDITOR
            if (SaveManager.Instance == null || SaveManager.Instance.ActiveSave == null)
            {
                Debug.LogWarning("[Tienda] Modo prueba: compra gratis (no hay perfil)");
                inventario.Agregar(data.tipo);
                return;
            }
        #endif

        if (SaveManager.Instance == null || !SaveManager.Instance.TrySpendFichas(data.precio))
        {
            Debug.Log("No tenes suficientes fichas bleeeh");
            return;
        }

        inventario.Agregar(data.tipo);
    }


}
