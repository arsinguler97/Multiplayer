using UnityEngine;

public class PlayerCannonBall : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private string targetTag = "Enemy";

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            AudioManager.Instance?.PlayEnemyDamaged();
            Destroy(gameObject);
            return;
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
