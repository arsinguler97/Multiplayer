using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private bool stopToFire = true;
    [SerializeField] private bool rotateToTarget = true;
    [SerializeField] private bool broadsideAim = true;
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private bool aimCannons = true;
    [SerializeField] private float cannonRotateSpeed = 180f;
    [SerializeField] private float sideDeadZone = 0.5f;

    private int _attackSide = 1;
    [SerializeField] private EnemyCannonController[] cannons;

    private EnemyController _controller;
    private NavMeshAgent _agent;
    private float _nextFireTime;

    public void Initialize(EnemyController controller)
    {
        _controller = controller;
        _agent = controller.Agent;
        if (cannons == null || cannons.Length == 0)
        {
            cannons = GetComponentsInChildren<EnemyCannonController>();
        }
    }

    public void Enter()
    {
        if (_agent != null && stopToFire) _agent.isStopped = true;
        _nextFireTime = Time.time;
        ChooseAttackSide();
    }

    public void Tick()
    {
        Transform target = _controller.Target;
        if (target == null) return;

        if (rotateToTarget)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            if (toTarget.sqrMagnitude > 0.01f)
            {
                Vector3 desiredForward = toTarget.normalized;
                if (broadsideAim)
                {
                    Vector3 forwardA = Vector3.Cross(desiredForward, Vector3.up);
                    Vector3 forwardB = Vector3.Cross(Vector3.up, desiredForward);
                    desiredForward = _attackSide >= 0 ? forwardA : forwardB;
                }

                Quaternion desired = Quaternion.LookRotation(desiredForward, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desired, rotateSpeed * Time.deltaTime);
            }
        }

        if (aimCannons && cannons != null)
        {
            for (int i = 0; i < cannons.Length; i++)
            {
                if (cannons[i] != null)
                {
                    cannons[i].AimAt(target.position, cannonRotateSpeed);
                }
            }
        }

        if (Time.time >= _nextFireTime)
        {
            FireAll();
            _nextFireTime = Time.time + Mathf.Max(0.05f, fireRate);
        }
    }

    public void Exit()
    {
        if (_agent != null && stopToFire) _agent.isStopped = false;
    }

    private void FireAll()
    {
        if (cannons == null || cannons.Length == 0) return;
        Transform target = _controller.Target;
        if (target == null)
        {
            for (int i = 0; i < cannons.Length; i++)
            {
                if (cannons[i] != null) cannons[i].Fire();
            }
            return;
        }

        Vector3 targetLocal = transform.InverseTransformPoint(target.position);
        float side = targetLocal.x;
        if (Mathf.Abs(side) > sideDeadZone)
        {
            _attackSide = side >= 0f ? 1 : -1;
        }
        bool fireAll = Mathf.Abs(side) <= sideDeadZone;

        for (int i = 0; i < cannons.Length; i++)
        {
            if (cannons[i] == null) continue;

            Transform firePoint = cannons[i].FirePoint;
            if (firePoint == null || fireAll)
            {
                cannons[i].FireAt(target.position);
                continue;
            }

            float cannonSide = transform.InverseTransformPoint(firePoint.position).x;
            if (_attackSide >= 0 && cannonSide >= 0f)
            {
                cannons[i].FireAt(target.position);
            }
            else if (_attackSide < 0 && cannonSide < 0f)
            {
                cannons[i].FireAt(target.position);
            }
        }
    }

    private void ChooseAttackSide()
    {
        Transform target = _controller != null ? _controller.Target : null;
        if (target == null) return;
        Vector3 targetLocal = transform.InverseTransformPoint(target.position);
        if (Mathf.Abs(targetLocal.x) > sideDeadZone)
        {
            _attackSide = targetLocal.x >= 0f ? 1 : -1;
        }
    }
}
