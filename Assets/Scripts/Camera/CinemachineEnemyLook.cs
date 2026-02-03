using UnityEngine;
using Unity.Cinemachine;

public class CinemachineEnemyLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;
    [SerializeField] private Transform origin;
    [SerializeField] private Transform enemyTarget;
    [SerializeField] private string enemyTag = "Enemy";

    [Header("Detection")]
    [SerializeField] private float range = 250f;
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private bool requireLineOfSight = true;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3 raycastOffset = new Vector3(0f, 2f, 0f);

    [Header("Smoothing")]
    [SerializeField] private float turnSpeed = 80f;
    [SerializeField] private float fullInfluenceRange = 60f;

    private float _defaultAxis;
    private float _currentAxis;

    private void Reset()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
    }

    private void Awake()
    {
        if (orbitalFollow == null)
        {
            orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        }

        if (orbitalFollow != null)
        {
            _defaultAxis = orbitalFollow.HorizontalAxis.Value;
            _currentAxis = _defaultAxis;
        }
    }

    private void LateUpdate()
    {
        if (orbitalFollow == null || origin == null) return;

        if (enemyTarget == null && !string.IsNullOrEmpty(enemyTag))
        {
            GameObject enemy = GameObject.FindGameObjectWithTag(enemyTag);
            if (enemy != null) enemyTarget = enemy.transform;
        }

        float targetAxis = _defaultAxis;

        if (enemyTarget != null)
        {
            Vector3 toEnemy = enemyTarget.position - origin.position;
            float dist = toEnemy.magnitude;

            if (dist > 0.001f && dist <= range)
            {
                Vector3 dir = toEnemy / dist;
                float angle = Vector3.Angle(origin.forward, dir);

                if (angle <= viewAngle * 0.5f && HasLineOfSight(dir, dist))
                {
                    Vector3 localDir = origin.InverseTransformDirection(dir);
                    float desired = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
                    float weight = GetDistanceWeight(dist);
                    targetAxis = Mathf.LerpAngle(_defaultAxis, desired, weight);
                }
            }
        }

        _currentAxis = Mathf.MoveTowardsAngle(_currentAxis, targetAxis, turnSpeed * Time.deltaTime);
        orbitalFollow.HorizontalAxis.Value = _currentAxis;
    }

    private bool HasLineOfSight(Vector3 dir, float dist)
    {
        if (!requireLineOfSight) return true;
        if (obstacleMask.value == 0) return true;

        Vector3 originPos = origin.position + raycastOffset;
        bool blocked = Physics.Linecast(
            originPos,
            enemyTarget.position,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );

        return !blocked;
    }

    private float GetDistanceWeight(float dist)
    {
        if (range <= 0f) return 0f;
        if (dist <= fullInfluenceRange) return 1f;
        if (dist >= range) return 0f;

        float t = (dist - fullInfluenceRange) / Mathf.Max(0.001f, range - fullInfluenceRange);
        return 1f - Mathf.Clamp01(t);
    }
}
