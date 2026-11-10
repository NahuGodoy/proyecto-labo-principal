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
    public static int Multiplier => Selected switch
    {
        ScenarioDifficulty.Easy => 1,
        ScenarioDifficulty.Hard => 3,
        _ => 2
    };

    public static string DifficultyLabel => Selected switch
    {
        ScenarioDifficulty.Easy => "Fácil",
        ScenarioDifficulty.Hard => "Difícil",
        _ => "Normal"
    };

    public static string LevelId => GetLevelId("escenario1");

    public static string GetLevelId(string scenarioId)
    {
        string difficultyId = Selected switch
        {
            ScenarioDifficulty.Easy => "facil",
            ScenarioDifficulty.Hard => "dificil",
            _ => "normal"
        };

        return $"{scenarioId}:{difficultyId}";
    }

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
    public bool IsScenarioLevel => levelId == "escenario1" || levelId == "escenario2";
    public string CurrentLevelId => IsScenarioLevel
        ? ScenarioDifficultySelection.GetLevelId(levelId)
        : levelId;
    public bool LastCompletionWasNewRecord { get; private set; }

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
        LastCompletionWasNewRecord = false;

        string completedLevelId = CurrentLevelId;

        if (SaveManager.Instance != null && SaveManager.Instance.ActiveSave != null)
        {
            if (completedLevelId == "tutorial")
            {
                float previousBestTime = SaveManager.Instance.ActiveSave.GetBestTime(completedLevelId);
                SaveManager.Instance.CompleteTutorial(recordTime ? elapsedSeconds : -1f);
                LastCompletionWasNewRecord = recordTime && (previousBestTime < 0f || elapsedSeconds < previousBestTime);
                if (recordTime)
                {
                    AchievementSystem.Instance?.Unlock(AchievementSystem.TutorialCompletedId);
                }
            }
            else if (recordTime)
            {
                LastCompletionWasNewRecord = SaveManager.Instance.TrySetBestTime(completedLevelId, elapsedSeconds);
                if(completedLevelId == "escenario2:facil" ||
                completedLevelId == "escenario2:normal" ||
                completedLevelId == "escenario2:dificil")
                {
                    AchievementSystem.Instance?.Unlock(AchievementSystem.Escenario2CompletedId);
                }
                
            }
        }

        completed?.Invoke(elapsedSeconds);
    }
}