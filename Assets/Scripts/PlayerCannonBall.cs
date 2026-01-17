using UnityEngine;

public class PlayerCannonBall : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private string enemyTag = "Enemy";

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (!other.CompareTag(enemyTag)) return;

        EnemyHealth health = other.GetComponentInParent<EnemyHealth>();
        if (health == null) return;

        health.TakeDamage(damage);
        Destroy(gameObject);
    }
}
