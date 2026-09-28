using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSelectionController : MonoBehaviour
{
    [SerializeField] private Button[] slotButtons = new Button[SaveManager.SlotCount];
    [SerializeField] private TMP_Text[] slotLabels = new TMP_Text[SaveManager.SlotCount];
    [SerializeField] private string lobbySceneName = "Lobby";

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ProfilesChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ProfilesChanged -= Refresh;
        }
    }

    public void SelectSlot(int slotIndex)
    {
        SaveManager.Instance.SelectSlot(slotIndex);
        SceneManager.LoadScene(lobbySceneName);
    }

    public void ResetSlot(int slotIndex)
    {
        SaveManager.Instance.ResetSlot(slotIndex);
        Refresh();
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

            if (slotIndex < slotButtons.Length && slotButtons[slotIndex] != null)
            {
                int capturedSlotIndex = slotIndex;
                slotButtons[slotIndex].onClick.RemoveAllListeners();
                slotButtons[slotIndex].onClick.AddListener(() => SelectSlot(capturedSlotIndex));
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