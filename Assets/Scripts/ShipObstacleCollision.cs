using System.Collections;
using UnityEngine;

public class ShipObstacleCollision : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldownSeconds = 1f;
    [SerializeField] private LayerMask obstacleLayers;
    [SerializeField] private float basePushDistance = 2f;
    [SerializeField] private float speedPushMultiplier = 0.2f;
    [SerializeField] private float maxExtraPush = 3f;
    [SerializeField] private float pushDuration = 0.25f;

    private float _nextDamageTime;
    private Coroutine _pushRoutine;

    private void OnCollisionEnter(Collision collision)
    {
        TryHit(collision.collider, true);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other, true);
    }

    private void OnCollisionStay(Collision collision)
    {
        TryHit(collision.collider, false);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other, false);
    }

    private void TryHit(Collider other, bool allowDamage)
    {
        if (other == null) return;
        if (other.transform.root == transform.root) return;
        if ((obstacleLayers.value & (1 << other.gameObject.layer)) == 0) return;

        Transform selfRoot = transform.root;
        if (selfRoot == null) return;

        Vector3 pushDir = GetPushDirection(selfRoot.position, other);
        float pushDistance = GetPushDistance(selfRoot);
        StartPush(selfRoot, pushDir, pushDistance);

        if (!allowDamage) return;
        if (Time.time < _nextDamageTime) return;

        PlayerHealth player = selfRoot.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        _nextDamageTime = Time.time + cooldownSeconds;
    }

    private float GetPushDistance(Transform root)
    {
        float speed = 0f;
        ShipAutoMove ship = root.GetComponent<ShipAutoMove>();
        if (ship != null) speed = Mathf.Abs(ship.ShipSpeed);

        return basePushDistance + Mathf.Min(maxExtraPush, speed * speedPushMultiplier);
    }

    private static Vector3 GetPushDirection(Vector3 selfPos, Collider other)
    {
        Vector3 closest = other.ClosestPoint(selfPos);
        Vector3 dir = selfPos - closest;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = selfPos - other.transform.position;
            dir.y = 0f;
        }

        if (dir.sqrMagnitude < 0.0001f)
        {
            return Vector3.back;
        }

        return dir.normalized;
    }

    private void StartPush(Transform root, Vector3 dir, float distance)
    {
        if (distance <= 0f || pushDuration <= 0f) return;
        if (_pushRoutine != null) StopCoroutine(_pushRoutine);
        _pushRoutine = StartCoroutine(PushRoutine(root, dir, distance));
    }

    private IEnumerator PushRoutine(Transform root, Vector3 dir, float distance)
    {
        Vector3 start = root.position;
        Vector3 target = start + dir * distance;
        float elapsed = 0f;

        while (elapsed < pushDuration)
        {
            float t = elapsed / pushDuration;
            WarpOrMove(root, Vector3.Lerp(start, target, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        WarpOrMove(root, target);
        _pushRoutine = null;
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
}
