using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("Targeting")]
    [SerializeField] private string targetTag = "PlayerShip";
    [SerializeField] private float detectionRange = 30f;
    [SerializeField] private float attackRange = 15f;
    [SerializeField] private float senseInterval = 0.25f;

    private NavMeshAgent _agent;
    private EnemyPatrol _patrol;
    private EnemyChase _chase;
    private EnemyAttack _attack;
    private Transform _target;
    private EnemyState _state = (EnemyState)(-1);
    private float _nextSenseTime;

    public Transform Target => _target;
    public float AttackRange => attackRange;
    public NavMeshAgent Agent => _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _patrol = GetComponent<EnemyPatrol>();
        _chase = GetComponent<EnemyChase>();
        _attack = GetComponent<EnemyAttack>();

        if (_agent == null)
        {
            Debug.LogWarning($"{name} needs a NavMeshAgent.", this);
        }
    }

    private void Start()
    {
        if (_patrol != null) _patrol.Initialize(this, transform.position);
        if (_chase != null) _chase.Initialize(this);
        if (_attack != null) _attack.Initialize(this);

        ChangeState(EnemyState.Patrol);
    }

    private void Update()
    {
        Sense();

        switch (_state)
        {
            case EnemyState.Patrol:
                if (_patrol != null) _patrol.Tick();
                break;
            case EnemyState.Chase:
                if (_chase != null) _chase.Tick();
                break;
            case EnemyState.Attack:
                if (_attack != null) _attack.Tick();
                break;
        }
    }

    private void Sense()
    {
        if (Time.time < _nextSenseTime) return;
        _nextSenseTime = Time.time + senseInterval;

        if (_target == null)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange, ~0, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hits.Length; i++)
            {
                Transform hitTransform = hits[i].transform;
                if (hitTransform.CompareTag(targetTag) || hitTransform.root.CompareTag(targetTag))
                {
                    _target = hitTransform.root;
                    break;
                }
            }
        }

        if (_target == null)
        {
            if (_state != EnemyState.Patrol)
            {
                ChangeState(EnemyState.Patrol);
            }
            return;
        }

        float dist = Vector3.Distance(transform.position, _target.position);

        if (dist <= attackRange)
        {
            if (_state != EnemyState.Attack)
            {
                ChangeState(EnemyState.Attack);
            }
        }
        else if (dist <= detectionRange)
        {
            if (_state != EnemyState.Chase)
            {
                ChangeState(EnemyState.Chase);
            }
        }
        else
        {
            _target = null;
            if (_state != EnemyState.Patrol)
            {
                ChangeState(EnemyState.Patrol);
            }
        }
    }

    private void ChangeState(EnemyState next)
    {
        if (_state == next) return;

        switch (_state)
        {
            case EnemyState.Patrol:
                if (_patrol != null) _patrol.Exit();
                break;
            case EnemyState.Chase:
                if (_chase != null) _chase.Exit();
                break;
            case EnemyState.Attack:
                if (_attack != null) _attack.Exit();
                break;
        }

        _state = next;

        Debug.Log($"{name} entered {_state} state.", this);

        switch (_state)
        {
            case EnemyState.Patrol:
                if (_patrol != null) _patrol.Enter();
                break;
            case EnemyState.Chase:
                if (_chase != null) _chase.Enter();
                break;
            case EnemyState.Attack:
                if (_attack != null) _attack.Enter();
                break;
        }
    }

    private void OnDrawGizmos()
    {
        DrawRanges();
    }

    private void OnDrawGizmosSelected()
    {
        DrawRanges();
    }

    private void DrawRanges()
    {
        Gizmos.color = new Color(0f, 0.6f, 1f, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionRange);

        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.2f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
