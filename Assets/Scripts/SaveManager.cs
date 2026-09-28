using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public const int SlotCount = 3;
    public const float NoBestTime = -1f;

    public static SaveManager Instance { get; private set; }

    [SerializeField] private string saveFilePrefix = "save_slot_";

    private readonly SaveData[] saves = new SaveData[SlotCount];
    private int activeSlotIndex = -1;

    public int ActiveSlotIndex => activeSlotIndex;
    public SaveData ActiveSave => activeSlotIndex < 0 ? null : saves[activeSlotIndex];

    public event Action ProfilesChanged;
    public event Action ActiveProfileChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstanceBeforeSceneLoad()
    {
        if (Instance == null)
        {
            GameObject managerObject = new GameObject(nameof(SaveManager));
            managerObject.AddComponent<SaveManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAll();
    }

    public SaveData GetSave(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);
        return saves[slotIndex];
    }

    public void SelectSlot(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);

        SaveData save = saves[slotIndex];
        if (!save.created)
        {
            save.MarkCreated();
            SaveSlot(slotIndex);
        }

        activeSlotIndex = slotIndex;
        ActiveProfileChanged?.Invoke();
    }

    public void ResetSlot(int slotIndex)
    {
        ValidateSlotIndex(slotIndex);

        saves[slotIndex] = SaveData.CreateEmpty(slotIndex);
        SaveSlot(slotIndex);

        if (activeSlotIndex == slotIndex)
        {
            activeSlotIndex = -1;
            ActiveProfileChanged?.Invoke();
        }
    }

    public void CompleteTutorial(float elapsedSeconds)
    {
        RequireActiveSave();

        ActiveSave.MarkCreated();
        ActiveSave.tutorialCompleted = true;
        ActiveSave.SetMapUnlocked("map1", true);
        ActiveSave.TrySetBestTime("tutorial", elapsedSeconds);
        SaveActiveSlot();
    }

    public void SetMapUnlocked(string mapId, bool unlocked = true)
    {
        RequireActiveSave();
        ActiveSave.SetMapUnlocked(mapId, unlocked);
        SaveActiveSlot();
    }

    public bool SetCollectible(string collectibleId, bool unlocked = true)
    {
        RequireActiveSave();
        bool previousValue = ActiveSave.HasCollectible(collectibleId);
        ActiveSave.SetCollectible(collectibleId, unlocked);
        SaveActiveSlot();
        return previousValue != unlocked;
    }

    public bool SetAchievement(string achievementId, bool unlocked = true)
    {
        RequireActiveSave();
        bool previousValue = ActiveSave.HasAchievement(achievementId);
        ActiveSave.SetAchievement(achievementId, unlocked);
        SaveActiveSlot();
        return previousValue != unlocked;
    }

    public bool TrySetBestTime(string levelId, float elapsedSeconds)
    {
        RequireActiveSave();
        bool improved = ActiveSave.TrySetBestTime(levelId, elapsedSeconds);
        if (improved)
        {
            SaveActiveSlot();
        }

        return improved;
    }

    public void SaveActiveSlot()
    {
        RequireActiveSave();
        SaveSlot(activeSlotIndex);
    }

    private void LoadAll()
    {
        for (int slotIndex = 0; slotIndex < SlotCount; slotIndex++)
        {
            saves[slotIndex] = LoadSlot(slotIndex);
        }

        ProfilesChanged?.Invoke();
    }

    private SaveData LoadSlot(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);
        if (!File.Exists(path))
        {
            SaveData emptySave = SaveData.CreateEmpty(slotIndex);
            SaveSlot(slotIndex, emptySave);
            return emptySave;
        }

        try
        {
            SaveData loadedSave = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            if (loadedSave == null)
            {
                throw new InvalidDataException("Save file is empty.");
            }

            loadedSave.EnsureValid(slotIndex);
            return loadedSave;
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Could not load slot {slotIndex + 1}: {exception.Message}");
            SaveData recoveredSave = SaveData.CreateEmpty(slotIndex);
            SaveSlot(slotIndex, recoveredSave);
            return recoveredSave;
        }
    }

    private void SaveSlot(int slotIndex)
    {
        SaveSlot(slotIndex, saves[slotIndex]);
        ProfilesChanged?.Invoke();
    }

    private void SaveSlot(int slotIndex, SaveData save)
    {
        string path = GetSlotPath(slotIndex);
        string temporaryPath = path + ".tmp";
        string json = JsonUtility.ToJson(save, true);

        File.WriteAllText(temporaryPath, json);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        File.Move(temporaryPath, path);
    }

    private string GetSlotPath(int slotIndex)
    {
        return Path.Combine(Application.persistentDataPath, saveFilePrefix + (slotIndex + 1) + ".json");
    }

    private void RequireActiveSave()
    {
        if (ActiveSave == null)
        {
            throw new InvalidOperationException("There is no active save slot.");
        }
    }

    private static void ValidateSlotIndex(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= SlotCount)
        {
            throw new ArgumentOutOfRangeException(nameof(slotIndex));
        }
    }
}