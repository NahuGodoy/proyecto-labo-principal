using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlsMenuUI : MonoBehaviour
{
    private const string InitialStatusMessage = "Presione sobre el botón para actualizar la tecla";

    [Serializable]
    private class BindingRow
    {
        public RebindableKey key;
        public Button rebindButton;
        public TMP_Text keyLabel;
    }

    [SerializeField] private KeybindingRebindManager rebindManager;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private BindingRow[] bindings;
    [SerializeField] private Button resetAllButton;
    [SerializeField] private TMP_Text statusLabel;

    private Action[] rebindButtonActions;
    private bool initialized;

    private void OnEnable()
    {
        initialized = false;
        TryInitialize();
    }

    private void Update()
    {
        if (!initialized)
        {
            TryInitialize();
        }
    }

    private void TryInitialize()
    {
        if (rebindManager == null)
        {
            rebindManager = GetComponent<KeybindingRebindManager>();
        }

        if (playerInput == null)
        {
            playerInput = FindAnyObjectByType<PlayerInput>();
        }

        if (rebindManager == null || playerInput == null)
        {
            return;
        }

        rebindManager.Initialize(playerInput);
        BindButtons();
        RefreshLabels();
        SetStatus(InitialStatusMessage);
        initialized = true;
    }

    private void OnDisable()
    {
        UnbindButtons();
        if (rebindManager != null && rebindManager.IsRebinding)
        {
            rebindManager.CancelRebinding();
        }

        initialized = false;
    }

    public void RefreshLabels()
    {
        if (rebindManager == null || bindings == null)
        {
            return;
        }

        foreach (BindingRow row in bindings)
        {
            if (row != null && row.keyLabel != null)
            {
                row.keyLabel.text = rebindManager.GetBindingDisplayName(row.key);
            }
        }
    }

    public void ResetAllBindings()
    {
        if (rebindManager == null)
        {
            return;
        }

        rebindManager.ResetAllBindings();
        RefreshLabels();
        SetStatus("Teclas restauradas");
    }

    public bool CancelRebindingIfActive()
    {
        if (rebindManager == null)
        {
            return false;
        }

        if (rebindManager.ConsumeRebindingCancellationThisFrame())
        {
            return true;
        }

        if (!rebindManager.IsRebinding)
        {
            return false;
        }

        rebindManager.CancelRebinding();
        return true;
    }

    private void BindButtons()
    {
        if (bindings != null)
        {
            rebindButtonActions = new Action[bindings.Length];
            for (int index = 0; index < bindings.Length; index++)
            {
                BindingRow row = bindings[index];
                if (row == null || row.rebindButton == null)
                {
                    continue;
                }

                RebindableKey capturedKey = row.key;
                rebindButtonActions[index] = () => BeginRebinding(capturedKey);
                row.rebindButton.onClick.AddListener(rebindButtonActions[index].Invoke);
            }
        }

        if (resetAllButton != null)
        {
            resetAllButton.onClick.AddListener(ResetAllBindings);
        }
    }

    private void UnbindButtons()
    {
        if (rebindButtonActions != null && bindings != null)
        {
            for (int index = 0; index < rebindButtonActions.Length && index < bindings.Length; index++)
            {
                if (rebindButtonActions[index] != null && bindings[index] != null && bindings[index].rebindButton != null)
                {
                    bindings[index].rebindButton.onClick.RemoveListener(rebindButtonActions[index].Invoke);
                }
            }
        }

        rebindButtonActions = null;
        if (resetAllButton != null)
        {
            resetAllButton.onClick.RemoveListener(ResetAllBindings);
        }
    }

    private void BeginRebinding(RebindableKey key)
    {
        SetStatus("Presiona una tecla o botón del mouse... (Escape para cancelar)");
        bool started = rebindManager.StartRebinding(key, succeeded =>
        {
            RefreshLabels();
            SetStatus(succeeded ? "Tecla actualizada" : InitialStatusMessage);
        });

        if (!started)
        {
            SetStatus("No se pudo iniciar la reasignacion");
        }
    }

    private void SetStatus(string message)
    {
        if (statusLabel != null)
        {
            statusLabel.text = message;
        }
    }
}