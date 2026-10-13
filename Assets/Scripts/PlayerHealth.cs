using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    private SharedPlayerStats sharedStats;

    public int cantVidas => sharedStats != null ? sharedStats.cantVidas : 0;
    public UnityEvent<PlayerHealth> onDamageTaken;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private CharacterController characterController;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        sharedStats = SharedPlayerStats.GetOrCreate();
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();

        if (Checkpoint.CheckpointActivo != null)
        {
            SetCheckpoint(Checkpoint.CheckpointActivo);
            transform.SetPositionAndRotation(respawnPosition, respawnRotation);
        }
    }

    private void OnEnable()
    {
        Checkpoint.CheckpointActivado += SetCheckpoint;
    }

    private void OnDisable()
    {
        Checkpoint.CheckpointActivado -= SetCheckpoint;
    }

    public bool invencible = false;

    public void perderVida()
    {
        if (invencible)
        {
            return;
        }
        int vidasRestantes = sharedStats.PerderVida();
        onDamageTaken?.Invoke(this);
        Respawn();

        if (vidasRestantes == 0)
            if (GameManager.Instance != null)
            {
            GameManager.Instance.GameOver();
            }
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        if (checkpoint == null)
        {
            return;
        }

        respawnPosition = checkpoint.position;
        respawnRotation = checkpoint.rotation;
    }

    private void Respawn()
    {
        if (playerMovement != null)
        {
            playerMovement.TeleportTo(respawnPosition, respawnRotation);
            return;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        transform.SetPositionAndRotation(respawnPosition, respawnRotation);

        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }
}
