using System;
using UnityEngine;

public class SharedPlayerStats : MonoBehaviour
{
    public static SharedPlayerStats Instance { get; private set; }

    [SerializeField, Min(0)] private int vidasIniciales = 5;
    [SerializeField, Min(0)] private int fichasIniciales;

    public int cantVidas { get; private set; }
    public int cantFichas { get; private set; }

    public event Action<int> VidasChanged;
    public event Action<int> FichasChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetInstance()
    {
        Instance = null;
    }

    public static SharedPlayerStats GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        Instance = FindAnyObjectByType<SharedPlayerStats>();
        if (Instance != null)
        {
            return Instance;
        }

        GameObject statsObject = new GameObject("SharedPlayerStats");
        return statsObject.AddComponent<SharedPlayerStats>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        cantVidas = Mathf.Max(0, vidasIniciales);
        cantFichas = Mathf.Max(0, fichasIniciales);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public int AddFicha()
    {
        cantFichas++;
        FichasChanged?.Invoke(cantFichas);
        return cantFichas;
    }

    public int PerderVida()
    {
        cantVidas = Mathf.Max(0, cantVidas - 1);
        VidasChanged?.Invoke(cantVidas);
        return cantVidas;
    }
}