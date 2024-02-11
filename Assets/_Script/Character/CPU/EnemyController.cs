using System.Collections;
using DG.Tweening;
using Game.World;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemyController : WorldEntity
{
    [Inject] private readonly SignalBus _bus;
    [Inject] private readonly PlayerController _player;
    
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private PatrolManager _patrolManager;
    [SerializeField] private LookAtIK _lookAtManager;

    [Header("Enemy Behavior Parameters")]
    [SerializeField] private float _timeUntilStopChase = 6f;
    [SerializeField] private float _chaseSpeed = 1f;
    [SerializeField] private float _walkSpeed = 0.5f;

    private bool m_isChasing;

    private Vector3[] _pathArray; 
    private bool _isPatrolling = true;
    private int m_currentNodeIndex = 0;
    private Vector3 m_target_patrolPoint;
    private Coroutine _chaseRoutine;
    
    protected override void Start()
    {
        base.Start();
        _bus.Subscribe<CoreSignals.PlayerWasSightedSignal>(OnSpotsPlayer);
        _bus.Subscribe<CoreSignals.PlayerSightWasLostSignal>(OnPlayerSightWasLost);
    }

    private void OnSpotsPlayer(CoreSignals.PlayerWasSightedSignal signal)
    {
        if (Id == signal.Agent.Id)
        {
            m_isChasing = true;
            
            SetLookAtState(_player.transform, true, 0.5f);
            if (_chaseRoutine != null)
                StopCoroutine(_chaseRoutine);
        }
    }

    private void OnPlayerSightWasLost(CoreSignals.PlayerSightWasLostSignal signal)
    {
        if (Id == signal.Agent.Id)
        {
            SetLookAtState(_player.transform, false, 0.5f);
            _chaseRoutine = StartCoroutine(ChaseRoutine());
            _animator.SetTrigger("WALK");
        }
    }
    
    private void Awake()
    {
        SetupDependencies();
    }

    private void SetupDependencies()
    {
        if (_navMeshAgent == null) GetComponent<NavMeshAgent>();
        if (_animator == null) GetComponent<Animator>();
        if (_patrolManager == null) GetComponent<PatrolManager>();
        if (_lookAtManager == null) GetComponent<LookAtIK>();

        _pathArray = _patrolManager._patrolNodes.ToArray();
        _animator.SetTrigger("WALK");
        _lookAtManager.solver.SetLookAtWeight(0);
    }

    void LateUpdate()
    {
        Debug.Log(_navMeshAgent.velocity);
        _animator.SetBool("CHASE", m_isChasing);
        _navMeshAgent.speed = m_isChasing ? _chaseSpeed : _walkSpeed;
        _isPatrolling = m_isChasing == false;
        
        HandleChase();
        
        if (_isPatrolling == false) return;
        UpdateDestination();

        if (Vector3.Distance(transform.position, m_target_patrolPoint) < 0.5f)
        {
            IterateDestinationPointIndex();
            UpdateDestination();
        }
    }

    private const int m_chaseUpdateFreq = 250;
    private int m_currentChaseUpdateTick = 0;
    
    private void HandleChase()
    {
        if (m_isChasing == false) return;

        if (m_currentChaseUpdateTick == m_chaseUpdateFreq) m_currentChaseUpdateTick = 0;

        if (m_currentChaseUpdateTick > 0)
        {
            m_currentChaseUpdateTick++;
            return;
        }
        
        _navMeshAgent.SetDestination(_player.transform.position);
    }

    private void UpdateDestination()
    {
        m_target_patrolPoint = _pathArray[m_currentNodeIndex];
        _navMeshAgent.SetDestination(m_target_patrolPoint);
    }

    private void IterateDestinationPointIndex()
    {
        m_currentNodeIndex++;
        if (m_currentNodeIndex > _pathArray.Length - 1)
        {
            m_currentNodeIndex = 0;
        }
    }

    private IEnumerator ChaseRoutine()
    {
        yield return new WaitForSeconds(_timeUntilStopChase);
        
        m_isChasing = false;
        Debug.Log(Id + " has lost player at: " + Time.time.ToString("F3"));
    }

    private Sequence _lookSeq;
    private bool m_lastLookState;
    private void SetLookAtState(Transform target, bool look, float lookSpeed = 1)
    {
        if (m_lastLookState == look) return;
        
        _lookAtManager.solver.target = target;
        _lookSeq?.Kill();
        _lookSeq = DOTween.Sequence();

        _lookSeq.Append(DOVirtual.Float(look ? 0 : 1, look ? 1 : 0, lookSpeed, a =>
        {
            _lookAtManager.solver.SetLookAtWeight(a);
        }));
    }

    [Button]
    private void StartPatrol()
    {
        _isPatrolling = true;
    }
    
}