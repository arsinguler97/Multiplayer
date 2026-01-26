using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [SerializeField] private float patrolRadius = 20f;
    [SerializeField] private float waitTime = 2f;

    private EnemyController _controller;
    private NavMeshAgent _agent;
    private Vector3 _origin;
    private float _nextMoveTime;
    private bool _hasDestination;

    public void Initialize(EnemyController controller, Vector3 origin)
    {
        _controller = controller;
        _agent = controller.Agent;
        _origin = origin;
    }

    public void Enter()
    {
        if (_agent == null) return;
        _agent.isStopped = false;
        _agent.stoppingDistance = 0f;
        _nextMoveTime = Time.time;
        _hasDestination = false;
    }

    public void Tick()
    {
        if (_agent == null) return;
        if (Time.time < _nextMoveTime) return;

        if (!_hasDestination || (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance + 0.2f))
        {
            Vector3 random = _origin + Random.insideUnitSphere * patrolRadius;
            random.y = _origin.y;

            if (NavMesh.SamplePosition(random, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
                _hasDestination = true;
                _nextMoveTime = Time.time + waitTime;
            }
            else
            {
                _nextMoveTime = Time.time + 0.5f;
            }
        }
    }

    public void Exit()
    {
        _hasDestination = false;
    }
}
