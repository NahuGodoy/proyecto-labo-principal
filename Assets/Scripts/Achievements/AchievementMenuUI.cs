using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementMenuUI : MonoBehaviour
{
    private static readonly Color UnlockedRowColor = ParseHexColor("#266b53");
    private static readonly Color LockedRowColor = ParseHexColor("#303030");
    private static readonly Color UnlockedIconColor = ParseHexColor("#fffefd");
    private static readonly Color LockedIconColor = ParseHexColor("#000000");

    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private Image[] icons;
    [SerializeField] private TMP_Text[] iconFallbacks;
    [SerializeField] private TMP_Text[] titles;
    [SerializeField] private TMP_Text[] descriptions;
    [SerializeField] private Image[] rowBackgrounds;
    [SerializeField] private GameObject notificationPanel;
    [SerializeField] private TMP_Text notificationTitle;
    [SerializeField] private TMP_Text notificationAchievementName;
    [SerializeField] private Image notificationIcon;
    [SerializeField] private float notificationDuration = 3.5f;

    private AchievementSystem achievementSystem;
    private Coroutine notificationRoutine;

    public bool IsOpen => panel != null && panel.activeSelf;

    public void Initialize(AchievementSystem system, PauseMenu pauseMenu)
    {
        if (achievementSystem != null)
        {
            achievementSystem.AchievementUnlocked -= ShowAchievementUnlocked;
        }

        achievementSystem = system;
        if (achievementSystem != null)
        {
            achievementSystem.AchievementUnlocked += ShowAchievementUnlocked;
        }

        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }

        if (openButton != null)
        {
            openButton.onClick.RemoveListener(pauseMenu.AbrirLogros);
            openButton.onClick.AddListener(pauseMenu.AbrirLogros);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
            closeButton.onClick.AddListener(Close);
        }

        Refresh();
    }

    private void OnDestroy()
    {
        if (achievementSystem != null)
        {
            achievementSystem.AchievementUnlocked -= ShowAchievementUnlocked;
        }
    }

    public void Open()
    {
        Refresh();
        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
        }
    }

    public void Close()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Refresh()
    {
        if (achievementSystem == null)
        {
            return;
        }

        var achievements = achievementSystem.Achievements;
        int rowCount = GetAvailableRowCount();
        int unlockedCount = 0;
        for (int index = 0; index < achievements.Count; index++)
        {
            AchievementDefinition achievement = achievements[index];
            bool unlocked = achievementSystem.IsUnlocked(achievement.id);
            if (unlocked)
            {
                unlockedCount++;
            }

            if (index >= rowCount)
            {
                continue;
            }

            if (rowBackgrounds[index] != null)
            {
                rowBackgrounds[index].color = unlocked
                    ? UnlockedRowColor
                    : LockedRowColor;
            }

            if (icons[index] != null)
            {
                icons[index].sprite = achievement.icon;
                icons[index].color = unlocked
                    ? UnlockedIconColor
                    : LockedIconColor;
            }

            if (iconFallbacks[index] != null)
            {
                iconFallbacks[index].text = achievement.icon == null
                    ? (unlocked ? "OK" : (index + 1).ToString("00"))
                    : string.Empty;
            }

            if (titles[index] != null)
            {
                titles[index].text = achievement.displayName;
            }

            if (descriptions[index] != null)
            {
                descriptions[index].text = unlocked
                    ? (string.IsNullOrWhiteSpace(achievement.unlockedDescription)
                        ? "Conseguido"
                        : achievement.unlockedDescription)
                    : achievement.description;
            }
        }

        if (progressText != null)
        {
            progressText.text = unlockedCount + " / " + achievements.Count + " conseguidos";
        }

        if (rowCount < achievements.Count)
        {
            Debug.LogWarning($"AchievementMenuUI can display {rowCount} of {achievements.Count} achievements. " +
                "Add rows to the panel hierarchy and assign all row arrays in the Inspector.", this);
        }
    }

    private void ShowAchievementUnlocked(string achievementId)
    {
        if (notificationPanel == null || achievementSystem == null)
        {
            return;
        }

        AchievementDefinition achievement = null;
        foreach (AchievementDefinition definition in achievementSystem.Achievements)
        {
            if (definition.id == achievementId)
            {
                achievement = definition;
                break;
            }
        }

        if (achievement == null)
        {
            return;
        }

        if (notificationTitle != null)
        {
            notificationTitle.text = "Logro conseguido";
        }

        if (notificationAchievementName != null)
        {
            notificationAchievementName.text = achievement.displayName;
        }

        if (notificationIcon != null)
        {
            notificationIcon.sprite = achievement.icon;
            notificationIcon.enabled = achievement.icon != null;
        }

        notificationPanel.SetActive(true);
        if (notificationRoutine != null)
        {
            StopCoroutine(notificationRoutine);
        }

        notificationRoutine = StartCoroutine(HideNotificationAfterDelay());
    }

    private IEnumerator HideNotificationAfterDelay()
    {
        yield return new WaitForSecondsRealtime(notificationDuration);
        notificationPanel.SetActive(false);
        notificationRoutine = null;
    }

    private int GetAvailableRowCount()
    {
        if (icons == null || iconFallbacks == null || titles == null || descriptions == null || rowBackgrounds == null)
        {
            return 0;
        }

        return Mathf.Min(icons.Length, iconFallbacks.Length, titles.Length, descriptions.Length, rowBackgrounds.Length);
    }

    private static Color ParseHexColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            return color;
        }

        Debug.LogError($"Invalid achievement UI color: {hex}");
        return Color.white;
    }
}