using UnityEngine;

public class AttackEvents : MonoBehaviour
{
    public Collider hitbox;

    public void ActivarHitbox()
    {
        hitbox.enabled = true;
    }

    public void DesactivarHitbox()
    {
        hitbox.enabled = false;
    }
}