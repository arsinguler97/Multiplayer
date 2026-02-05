using UnityEngine;

public class PlayerCannonBall : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private float hitParticleLifetime = 2f;

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
            SpawnHitParticle(other);
            Destroy(gameObject);
            return;
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            SpawnHitParticle(other);
            Destroy(gameObject);
        }
    }

    private void SpawnHitParticle(Collider other)
    {
        if (hitParticlePrefab == null || other == null) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);
        if ((hitPoint - transform.position).sqrMagnitude < 0.0001f)
        {
            hitPoint = transform.position;
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        Vector3 forward = rb != null && rb.linearVelocity.sqrMagnitude > 0.001f
            ? rb.linearVelocity.normalized
            : transform.forward;

        GameObject fx = Instantiate(hitParticlePrefab, hitPoint, Quaternion.LookRotation(forward, Vector3.up));
        Destroy(fx, hitParticleLifetime);
    }
}
