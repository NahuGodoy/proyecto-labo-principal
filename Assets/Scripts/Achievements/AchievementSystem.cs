using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementDefinition
{
    public string id;
    public string displayName;
    [TextArea] public string description;
    [TextArea] public string unlockedDescription;
    public Sprite icon;
    public bool isPlaceholder;

    public AchievementDefinition(string id, string displayName, string description, bool isPlaceholder = false,
        string unlockedDescription = "")
    {
        this.id = id;
        this.displayName = displayName;
        this.description = description;
        this.unlockedDescription = unlockedDescription;
        this.isPlaceholder = isPlaceholder;
    }
}

public class AchievementSystem : MonoBehaviour
{
    public const string TutorialCompletedId = "tutorial_completed";

    private static AchievementSystem instance;
    [SerializeField] private AchievementCatalog catalog;

    private readonly List<AchievementDefinition> fallbackAchievements = new List<AchievementDefinition>
    {
        new AchievementDefinition(TutorialCompletedId, "Tutorial completado", "Completa el recorrido del tutorial sin omitirlo."),
        new AchievementDefinition("placeholder_collector", "Coleccionista", "Logro en desarrollo.", true),
        new AchievementDefinition("placeholder_speedrunner", "A toda velocidad", "Logro en desarrollo.", true),
        new AchievementDefinition("placeholder_survivor", "Intocable", "Logro en desarrollo.", true),
        new AchievementDefinition("placeholder_explorer", "Explorador", "Logro en desarrollo.", true),
        new AchievementDefinition("placeholder_master", "Maestro", "Logro en desarrollo.", true)
    };

    public static AchievementSystem Instance => instance;
    public IReadOnlyList<AchievementDefinition> Achievements => catalog != null
        && catalog.achievements != null
        && catalog.achievements.Count > 0
            ? catalog.achievements
            : fallbackAchievements;

    public event Action<string> AchievementUnlocked;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetInstance()
    {
        instance = null;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstanceBeforeSceneLoad()
    {
        if (instance == null)
        {
            new GameObject(nameof(AchievementSystem)).AddComponent<AchievementSystem>();
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        if (catalog == null)
        {
            catalog = Resources.Load<AchievementCatalog>("AchievementCatalog");
        }
    }

    public bool Unlock(string achievementId)
    {
        if (string.IsNullOrEmpty(achievementId) || !ContainsAchievement(achievementId))
        {
            return false;
        }

        if (SaveManager.Instance == null || SaveManager.Instance.ActiveSave == null)
        {
            return false;
        }

        bool unlocked = SaveManager.Instance.SetAchievement(achievementId);
        if (unlocked)
        {
            AchievementUnlocked?.Invoke(achievementId);
        }

        return unlocked;
    }

    public bool IsUnlocked(string achievementId)
    {
        return SaveManager.Instance != null
            && SaveManager.Instance.ActiveSave != null
            && SaveManager.Instance.ActiveSave.HasAchievement(achievementId);
    }

    private bool ContainsAchievement(string achievementId)
    {
        foreach (AchievementDefinition achievement in Achievements)
        {
            if (achievement.id == achievementId)
            {
                return true;
            }
        }

        return false;
    }
}