using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public int cantVidas { get; private set; }
    public UnityEvent<PlayerHealth> onDamageTaken;
    private Vector3 respawnPosition;
    private Quaternion respawnRotation;
    private CharacterController characterController;
    private PlayerMovement playerMovement;

    private void Awake()
    {
        respawnPosition = transform.position;
        respawnRotation = transform.rotation;
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void perderVida()
    {
        if (cantVidas <= 0)
        {
            return;
        }
        cantVidas--;
        onDamageTaken?.Invoke(this);
        Respawn();

        if (cantVidas == 0)
            if (GameManager.Instance != null)
            {
            GameManager.Instance.GameOver();
            }
    }

    void Start()
    {
        cantVidas = 5;
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
