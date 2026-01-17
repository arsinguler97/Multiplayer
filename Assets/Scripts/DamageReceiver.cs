using UnityEngine;

public class DamageReceiver : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    public void ApplyDamage(float amount)
    {
        if (amount <= 0f) return;

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(amount);
        }
    }
}
