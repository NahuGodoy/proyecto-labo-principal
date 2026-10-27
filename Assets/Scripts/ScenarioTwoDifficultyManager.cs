using System;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ScenarioTwoDifficultyManager : MonoBehaviour
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
