using System.Collections;
using UnityEngine;

public class ShipTerrainCollision : MonoBehaviour
{
    [SerializeField] private float damage = 20f;
    [SerializeField] private float cooldownSeconds = 1f;
    [SerializeField] private LayerMask terrainLayers;
    [SerializeField] private float pushDistance = 2f;
    [SerializeField] private float pushDuration = 0.25f;

    private float _nextDamageTime;
    private Coroutine _pushRoutine;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[ShipTerrainCollision] Trigger enter: {name} hit {other.name}", this);
        TryApply(other);
    }

    private void TryApply(Collider other)
    {
        if (other == null) return;
        if ((terrainLayers.value & (1 << other.gameObject.layer)) == 0)
        {
            Debug.Log($"[ShipTerrainCollision] Layer filtered: {other.name} layer {other.gameObject.layer}", this);
            return;
        }
        // Accept any collider type; rely on layer mask to filter targets.
        if (Time.time < _nextDamageTime)
        {
            Debug.Log("[ShipTerrainCollision] Cooldown active", this);
            return;
        }

        Transform root = transform.root;
        if (root == null) return;

        PlayerHealth player = root.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Vector3 pushDir = GetPushDirection(root.position, other);
        StartPush(root, pushDir);

        _nextDamageTime = Time.time + cooldownSeconds;
    }

    private static Vector3 GetPushDirection(Vector3 selfPos, Collider other)
    {
        Vector3 closest = other.ClosestPoint(selfPos);
        Vector3 dir = (selfPos - closest);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f)
        {
            return Vector3.back;
        }
        return dir.normalized;
    }

    private void StartPush(Transform root, Vector3 dir)
    {
        if (pushDistance <= 0f || pushDuration <= 0f) return;
        if (_pushRoutine != null) StopCoroutine(_pushRoutine);
        _pushRoutine = StartCoroutine(PushRoutine(root, dir));
    }

    private IEnumerator PushRoutine(Transform root, Vector3 dir)
    {
        Vector3 start = root.position;
        Vector3 target = start + dir * pushDistance;
        float elapsed = 0f;

        while (elapsed < pushDuration)
        {
            float t = elapsed / pushDuration;
            root.position = Vector3.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        root.position = target;
        _pushRoutine = null;
    }
}
