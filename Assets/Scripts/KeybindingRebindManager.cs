using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum RebindableKey
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Jump,
    Interact,
    Attack
}

public class KeybindingRebindManager : MonoBehaviour
{
    private const string PlayerMapName = "Player";
    private const string PreferenceKeyPrefix = "KeybindingOverrides.";

    private InputActionAsset actions;
    private InputActionRebindingExtensions.RebindingOperation activeRebinding;
    private int rebindCancelFrame = -1;

    public bool IsRebinding => activeRebinding != null;

    public void Initialize(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            Debug.LogError("KeybindingRebindManager requires a PlayerInput.", this);
            return;
        }

        if (actions == playerInput.actions)
        {
            return;
        }

        CancelRebinding();
        actions = playerInput.actions;
        LoadOverrides();
    }

    public string GetBindingDisplayName(RebindableKey key)
    {
        if (!TryGetBinding(key, out InputAction action, out int bindingIndex))
        {
            return string.Empty;
        }

        InputBinding binding = action.bindings[bindingIndex];
        if (binding.effectivePath.EndsWith("/space", StringComparison.OrdinalIgnoreCase))
        {
            return "Espacio";
        }

        if (key == RebindableKey.Attack &&
            binding.effectivePath.EndsWith("/leftButton", StringComparison.OrdinalIgnoreCase))
        {
            return "LMB";
        }

        return action.GetBindingDisplayString(bindingIndex);
    }

    public bool StartRebinding(RebindableKey key, Action<bool> onFinished = null)
    {
        CancelRebinding();

        if (!TryGetBinding(key, out InputAction action, out int bindingIndex))
        {
            Debug.LogError($"No keyboard binding found for {key}.", this);
            return false;
        }

        string controlPath = key == RebindableKey.Attack ? "<Mouse>" : "<Keyboard>";
        action.Disable();
        activeRebinding = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsHavingToMatchPath(controlPath)
            .WithControlsExcluding("<Keyboard>/escape")
            .OnComplete(operation => FinishRebinding(operation, action, onFinished, true))
            .OnCancel(operation => FinishRebinding(operation, action, onFinished, false))
            .Start();

        return true;
    }

    public void CancelRebinding()
    {
        activeRebinding?.Cancel();
    }

    public bool ConsumeRebindingCancellationThisFrame()
    {
        if (rebindCancelFrame != Time.frameCount)
        {
            return false;
        }

        rebindCancelFrame = -1;
        return true;
    }

    public void ResetBinding(RebindableKey key)
    {
        if (!TryGetBinding(key, out InputAction action, out int bindingIndex))
        {
            return;
        }

        action.RemoveBindingOverride(bindingIndex);
        SaveOverrides();
    }

    public void ResetAllBindings()
    {
        if (actions == null)
        {
            return;
        }

        CancelRebinding();
        foreach (RebindableKey key in Enum.GetValues(typeof(RebindableKey)))
        {
            if (TryGetBinding(key, out InputAction action, out int bindingIndex))
            {
                action.RemoveBindingOverride(bindingIndex);
            }
        }

        SaveOverrides();
    }

    private void OnDisable()
    {
        CancelRebinding();
    }

    private bool TryGetBinding(RebindableKey key, out InputAction action, out int bindingIndex)
    {
        action = null;
        bindingIndex = -1;

        if (actions == null)
        {
            Debug.LogError("Call Initialize with a PlayerInput before using keybinding methods.", this);
            return false;
        }

        InputActionMap playerMap = actions.FindActionMap(PlayerMapName, false);
        if (playerMap == null)
        {
            Debug.LogError($"Input action map '{PlayerMapName}' was not found.", this);
            return false;
        }

        string actionName = key switch
        {
            RebindableKey.MoveUp or RebindableKey.MoveDown or RebindableKey.MoveLeft or RebindableKey.MoveRight => "Move",
            RebindableKey.Jump => "Jump",
            RebindableKey.Interact => "Interact",
            RebindableKey.Attack => "Attack",
            _ => null
        };

        action = playerMap.FindAction(actionName, false);
        if (action == null)
        {
            Debug.LogError($"Input action '{actionName}' was not found.", this);
            return false;
        }

        string compositePart = key switch
        {
            RebindableKey.MoveUp => "up",
            RebindableKey.MoveDown => "down",
            RebindableKey.MoveLeft => "left",
            RebindableKey.MoveRight => "right",
            _ => null
        };

        for (int index = 0; index < action.bindings.Count; index++)
        {
            InputBinding binding = action.bindings[index];
            string devicePath = key == RebindableKey.Attack ? "<Mouse>/" : "<Keyboard>/";
            bool isTargetDeviceBinding = binding.path.StartsWith(devicePath, StringComparison.OrdinalIgnoreCase);
            bool matches = compositePart == null
                ? !binding.isComposite && !binding.isPartOfComposite && isTargetDeviceBinding
                : binding.isPartOfComposite && isTargetDeviceBinding &&
                  string.Equals(binding.name, compositePart, StringComparison.OrdinalIgnoreCase);

            if (matches)
            {
                bindingIndex = index;
                return true;
            }
        }

        return false;
    }

    private void FinishRebinding(
        InputActionRebindingExtensions.RebindingOperation operation,
        InputAction action,
        Action<bool> onFinished,
        bool succeeded)
    {
        if (activeRebinding == operation)
        {
            activeRebinding = null;
        }

        if (!succeeded)
        {
            rebindCancelFrame = Time.frameCount;
        }

        if (succeeded)
        {
            SaveOverrides();
        }

        operation.Dispose();
        action.Enable();
        onFinished?.Invoke(succeeded);
    }

    private void LoadOverrides()
    {
        string preferenceKey = GetPreferenceKey();
        if (PlayerPrefs.HasKey(preferenceKey))
        {
            actions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(preferenceKey));
        }
    }

    private void SaveOverrides()
    {
        if (actions == null)
        {
            return;
        }

        PlayerPrefs.SetString(GetPreferenceKey(), actions.SaveBindingOverridesAsJson());
        PlayerPrefs.Save();
    }

    private string GetPreferenceKey()
    {
        return PreferenceKeyPrefix + actions.name;
    }
}