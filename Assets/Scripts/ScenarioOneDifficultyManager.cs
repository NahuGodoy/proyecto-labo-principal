using System;
using UnityEngine;

[Serializable]
public class CheckpointDifficultyRule
{
    public Checkpoint checkpoint;
    public bool enabledEasy = true;
    public bool enabledNormal = true;
    public bool enabledHard = true;

    public bool IsEnabledFor(ScenarioDifficulty difficulty)
    {
        return difficulty switch
        {
            ScenarioDifficulty.Easy => enabledEasy,
            ScenarioDifficulty.Hard => enabledHard,
            _ => enabledNormal
        };
    }
}

[DefaultExecutionOrder(-100)]
public class ScenarioOneDifficultyManager : MonoBehaviour
{
    [SerializeField] private CheckpointDifficultyRule[] checkpointRules = Array.Empty<CheckpointDifficultyRule>();

    public ScenarioDifficulty AppliedDifficulty { get; private set; }

    private void Awake()
    {
        ApplyDifficulty(ScenarioDifficultySelection.Selected);
    }

    public void ApplyDifficulty(ScenarioDifficulty difficulty)
    {
        AppliedDifficulty = difficulty;

        if (checkpointRules == null)
        {
            return;
        }

        foreach (CheckpointDifficultyRule rule in checkpointRules)
        {
            if (rule == null || rule.checkpoint == null)
            {
                continue;
            }

            rule.checkpoint.gameObject.SetActive(rule.IsEnabledFor(difficulty));
        }
    }
}