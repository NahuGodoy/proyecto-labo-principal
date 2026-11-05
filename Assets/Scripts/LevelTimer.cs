using UnityEngine;
using UnityEngine.Events;

public enum ScenarioDifficulty
{
    Easy,
    Normal,
    Hard
}

public static class ScenarioDifficultySelection
{
    public static ScenarioDifficulty Selected { get; private set; } = ScenarioDifficulty.Normal;

    public static string LevelId => Selected switch
    {
        ScenarioDifficulty.Easy => "escenario1:facil",
        ScenarioDifficulty.Hard => "escenario1:dificil",
        _ => "escenario1:normal"
    };

    public static void Select(ScenarioDifficulty difficulty)
    {
        Selected = difficulty;
    }
}

public static class ScenarioModeSelection
{
    public static bool TimeTrialEnabled { get; private set; } = true;
    public static bool RandomEventsEnabled { get; private set; } = true;

    public static void Select(bool timeTrialEnabled, bool randomEventsEnabled)
    {
        TimeTrialEnabled = timeTrialEnabled;
        RandomEventsEnabled = randomEventsEnabled;
    }
}

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private string levelId = "tutorial";
    [SerializeField] private bool startOnEnable = true;
    [SerializeField] private UnityEvent<float> completed;
    [Header("Cuenta regresiva (opcional)")]
    [SerializeField] private bool countdown = false;
    [SerializeField] private float countdownSeconds = 30f;
    [SerializeField] private UnityEvent timeUp;

    public UnityEvent TimeUp => timeUp;

    // Lo que se muestra en pantalla: tiempo restante si es regresivo, o transcurrido si no
    public float DisplaySeconds => countdown
        ? Mathf.Max(0f, countdownSeconds - elapsedSeconds)
        : elapsedSeconds;

    private float elapsedSeconds;
    private bool running;
    private bool finished;

    private void Awake()
    {
        if (levelId != "escenario1" && levelId != "escenario2")
        {
            return;
        }

        countdown = ScenarioModeSelection.TimeTrialEnabled;

        EventosAleatoriosManager eventManager = FindAnyObjectByType<EventosAleatoriosManager>(FindObjectsInactive.Include);
        if (eventManager != null)
        {
            eventManager.gameObject.SetActive(ScenarioModeSelection.RandomEventsEnabled);
        }
    }

    public float ElapsedSeconds => elapsedSeconds;
    public bool IsRunning => running;

    private void OnEnable()
    {
        if (startOnEnable)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (!running) return;

        elapsedSeconds += Time.deltaTime;

        if (countdown && elapsedSeconds >= countdownSeconds)
        {
            elapsedSeconds = countdownSeconds;
            running = false;
            timeUp?.Invoke(); 
        }
    }

    public void StartTimer()
    {
        elapsedSeconds = 0f;
        finished = false;
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }

    public void CompleteLevel()
    {
        CompleteLevel(true);
    }

    public void CompleteLevelWithoutRecordingTime()
    {
        CompleteLevel(false);
    }

    private void CompleteLevel(bool recordTime)
    {
        if (finished)
        {
            return;
        }

        finished = true;
        running = false;

        string completedLevelId = levelId == "escenario1"
            ? ScenarioDifficultySelection.LevelId
            : levelId;

        if (SaveManager.Instance != null && SaveManager.Instance.ActiveSave != null)
        {
            if (completedLevelId == "tutorial")
            {
                SaveManager.Instance.CompleteTutorial(recordTime ? elapsedSeconds : -1f);
                if (recordTime)
                {
                    AchievementSystem.Instance?.Unlock(AchievementSystem.TutorialCompletedId);
                }
            }
            else if (recordTime)
            {
                SaveManager.Instance.TrySetBestTime(completedLevelId, elapsedSeconds);
            }
        }

        completed?.Invoke(elapsedSeconds);
    }
}