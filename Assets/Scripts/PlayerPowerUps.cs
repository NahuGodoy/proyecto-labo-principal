using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPowerUps : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public enum TipoPowerUp
    {
        Invencibilidad
    }
    public TipoPowerUp tipoPowerUp;

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
       StartCoroutine(InvencivilidadTemporal(5f));
    }

    IEnumerator InvencivilidadTemporal(float duracion)
    {
        playerHealth.invencible = true;
        Debug.Log("invencible");
        yield return new WaitForSeconds(duracion);
        Debug.Log("no invencible");
        playerHealth.invencible = false;
    }
}
