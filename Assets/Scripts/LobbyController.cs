using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
    [SerializeField] private string map1SceneName = "EscenarioUno";
    [SerializeField] private string tutorialSceneName = "EscenarioTutorial";
    [SerializeField] private TMP_Text activeSlotLabel;
    [SerializeField] private TMP_Text tutorialTimeLabel;
    [SerializeField] private string selectionSceneName = "SaveSelection";
    [SerializeField] private Transform tutorialPortalTransform;
    [SerializeField] private Transform scenarioOnePortalTransform;
    [SerializeField] private float portalInteractionRadius = 2.5f;
    [SerializeField] private GameObject lobbyPortalUI;

    private enum PortalKind
    {
        Tutorial,
        ScenarioOne
    }

    private PlayerInput playerInput;
    private InputAction interactAction;
    private UnityEngine.Events.UnityAction[] difficultyActions;
    private GameObject promptRoot;
    private TMP_Text promptText;
    private GameObject selectorRoot;
    private TMP_Text selectorTitle;
    private TMP_Text recordText;
    private GameObject difficultyRoot;
    private Button[] difficultyButtons;
    private Button enterButton;
    private Button cancelButton;
    private PortalKind activePortal;
    private bool selectorOpen;
    private float previousTimeScale = 1f;
    private bool previousPauseState;

    private void OnEnable()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ActiveProfileChanged += Refresh;
        }

        Time.timeScale = 1f;
        PauseMenu.JuegoEsPausado = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Refresh();
        if (lobbyPortalUI != null && !lobbyPortalUI.activeSelf)
        {
            lobbyPortalUI.SetActive(true);
        }

        CacheUIReferences();
        BindUIButtons();
        SetPromptVisible(false);
        if (selectorRoot != null)
        {
            selectorRoot.SetActive(false);
        }
    }

    private void OnDisable()
    {
        if (selectorOpen)
        {
            CloseSelector();
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ActiveProfileChanged -= Refresh;
        }

        UnbindUIButtons();
    }

    private void Update()
    {
        if (playerInput == null)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
            if (playerInput != null)
            {
                interactAction = playerInput.actions.FindAction("Player/Interact", false);
            }
        }

        if (selectorOpen)
        {
            if ((Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) ||
                (Gamepad.current != null && Gamepad.current.buttonEast.wasPressedThisFrame))
            {
                CloseSelector();
            }

            return;
        }

        PlayerMovement player = PlayerMovement.Instance;
        if (player == null)
        {
            SetPromptVisible(false);
            return;
        }

        PortalKind? nearbyPortal = GetNearbyPortal(player.transform.position);
        SetPromptVisible(nearbyPortal.HasValue);
        if (!nearbyPortal.HasValue)
        {
            return;
        }

        activePortal = nearbyPortal.Value;
        if (promptText != null)
        {
            promptText.text = activePortal == PortalKind.Tutorial
                ? "Tutorial  |  E para ingresar"
                : "Escenario 1  |  E para elegir dificultad";
        }

        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            OpenSelector(activePortal);
        }
    }

    public void OpenMap1()
    {
        OpenSelector(PortalKind.ScenarioOne);
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

        SetVisibility(map1, true);

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

    private PortalKind? GetNearbyPortal(Vector3 playerPosition)
    {
        float radiusSquared = portalInteractionRadius * portalInteractionRadius;
        float tutorialDistance = tutorialPortalTransform == null
            ? float.PositiveInfinity
            : HorizontalDistanceSquared(playerPosition, tutorialPortalTransform.position);
        float scenarioDistance = scenarioOnePortalTransform == null
            ? float.PositiveInfinity
            : HorizontalDistanceSquared(playerPosition, scenarioOnePortalTransform.position);
        bool tutorialNearby = tutorialDistance <= radiusSquared;
        bool scenarioNearby = scenarioDistance <= radiusSquared;

        if (!tutorialNearby && !scenarioNearby)
        {
            return null;
        }

        return tutorialNearby && (!scenarioNearby || tutorialDistance <= scenarioDistance)
            ? PortalKind.Tutorial
            : PortalKind.ScenarioOne;
    }

    private static float HorizontalDistanceSquared(Vector3 first, Vector3 second)
    {
        first.y = 0f;
        second.y = 0f;
        return (first - second).sqrMagnitude;
    }

    private void OpenSelector(PortalKind portal)
    {
        if (lobbyPortalUI == null || selectorRoot == null || selectorTitle == null || recordText == null)
        {
            Debug.LogError("La instancia LobbyPortalUI o sus referencias Title, Record y LevelSelector no están disponibles.");
            return;
        }

        activePortal = portal;
        lobbyPortalUI.SetActive(true);
        selectorTitle.text = portal == PortalKind.Tutorial ? "Tutorial" : "Escenario 1";
        if (difficultyRoot != null)
        {
            difficultyRoot.SetActive(portal == PortalKind.ScenarioOne);
        }
        RefreshRecordLabel();
        RefreshDifficultyButtons();
        SetPromptVisible(false);
        selectorRoot.SetActive(true);
        if (!selectorRoot.activeInHierarchy)
        {
            Debug.LogError("El panel LevelSelector no está activo en la jerarquía de LobbyPortalUI.");
            return;
        }

        selectorOpen = true;
        previousTimeScale = Time.timeScale;
        previousPauseState = PauseMenu.JuegoEsPausado;
        Time.timeScale = 0f;
        PauseMenu.JuegoEsPausado = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (EventSystem.current != null)
        {
            GameObject selectedControl = portal == PortalKind.ScenarioOne && difficultyButtons != null && difficultyButtons.Length > 1 && difficultyButtons[1] != null
                ? difficultyButtons[1].gameObject
                : enterButton != null ? enterButton.gameObject : null;
            EventSystem.current.SetSelectedGameObject(selectedControl);
        }
    }

    private void CloseSelector()
    {
        selectorOpen = false;
        if (selectorRoot != null)
        {
            selectorRoot.SetActive(false);
        }

        Time.timeScale = previousTimeScale;
        PauseMenu.JuegoEsPausado = previousPauseState;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SelectDifficulty(ScenarioDifficulty difficulty)
    {
        ScenarioDifficultySelection.Select(difficulty);
        RefreshDifficultyButtons();
        RefreshRecordLabel();
    }

    private void RefreshRecordLabel()
    {
        SaveData save = SaveManager.Instance == null ? null : SaveManager.Instance.ActiveSave;
        if (save == null)
        {
                if (recordText != null)
                {
                    recordText.text = "Sin récord";
                }
            return;
        }

        string levelId = activePortal == PortalKind.Tutorial
            ? "tutorial"
            : ScenarioDifficultySelection.LevelId;
        float bestTime = save.GetBestTime(levelId);
        string levelLabel = activePortal == PortalKind.Tutorial
            ? "Récord"
            : $"Récord {GetDifficultyLabel(ScenarioDifficultySelection.Selected)}";
        if (recordText != null)
        {
            recordText.text = bestTime < 0f ? $"{levelLabel}: sin tiempo" : $"{levelLabel}: {bestTime:F2} s";
        }
    }

    private void RefreshDifficultyButtons()
    {
        if (difficultyButtons == null)
        {
            return;
        }

        for (int index = 0; index < difficultyButtons.Length; index++)
        {
            if (difficultyButtons[index] == null)
            {
                continue;
            }

            Image image = difficultyButtons[index].GetComponent<Image>();
            if (image == null)
            {
                continue;
            }

            bool selected = (int)ScenarioDifficultySelection.Selected == index;
            image.color = selected ? new Color(0.2f, 0.68f, 0.68f) : new Color(0.13f, 0.2f, 0.21f);
        }
    }

    private void EnterSelectedLevel()
    {
        string sceneName;
        if (activePortal == PortalKind.Tutorial)
        {
            sceneName = tutorialSceneName;
        }
        else
        {
            sceneName = map1SceneName;
            ScenarioDifficultySelection.Select(ScenarioDifficultySelection.Selected);
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"La escena '{sceneName}' no está disponible en Build Settings.");
            return;
        }

        Time.timeScale = 1f;
        PauseMenu.JuegoEsPausado = false;
        SceneManager.LoadScene(sceneName);
    }

    private void SetPromptVisible(bool visible)
    {
        if (promptRoot != null && promptRoot.activeSelf != visible)
        {
            promptRoot.SetActive(visible);
        }
    }

    private void BindUIButtons()
    {
        if (difficultyButtons != null)
        {
            difficultyActions = new UnityEngine.Events.UnityAction[difficultyButtons.Length];
            for (int index = 0; index < difficultyButtons.Length; index++)
            {
                int selectedIndex = index;
                difficultyActions[index] = () => SelectDifficulty((ScenarioDifficulty)selectedIndex);
                if (difficultyButtons[index] != null)
                {
                    difficultyButtons[index].onClick.AddListener(difficultyActions[index]);
                }
            }
        }

        if (enterButton != null)
        {
            enterButton.onClick.AddListener(EnterSelectedLevel);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(CloseSelector);
        }
    }

    private void CacheUIReferences()
    {
        if (lobbyPortalUI == null)
        {
            Debug.LogError("Asigna la instancia LobbyPortalUI de la escena en LobbyController.");
            return;
        }

        Transform root = lobbyPortalUI.transform;
        Transform prompt = FindChild(root, "PortalPrompt");
        Transform selector = FindChild(root, "LevelSelector");
        Transform difficulty = FindChild(root, "DifficultyOptions");
        Transform promptLabel = FindChild(root, "PromptText");
        Transform titleLabel = FindChild(root, "Title");
        Transform recordLabel = FindChild(root, "Record");

        promptRoot = prompt == null ? null : prompt.gameObject;
        promptText = promptLabel == null ? null : promptLabel.GetComponent<TMP_Text>();
        selectorRoot = selector == null ? null : selector.gameObject;
        selectorTitle = titleLabel == null ? null : titleLabel.GetComponent<TMP_Text>();
        recordText = recordLabel == null ? null : recordLabel.GetComponent<TMP_Text>();
        difficultyRoot = difficulty == null ? null : difficulty.gameObject;
        difficultyButtons = new[]
        {
            FindButton(root, "Fácil"),
            FindButton(root, "Normal"),
            FindButton(root, "Difícil")
        };
        enterButton = FindButton(root, "Ingresar");
        cancelButton = FindButton(root, "Cancelar");
    }

    private static Transform FindChild(Transform root, string childName)
    {
        if (root.name == childName)
        {
            return root;
        }

        for (int index = 0; index < root.childCount; index++)
        {
            Transform match = FindChild(root.GetChild(index), childName);
            if (match != null)
            {
                return match;
            }
        }

        return null;
    }

    private static Button FindButton(Transform root, string label)
    {
        Button[] buttons = root.GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name == label)
            {
                return button;
            }

            TMP_Text[] labels = button.GetComponentsInChildren<TMP_Text>(true);
            foreach (TMP_Text buttonLabel in labels)
            {
                if (buttonLabel.text == label)
                {
                    return button;
                }
            }
        }

        return null;
    }

    private void UnbindUIButtons()
    {
        if (difficultyActions != null && difficultyButtons != null)
        {
            for (int index = 0; index < difficultyActions.Length && index < difficultyButtons.Length; index++)
            {
                if (difficultyButtons[index] != null && difficultyActions[index] != null)
                {
                    difficultyButtons[index].onClick.RemoveListener(difficultyActions[index]);
                }
            }
        }

        if (enterButton != null)
        {
            enterButton.onClick.RemoveListener(EnterSelectedLevel);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveListener(CloseSelector);
        }
    }

    private static string GetDifficultyLabel(ScenarioDifficulty difficulty)
    {
        return difficulty switch
        {
            ScenarioDifficulty.Easy => "Fácil",
            ScenarioDifficulty.Hard => "Difícil",
            _ => "Normal"
        };
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