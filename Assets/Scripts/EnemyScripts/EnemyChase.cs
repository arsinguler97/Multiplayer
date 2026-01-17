using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private float repathInterval = 0.25f;
    [SerializeField] private float chaseSpeed = 8f;
    [SerializeField] private float chaseAcceleration = 20f;

    private EnemyController _controller;
    private NavMeshAgent _agent;
    private float _nextRepathTime;

    public void Initialize(EnemyController controller)
    {
        _controller = controller;
        _agent = controller.Agent;
    }

    public void Enter()
    {
        if (_agent == null) return;
        _agent.isStopped = false;
        if (chaseSpeed > 0f) _agent.speed = chaseSpeed;
        if (chaseAcceleration > 0f) _agent.acceleration = chaseAcceleration;
        _agent.stoppingDistance = 0f;
        _agent.autoBraking = false;
        _nextRepathTime = 0f;
    }

    public void Tick()
    {
        if (_agent == null) return;
        Transform target = _controller.Target;
        if (target == null) return;

        if (Time.time >= _nextRepathTime)
        {
            _agent.SetDestination(target.position);
            _nextRepathTime = Time.time + repathInterval;
        }

    }

    public void Exit()
    {
    }
}
