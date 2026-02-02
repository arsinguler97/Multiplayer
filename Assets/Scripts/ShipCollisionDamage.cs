using UnityEngine;

public class ShipCollisionDamage : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldownSeconds = 1f;
    [SerializeField] private LayerMask targetLayers;

    private float _nextDamageTime;

    private void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (other == null) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0) return;
        if (Time.time < _nextDamageTime) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            AudioManager.Instance?.PlayEnemyDamaged();
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        if (enemy == null && player == null) return;
        _nextDamageTime = Time.time + cooldownSeconds;
    }
}
