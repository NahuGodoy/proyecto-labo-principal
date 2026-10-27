using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.Mathematics;
namespace FortuneSpinWheel
{
    public class FortuneSpinWheel : MonoBehaviour
    {
        [Header("Referencias de la Ruleta")]
        public Image m_CircleBase;
        public Image[] m_RewardPictures; // Las 6 imagenes de las casillas en la ruleta
        public Text[] m_RewardCounts;    // Los 6 textos (+10, +3, etc.) en la ruleta

        [Header("Panel de Recompensa Final (Popup)")]
        public GameObject m_RewardPanel;
        public Text m_RewardFinalText;
        public Image m_RewardFinalImage;

        [Header("Controles")]
        public Image m_SpinButton;

        [HideInInspector]
        public bool m_IsSpinning = false;
        [HideInInspector]
        public float m_SpinSpeed = 0;
        [HideInInspector]
        public float m_Rotation = 0;
        [HideInInspector]
        public int m_RewardNumber = -1;

        [Header("Rango de Interacción")]
        public Transform jugador;          
        public float distanciaMaxima = 3.5f;

        public PlayerInventory inventario;

void Start()
{
    m_Rotation = 0;
    m_IsSpinning = false;
    m_RewardNumber = -1;

    if (m_RewardPanel != null) 
        m_RewardPanel.SetActive(false);

    if (jugador == null)
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            jugador = playerObj.transform;
        }
    }

    if (inventario == null && jugador != null)
    {
        inventario = jugador.GetComponent<PlayerInventory>();
    }
}

    void Update()
{
    bool jugadorCerca = false;
    if (jugador != null)
    {
        jugadorCerca = Vector3.Distance(transform.position, jugador.position) <= distanciaMaxima;
    }

    // compara si se presiono la E en teclado o la X/cuadrado en joystick
    bool presionoTeclado = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
    bool presionoJoystick = Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;

    
   if (!m_IsSpinning && jugadorCerca && (presionoTeclado || presionoJoystick))
    {
        if(pagarJuego(5))
                {
                    StartSpin();
                }
    }

    // Efecto de pulso en el icono ganador cuando la ruleta está detenida
    if (!m_IsSpinning)
    {
        if (m_RewardNumber >= 0 && m_RewardNumber < m_RewardPictures.Length && m_RewardPictures[m_RewardNumber] != null)
        {
            m_RewardPictures[m_RewardNumber].transform.localScale = (1 + 0.2f * Mathf.Sin(10 * Time.time)) * Vector3.one;
        }
    }
}
       IEnumerator ShowRewardMenu(int seconds)
{
    yield return new WaitForSeconds(seconds);

    if (m_RewardNumber >= 0 && m_RewardNumber < m_RewardPictures.Length)
    {
        Sprite spriteGanador = m_RewardPictures[m_RewardNumber].sprite;

        if (m_RewardPanel != null)
        {
            m_RewardPanel.SetActive(true);

            // asigna el sprite ganador (sea cruz, ficha o vida)
            if (m_RewardFinalImage != null)
            {
                m_RewardFinalImage.sprite = spriteGanador;
                EntregarRecompensa(m_RewardNumber);
            }

            // comprueba si cayo en la cruz
            bool esCruz = (spriteGanador != null && spriteGanador.name.ToLower().Contains("cross"));

            if (m_RewardFinalText != null)
            {
                if (esCruz)
                {
                    m_RewardFinalText.text = "";
                    Debug.Log("no hay remcompensa");
                    
                }
                else if (m_RewardNumber < m_RewardCounts.Length && m_RewardCounts[m_RewardNumber] != null)
                {
                    m_RewardFinalText.text = m_RewardCounts[m_RewardNumber].text;
                }
            }

            yield return new WaitForSeconds(2);
        }
    }

    yield return new WaitForSeconds(0.1f);
    Reset();
}

IEnumerator GirarHaciaObjetivo(float anguloObjetivo, int casillaGanadora)
{
    m_RewardNumber = -1;
    if (m_SpinButton != null) m_SpinButton.gameObject.SetActive(false);

    float tiempoGiro = 4.0f; 
    float tiempoTranscurrido = 0f;
     float anguloInicial = m_Rotation % 360f;

    while (tiempoTranscurrido < tiempoGiro)
    {
        tiempoTranscurrido += Time.deltaTime;
        float t = tiempoTranscurrido / tiempoGiro;

        // Curva de desaceleración suave 
        t = Mathf.SmoothStep(0f, 1f, t);

        m_Rotation = Mathf.Lerp(anguloInicial, anguloObjetivo, t);
        m_CircleBase.transform.localRotation = Quaternion.Euler(0, 0, m_Rotation);

        // Mantiene los iconos y textos derechos
        for (int i = 0; i < m_RewardPictures.Length; i++)
        {
            if (m_RewardPictures[i] != null)
                m_RewardPictures[i].transform.localRotation = Quaternion.Euler(0, 0, -m_Rotation);
        }

        yield return null;
    }

    m_Rotation = anguloObjetivo;
    m_CircleBase.transform.localRotation = Quaternion.Euler(0, 0, m_Rotation);

    m_RewardNumber = casillaGanadora;

    StartCoroutine(ShowRewardMenu(1));
}

    public void StartSpin()
    {
        if (m_IsSpinning)
            return;
        m_IsSpinning = true;
        {


        int casillaDeseada = UnityEngine.Random.Range(0,5); 
        Debug.Log(casillaDeseada);

        // Cantidad de vueltas completas fijas (por ejemplo, 5 vueltas)
        float vueltasCompletas = 5 * 360f;

        float anguloFinalObjetivo = vueltasCompletas + (casillaDeseada * 60f) + 30f;

        StartCoroutine(GirarHaciaObjetivo(anguloFinalObjetivo, casillaDeseada));
    }
    }

    public void Reset()
    {
        m_Rotation = 0;
        if (m_CircleBase != null)
            m_CircleBase.transform.localRotation = Quaternion.identity;

            m_IsSpinning = false;
            m_RewardNumber = -1;

            // Restablece el tamaño original de todos los iconos
            for (int i = 0; i < m_RewardPictures.Length; i++)
            {
                if (m_RewardPictures[i] != null)
                {
                    m_RewardPictures[i].transform.localRotation = Quaternion.identity; 
                    m_RewardPictures[i].transform.localScale = Vector3.one;
                }
            }

            if (m_SpinButton != null) m_SpinButton.gameObject.SetActive(true);
            if (m_RewardPanel != null) m_RewardPanel.SetActive(false);
    }

private void EntregarRecompensa(int indiceCasilla)
{
    if (indiceCasilla < 0 || indiceCasilla >= m_RewardPictures.Length) return;

    Sprite spriteGanador = m_RewardPictures[indiceCasilla].sprite;
    if (spriteGanador == null) return;

    string nombreSprite = spriteGanador.name.ToLower();

    if (nombreSprite.Contains("cross"))
    {
        return;
    }

    int cantidad = 0;
    if (indiceCasilla < m_RewardCounts.Length && m_RewardCounts[indiceCasilla] != null)
    {
        string textoLimpio = m_RewardCounts[indiceCasilla].text.Replace("+", "").Trim();
        int.TryParse(textoLimpio, out cantidad);
    }

    if (nombreSprite.Contains("coin-1"))
    {   
        if (SaveManager.Instance != null && SaveManager.Instance.ActiveSave != null)
        {
            SaveManager.Instance.AddFichas(cantidad);
        }
        else
        {
            Debug.LogError("No se pudo guardar la recompensa: no hay un perfil activo.");
        }
    }
    if (nombreSprite.Contains("gem-1"))
        if(inventario != null)
            inventario.AddVida(cantidad);
}


public bool pagarJuego(int costo = 5)
        {
            if (SaveManager.Instance != null && SaveManager.Instance.TrySpendFichas(costo))
            {
                return true;
            }
            Debug.Log("No hay un perfil activo o no tenés suficientes fichas para jugar.");
            return false;
                
        
        }

}    
}
    


