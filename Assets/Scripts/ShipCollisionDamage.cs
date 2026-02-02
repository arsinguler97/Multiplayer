using UnityEngine;

public class ShipCollisionDamage : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldownSeconds = 1f;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField] private float basePushDistance = 1.5f;
    [SerializeField] private float speedPushMultiplier = 0.2f;
    [SerializeField] private float maxExtraPush = 3f;
    [SerializeField] private float verticalPush = 0f;

    private float _nextDamageTime;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[ShipCollisionDamage] Collision enter: {name} hit {collision.collider.name}", this);
        TryDamage(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ShipCollisionDamage] Trigger enter: {name} hit {other.name}", this);
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (other == null) return;
        if ((targetLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log($"[ShipCollisionDamage] Layer filtered: {other.name} layer {other.gameObject.layer}", this);
            return;
        }
        if (Time.time < _nextDamageTime) return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            AudioManager.Instance?.PlayEnemyDamaged();
            Debug.Log($"[ShipCollisionDamage] Damaged EnemyHealth on {enemy.name}", this);
        }

        PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log($"[ShipCollisionDamage] Damaged PlayerHealth on {player.name}", this);
        }

        if (enemy == null && player == null) return;
        ApplyPush(other.transform);
        _nextDamageTime = Time.time + cooldownSeconds;
    }

    private void ApplyPush(Transform otherTransform)
    {
        Transform selfRoot = transform.root;
        Transform otherRoot = otherTransform.root;
        if (selfRoot == null || otherRoot == null || selfRoot == otherRoot) return;

        Vector3 dir = (otherRoot.position - selfRoot.position);
        dir.y = verticalPush;
        if (dir.sqrMagnitude < 0.0001f) return;
        dir = dir.normalized;

        float selfSpeed = GetShipSpeed(selfRoot);
        float otherSpeed = GetShipSpeed(otherRoot);

        float selfPush = basePushDistance + Mathf.Min(maxExtraPush, selfSpeed * speedPushMultiplier);
        float otherPush = basePushDistance + Mathf.Min(maxExtraPush, otherSpeed * speedPushMultiplier);

        Vector3 selfTarget = selfRoot.position - dir * selfPush;
        Vector3 otherTarget = otherRoot.position + dir * otherPush;

        WarpOrMove(selfRoot, selfTarget);
        WarpOrMove(otherRoot, otherTarget);
    }

    private static void WarpOrMove(Transform root, Vector3 targetPos)
    {
        UnityEngine.AI.NavMeshAgent agent = root.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null && agent.enabled)
        {
            agent.Warp(targetPos);
            return;
        }

        root.position = targetPos;
    }

    private static float GetShipSpeed(Transform root)
    {
        ShipAutoMove ship = root.GetComponent<ShipAutoMove>();
        if (ship != null) return Mathf.Abs(ship.ShipSpeed);
        return 0f;
    }
}
