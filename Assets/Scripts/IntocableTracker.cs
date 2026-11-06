using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class IntocableTracker
{
    public static bool Intocable { get; private set; } = true;

    private static readonly HashSet<string> escenariosConLogro = new HashSet<string>();

    public static event Action<string> OnLogroObtenido; // para mostrar un aviso, si queres

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Inicializar()
    {
        escenariosConLogro.Clear();
        Intocable = true;
        SceneManager.sceneLoaded -= AlCargarEscena;
        SceneManager.sceneLoaded += AlCargarEscena;
    }

    // Cada vez que se carga una escena (incluido Reiniciar) se vuelve a empezar limpio
    private static void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        Intocable = true;
    }

    public static void RegistrarGolpe()
    {
        Intocable = false;
    }

    
    public static bool RegistrarVictoria()
    {
        if (!Intocable) return false;

        string escenario = SceneManager.GetActiveScene().name;
        if (escenario.Contains("Tutorial")) return false; 

        // Se registra por escenario y nunca se pisa lo anterior
        bool nuevoEnEsteEscenario = escenariosConLogro.Add(escenario);

        if (AchievementSystem.Instance != null)
            AchievementSystem.Instance.Unlock(AchievementSystem.IntocableId);

        if (nuevoEnEsteEscenario)
        {
            Debug.Log("Intocable en " + escenario + " (total en la sesion: " + escenariosConLogro.Count + ")");
            OnLogroObtenido?.Invoke(escenario);
        }

        return nuevoEnEsteEscenario;
    }

    public static bool TieneLogroEn(string escenario) => escenariosConLogro.Contains(escenario);
    public static int CantidadEscenarios => escenariosConLogro.Count;

}
