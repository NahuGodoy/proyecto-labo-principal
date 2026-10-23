using TMPro;
using UnityEngine;

public class LevelTimerUI : MonoBehaviour
{
    [SerializeField] private LevelTimer levelTimer;

    private TMP_Text timerText;
    private int lastDisplayedSecond = -1;

    private void Awake()
    {
        timerText = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        if (levelTimer == null)
        {
            levelTimer = FindAnyObjectByType<LevelTimer>();
        }

        UpdateDisplay();
    }

    private void Update()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (levelTimer == null || timerText == null)
        {
            return;
        }

        int totalSeconds = Mathf.FloorToInt(levelTimer.DisplaySeconds);
        if (totalSeconds == lastDisplayedSecond)
        {
            return;
        }

        lastDisplayedSecond = totalSeconds;
        timerText.text = $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
    }
}