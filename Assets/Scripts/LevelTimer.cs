using UnityEngine;
using UnityEngine.Events;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private string levelId = "tutorial";
    [SerializeField] private bool startOnEnable = true;
    [SerializeField] private UnityEvent<float> completed;

    private float elapsedSeconds;
    private bool running;
    private bool finished;

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
        if (running)
        {
            elapsedSeconds += Time.deltaTime;
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
        if (finished)
        {
            return;
        }

        finished = true;
        running = false;

        if (SaveManager.Instance != null && SaveManager.Instance.ActiveSave != null)
        {
            if (levelId == "tutorial")
            {
                SaveManager.Instance.CompleteTutorial(elapsedSeconds);
            }
            else
            {
                SaveManager.Instance.TrySetBestTime(levelId, elapsedSeconds);
            }
        }

        completed?.Invoke(elapsedSeconds);
    }
}