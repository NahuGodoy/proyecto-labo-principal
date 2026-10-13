using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class SplitScreenCamera : MonoBehaviour
{
    Camera cam;
    int index;
    int totalPlayers;
    bool initialized;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined += HandlePlayerJoined;
        }
    }

    private void OnDisable()
    {
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.onPlayerJoined -= HandlePlayerJoined;
        }
    }

    private void Start()
    {
        PlayerInput playerInput = GetComponentInParent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("SplitScreenCamera must be on a Camera under a PlayerInput.", this);
            enabled = false;
            return;
        }

        index = playerInput.playerIndex;
        totalPlayers = PlayerInput.all.Count;
        cam.depth = index;
        SetupCinemachineChannels(playerInput);
        initialized = true;
        SetupCamera();
    }

    private void SetupCinemachineChannels(PlayerInput playerInput)
    {
        if (index < 0 || index >= 16)
        {
            Debug.LogError("Cinemachine supports split-screen channels for player indices 0 through 15.", this);
            return;
        }

        OutputChannels playerChannel = (OutputChannels)(1 << index);
        CinemachineBrain brain = playerInput.GetComponentInChildren<CinemachineBrain>(true);
        CinemachineCamera[] playerCameras = playerInput.GetComponentsInChildren<CinemachineCamera>(true);

        if (brain == null || playerCameras.Length == 0)
        {
            Debug.LogWarning("Player prefab needs a CinemachineBrain and at least one CinemachineCamera for isolated split-screen.", this);
            return;
        }

        brain.ChannelMask = playerChannel;
        foreach (CinemachineCamera playerCamera in playerCameras)
        {
            playerCamera.OutputChannel = playerChannel;
        }
    }

    private void HandlePlayerJoined(PlayerInput obj)
    {
        totalPlayers = PlayerInput.all.Count;
        if (initialized)
        {
            SetupCamera();
        }
    }

    private void SetupCamera()
    {
        if (totalPlayers == 1)
        {
            cam.rect = new Rect(0, 0, 1, 1);
        }
        else if (totalPlayers == 2)
        {
            cam.rect = new Rect(index == 0 ? 0 : 0.5f, 0, 0.5f, 1);
        }
        else if (totalPlayers == 3)
        {
            cam.rect = new Rect(
                index == 0 ? 0 : (index == 1 ? 0.5f : 0),
                index < 2 ? 0.5f : 0,
                index < 2 ? 0.5f : 1,
                0.5f
                );
        }
        else
        {
            cam.rect = new Rect((index % 2) * 0.5f, (index < 2) ? 0.5f : 0f, 0.5f, 0.5f);
        }
    }
}
