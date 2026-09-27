using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoolProgressEntry
{
    public string id;
    public bool unlocked;

    public BoolProgressEntry(string id, bool unlocked)
    {
        this.id = id;
        this.unlocked = unlocked;
    }
}

[Serializable]
public class LevelBestTime
{
    public string levelId;
    public float seconds = -1f;

    public LevelBestTime(string levelId)
    {
        this.levelId = levelId;
    }
}

[Serializable]
public class SaveData
{
    public int slotId;
    public bool created;
    public string createdAtUtc;
    public bool tutorialCompleted;
    public bool map1Unlocked;
    public List<BoolProgressEntry> collectibles = new List<BoolProgressEntry>();
    public List<BoolProgressEntry> achievements = new List<BoolProgressEntry>();
    public List<LevelBestTime> bestTimes = new List<LevelBestTime>();

    public static SaveData CreateEmpty(int slotId)
    {
        return new SaveData
        {
            slotId = slotId,
            created = false,
            createdAtUtc = string.Empty,
            tutorialCompleted = false,
            map1Unlocked = false
        };
    }

    public void MarkCreated()
    {
        created = true;
        if (string.IsNullOrEmpty(createdAtUtc))
        {
            createdAtUtc = DateTime.UtcNow.ToString("O");
        }
    }

    public void EnsureValid(int expectedSlotId)
    {
        slotId = expectedSlotId;
        collectibles ??= new List<BoolProgressEntry>();
        achievements ??= new List<BoolProgressEntry>();
        bestTimes ??= new List<LevelBestTime>();

        if (tutorialCompleted)
        {
            map1Unlocked = true;
        }
    }

    public bool HasCollectible(string id)
    {
        return GetFlag(collectibles, id);
    }

    public bool HasAchievement(string id)
    {
        return GetFlag(achievements, id);
    }

    public void SetCollectible(string id, bool unlocked)
    {
        SetFlag(collectibles, id, unlocked);
    }

    public void SetAchievement(string id, bool unlocked)
    {
        SetFlag(achievements, id, unlocked);
    }

    public float GetBestTime(string levelId)
    {
        LevelBestTime entry = bestTimes.Find(time => time.levelId == levelId);
        return entry == null ? -1f : entry.seconds;
    }

    public bool TrySetBestTime(string levelId, float seconds)
    {
        if (string.IsNullOrEmpty(levelId) || seconds < 0f)
        {
            return false;
        }

        LevelBestTime entry = bestTimes.Find(time => time.levelId == levelId);
        if (entry == null)
        {
            bestTimes.Add(new LevelBestTime(levelId) { seconds = seconds });
            return true;
        }

        if (entry.seconds >= 0f && entry.seconds <= seconds)
        {
            return false;
        }

        entry.seconds = seconds;
        return true;
    }

    private static bool GetFlag(List<BoolProgressEntry> entries, string id)
    {
        BoolProgressEntry entry = entries.Find(item => item.id == id);
        return entry != null && entry.unlocked;
    }

    private static void SetFlag(List<BoolProgressEntry> entries, string id, bool unlocked)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        BoolProgressEntry entry = entries.Find(item => item.id == id);
        if (entry == null)
        {
            entries.Add(new BoolProgressEntry(id, unlocked));
            return;
        }

        entry.unlocked = unlocked;
    }
}