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
    [SerializeField] private float pushDuration = 0.2f;

    private float _nextDamageTime;
    private readonly System.Collections.Generic.Dictionary<Transform, PushState> _pushes =
        new System.Collections.Generic.Dictionary<Transform, PushState>();
    private readonly System.Collections.Generic.List<Transform> _pushKeys =
        new System.Collections.Generic.List<Transform>();
    private readonly System.Collections.Generic.List<Transform> _pushesToRemove =
        new System.Collections.Generic.List<Transform>();

    private void OnCollisionEnter(Collision collision)
    {
        TryDamage(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDamage(other);
    }

    private void Update()
    {
        if (_pushes.Count == 0) return;

        _pushKeys.Clear();
        _pushKeys.AddRange(_pushes.Keys);
        _pushesToRemove.Clear();
        for (int i = 0; i < _pushKeys.Count; i++)
        {
            Transform root = _pushKeys[i];
            if (root == null || !_pushes.TryGetValue(root, out PushState state))
            {
                _pushesToRemove.Add(root);
                continue;
            }

            state.elapsed += Time.deltaTime;
            float t = state.duration > 0f ? Mathf.Clamp01(state.elapsed / state.duration) : 1f;
            Vector3 pos = Vector3.Lerp(state.start, state.target, t);
            WarpOrMove(root, pos);

            if (t >= 1f)
            {
                _pushesToRemove.Add(root);
            }
            else
            {
                _pushes[root] = state;
            }
        }

        for (int i = 0; i < _pushesToRemove.Count; i++)
        {
            _pushes.Remove(_pushesToRemove[i]);
        }
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

        StartPush(selfRoot, selfTarget);
        StartPush(otherRoot, otherTarget);
    }

    private void StartPush(Transform root, Vector3 targetPos)
    {
        if (root == null) return;

        if (pushDuration <= 0f)
        {
            WarpOrMove(root, targetPos);
            return;
        }

        PushState state = new PushState
        {
            start = root.position,
            target = targetPos,
            duration = pushDuration,
            elapsed = 0f
        };

        _pushes[root] = state;
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

    private struct PushState
    {
        public Vector3 start;
        public Vector3 target;
        public float duration;
        public float elapsed;
    }
}
