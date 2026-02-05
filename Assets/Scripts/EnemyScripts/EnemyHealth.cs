using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private float deathParticleLifetime = 3f;

    private float _current;

    public float Current => _current;
    public float Max => maxHealth;

    public event Action<float, float> HealthChanged;

    private void Awake()
    {
        _current = maxHealth;
        HealthChanged?.Invoke(_current, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        _current = Mathf.Max(0f, _current - amount);
        HealthChanged?.Invoke(_current, maxHealth);
        if (_current <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        AudioManager.Instance?.PlayEnemyDeath();
        GameManager.Instance?.OnEnemyDefeated();
        SpawnDeathParticle();
        if (destroyOnDeath) Destroy(gameObject);
    }

    private void SpawnDeathParticle()
    {
        if (deathParticlePrefab == null) return;

        GameObject fx = Instantiate(deathParticlePrefab, transform.position, transform.rotation);
        Destroy(fx, deathParticleLifetime);
    }
}
