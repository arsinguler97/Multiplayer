using UnityEngine;

public class EnemyCannonBall : MonoBehaviour
{
    [SerializeField] private float damage = 25f;
    [SerializeField] private string playerTag = "PlayerShip";

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        DamageReceiver receiver = other.GetComponent<DamageReceiver>();
        if (receiver == null) return;

        receiver.ApplyDamage(damage);
        Destroy(gameObject);
    }
}
