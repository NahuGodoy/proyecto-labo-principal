using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class LobbyUnlockable
{
    public string unlockId;
    public GameObject lockedView;
    public GameObject unlockedView;
    public Button levelButton;
}

public class LobbyController : MonoBehaviour
{
    [SerializeField] private LobbyUnlockable map1;
    [SerializeField] private string map1SceneName = "Map1";
    [SerializeField] private TMP_Text activeSlotLabel;
    [SerializeField] private TMP_Text tutorialTimeLabel;
    [SerializeField] private string selectionSceneName = "SaveSelection";

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ActiveProfileChanged += Refresh;
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Refresh();
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ActiveProfileChanged -= Refresh;
        }
    }

    public void OpenMap1()
    {
        if (SaveManager.Instance == null || SaveManager.Instance.ActiveSave == null ||
            !SaveManager.Instance.ActiveSave.map1Unlocked)
        {
            return;
        }

        SceneManager.LoadScene(map1SceneName);
    }

    public void ReturnToSaveSelection()
    {
        SceneManager.LoadScene(selectionSceneName);
    }

    public void Refresh()
    {
        SaveData save = SaveManager.Instance == null ? null : SaveManager.Instance.ActiveSave;
        if (save == null)
        {
            return;
        }

        bool map1Unlocked = save.map1Unlocked;
        SetVisibility(map1, map1Unlocked);

        if (activeSlotLabel != null)
        {
            activeSlotLabel.text = $"Partida {save.slotId + 1}";
        }

        if (tutorialTimeLabel != null)
        {
            float tutorialTime = save.GetBestTime("tutorial");
            tutorialTimeLabel.text = tutorialTime < 0f
                ? "Tutorial: sin tiempo"
                : $"Tutorial: {tutorialTime:F2} s";
        }
    }

    private static void SetVisibility(LobbyUnlockable unlockable, bool isUnlocked)
    {
        if (unlockable == null)
        {
            return;
        }

        if (unlockable.lockedView != null)
        {
            unlockable.lockedView.SetActive(!isUnlocked);
        }

        if (unlockable.unlockedView != null)
        {
            unlockable.unlockedView.SetActive(isUnlocked);
        }

        if (unlockable.levelButton != null)
        {
            unlockable.levelButton.interactable = isUnlocked;
        }
    }
}