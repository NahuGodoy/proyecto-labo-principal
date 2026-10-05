using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPowerUps : MonoBehaviour
{
    public enum TipoPowerUp
    {
        Invencibilidad,
        Velocidad
    }
    public void ActivarPowerUp(TipoPowerUp tipoPowerUp)
    {
        switch (tipoPowerUp)
        {
            case TipoPowerUp.Invencibilidad:
            ActivarInvencibilidad();
            break;
        }

        switch (tipoPowerUp)
        {
            case TipoPowerUp.Velocidad:
            ActivarVelocidad();
            break;
        }
    }

    void ActivarInvencibilidad()
    {
        PlayerHealth playerHealth = GetComponent<PlayerHealth>();
        StartCoroutine(InvencibilidadTemporal(playerHealth, 5f));
    }

        void ActivarVelocidad()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        StartCoroutine(VelocidadTemporal(playerMovement, 5f));
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
        IEnumerator VelocidadTemporal(PlayerMovement playerMovement, float duracion)
    {
        if(playerMovement != null)
        {
            playerMovement.walkSpeed = 15f;
            playerMovement.runSpeed = 27f;
            Debug.Log("veloz");
            yield return new WaitForSeconds(duracion);
            Debug.Log("lenteja");
            playerMovement.walkSpeed = 5f;
            playerMovement.runSpeed = 10f;     
        }
    }
}
