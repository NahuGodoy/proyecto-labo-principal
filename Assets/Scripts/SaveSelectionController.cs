using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SaveSelectionController : MonoBehaviour
{
    [SerializeField] private Button[] slotButtons = new Button[SaveManager.SlotCount];
    [SerializeField] private TMP_Text[] slotLabels = new TMP_Text[SaveManager.SlotCount];
    [SerializeField] private Button[] deleteButtons = new Button[SaveManager.SlotCount];
    [SerializeField] private GameObject deleteConfirmationPanel;
    [SerializeField] private TMP_Text deleteConfirmationLabel;
    [SerializeField] private Button confirmDeleteButton;
    [SerializeField] private Button cancelDeleteButton;
    [SerializeField] private string tutorialSceneName = "EscenarioTutorial";
    [SerializeField] private string lobbySceneName = "LobbyTest";

    private UnityAction[] selectActions;
    private UnityAction[] deleteActions;
    private int pendingDeleteSlot = -1;

    private void OnEnable()
    {
        BindButtonListeners();
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ProfilesChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        UnbindButtonListeners();
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ProfilesChanged -= Refresh;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        SaveManager.Instance.SelectSlot(slotIndex);
        string destination = SaveManager.Instance.ActiveSave.tutorialCompleted
            ? lobbySceneName
            : tutorialSceneName;
        SceneManager.LoadScene(destination);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ResetSlot(int slotIndex)
    {
        pendingDeleteSlot = slotIndex;
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(true);
        }

        if (deleteConfirmationLabel != null)
        {
            deleteConfirmationLabel.text = $"Borrar la partida {slotIndex + 1}?";
        }
    }

    public void ConfirmResetSlot()
    {
        if (pendingDeleteSlot < 0 || SaveManager.Instance == null)
        {
            return;
        }

        SaveManager.Instance.ResetSlot(pendingDeleteSlot);
        pendingDeleteSlot = -1;
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }

        Refresh();
    }

    public void CancelResetSlot()
    {
        pendingDeleteSlot = -1;
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }

    private void BindButtonListeners()
    {
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }

        selectActions = new UnityAction[SaveManager.SlotCount];
        deleteActions = new UnityAction[SaveManager.SlotCount];
        for (int slotIndex = 0; slotIndex < SaveManager.SlotCount; slotIndex++)
        {
            int capturedSlotIndex = slotIndex;
            selectActions[slotIndex] = () => SelectSlot(capturedSlotIndex);
            deleteActions[slotIndex] = () => ResetSlot(capturedSlotIndex);

            if (slotIndex < slotButtons.Length && slotButtons[slotIndex] != null)
            {
                slotButtons[slotIndex].onClick.AddListener(selectActions[slotIndex]);
            }

            if (slotIndex < deleteButtons.Length && deleteButtons[slotIndex] != null)
            {
                deleteButtons[slotIndex].onClick.AddListener(deleteActions[slotIndex]);
            }
        }

        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.AddListener(ConfirmResetSlot);
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.AddListener(CancelResetSlot);
        }
    }

    private void UnbindButtonListeners()
    {
        if (selectActions != null)
        {
            for (int slotIndex = 0; slotIndex < selectActions.Length; slotIndex++)
            {
                if (slotIndex < slotButtons.Length && slotButtons[slotIndex] != null)
                {
                    slotButtons[slotIndex].onClick.RemoveListener(selectActions[slotIndex]);
                }

                if (slotIndex < deleteButtons.Length && deleteButtons[slotIndex] != null)
                {
                    deleteButtons[slotIndex].onClick.RemoveListener(deleteActions[slotIndex]);
                }
            }
        }

        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.RemoveListener(ConfirmResetSlot);
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.RemoveListener(CancelResetSlot);
        }
    }

    public void Refresh()
    {
        if (SaveManager.Instance == null)
        {
            return;
        }

        for (int slotIndex = 0; slotIndex < SaveManager.SlotCount; slotIndex++)
        {
            SaveData save = SaveManager.Instance.GetSave(slotIndex);
            if (slotIndex < slotLabels.Length && slotLabels[slotIndex] != null)
            {
                slotLabels[slotIndex].text = BuildSlotLabel(slotIndex, save);
            }
        }
    }

    private static string BuildSlotLabel(int slotIndex, SaveData save)
    {
        if (!save.created)
        {
            return $"Partida {slotIndex + 1}\nNueva partida";
        }

        string tutorialState = save.tutorialCompleted ? "Tutorial completado" : "Tutorial pendiente";
        string mapState = save.map1Unlocked ? "Mapa 1 desbloqueado" : "Mapa 1 bloqueado";
        return $"Partida {slotIndex + 1}\n{tutorialState}\n{mapState}";
    }
}