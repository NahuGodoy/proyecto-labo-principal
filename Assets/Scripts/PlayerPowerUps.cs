using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPowerUps : MonoBehaviour
{
    public enum TipoPowerUp
    {
        Invencibilidad
    }
    public void ActivarPowerUp(TipoPowerUp tipoPowerUp)
    {
        switch (tipoPowerUp)
        {
            case TipoPowerUp.Invencibilidad:
            ActivarInvencibilidad();
            break;
        }
    }

    void ActivarInvencibilidad()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        StartCoroutine(InvencibilidadTemporal(playerHealth, 5f));
    }

    IEnumerator InvencibilidadTemporal(PlayerHealth playerHealth, float duracion)
    {
        if(playerHealth != null)
        {
            playerHealth.invencible = true;
            Debug.Log("invencible");
            yield return new WaitForSeconds(duracion);
            Debug.Log("no invencible");
            playerHealth.invencible = false;
        } else
        {
            Debug.Log("No hay playerHealth");
            yield break;
        }

    }
}
