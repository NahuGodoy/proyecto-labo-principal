using UnityEngine;
using System;

public class Checkpoint : MonoBehaviour
{
    public Transform respawnPoint;
    [SerializeField] private bool esCheckpointInicial;

    public static Transform CheckpointActivo { get; private set; }
    public static event Action<Transform> CheckpointActivado;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCheckpointActivo()
    {
        CheckpointActivo = null;
        CheckpointActivado = null;
    }

    private void Awake()
    {
        if (esCheckpointInicial)
        {
            ActivarCheckpoint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();
        if (playerHealth != null)
        {
            ActivarCheckpoint();
        }
    }

    private void ActivarCheckpoint()
    {
        CheckpointActivo = respawnPoint != null ? respawnPoint : transform;
        CheckpointActivado?.Invoke(CheckpointActivo);
    }
}