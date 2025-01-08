using System;
using System.Collections;
using DG.Tweening;
using Game.World;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

public class EnemyEntity : DynamicEntity
{
    [Inject] private readonly SignalBus _bus;
    [Inject] private readonly PlayerController _player;

    [Header("References")]
    [SerializeField] private GameObject _container;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [ShowIf("@(this._activeBehaviors & EnemyBehaviorFlags.Patroller) == EnemyBehaviorFlags.Patroller")]
    [SerializeField] private LookAtIK _lookAtManager;

    [Space]
    [BoxGroup("AI")]
    [EnumToggleButtons]
    [SerializeField] public EnemyBehaviorFlags _activeBehaviors;
    [ShowIf("@(this._activeBehaviors & EnemyBehaviorFlags.Patroller) == EnemyBehaviorFlags.Patroller")]
    [BoxGroup("Patroller")]
    [SerializeField] private float _walkSpeed = 0.5f;
    [ShowIf("@(this._activeBehaviors & EnemyBehaviorFlags.Chaser) == EnemyBehaviorFlags.Chaser")]
    [BoxGroup("Chaser")]
    [SerializeField] private float _chaseSpeed = 1f;
    [ShowIf("@(this._activeBehaviors & EnemyBehaviorFlags.Chaser) == EnemyBehaviorFlags.Chaser")]
    [BoxGroup("Chaser")]
    [SerializeField] private float _timeUntilStopChase = 6f;

    private bool m_isChasing;

    private bool _isObserving;
    private Vector3[] _pathArray;
    private bool _isPatrolling = true;
    private int m_currentNodeIndex = 0;
    private Vector3 m_target_patrolPoint;

    //Action Routines
    private Coroutine _chaseRoutine;
    private Coroutine _blinkRoutine;

    protected override void Start()
    {
        SetupDependencies();
        base.Start();
        _bus.Subscribe<CoreSignals.PlayerWasSightedSignal>(OnPlayerSighted);
        _bus.Subscribe<CoreSignals.PlayerSightWasLostSignal>(OnPlayerSightLost);
        
    }

    protected void OnDestroy()
    {
        _bus.TryUnsubscribe<CoreSignals.PlayerWasSightedSignal>(OnPlayerSighted);
        _bus.TryUnsubscribe<CoreSignals.PlayerSightWasLostSignal>(OnPlayerSightLost);
    }
    
    /// <summary>
    /// A method that can do many things on sight. but is not robust. 
    /// </summary>
    /// <param name="signal"></param>
    private void OnPlayerSighted(CoreSignals.PlayerWasSightedSignal signal)
    {
        if (GetModule<ObserverModule>() == false) return;
        if (GetModule<ObserverModule>().isActiveAndEnabled == false) return;
        if (_isObserving == false) return;

        if (Id == signal.Agent.Id)
        {
            m_isChasing = true;

            if (_lookAtManager != null)
                SetLookAtState(_player.transform, true, 0.5f);

            ///todo currently it chases ONLY if player is out of sight. it can also be like on sight.
            if (_chaseRoutine != null && ((_activeBehaviors & EnemyBehaviorFlags.Chaser) != 0))
                StopCoroutine(_chaseRoutine);
        }
    }

    private void OnPlayerSightLost(CoreSignals.PlayerSightWasLostSignal signal)
    {
        if (GetModule<ObserverModule>() == false) return;
        if (GetModule<ObserverModule>().isActiveAndEnabled == false) return;

        //if it was chasing it stops.
        if (Id == signal.Agent.Id && ((_activeBehaviors & EnemyBehaviorFlags.Chaser) != 0))
        {
            SetLookAtState(_player.transform, false, 0.5f);
            _chaseRoutine = StartCoroutine(ChaseRoutine());
            _animator.SetTrigger("WALK");
        }
    }

    private void SetupDependencies()
    {
        if (_animator == null) GetComponent<Animator>();
        if (_lookAtManager == null) GetComponent<LookAtIK>();

        if ((_activeBehaviors & EnemyBehaviorFlags.Patroller) != 0 || (_activeBehaviors & EnemyBehaviorFlags.Chaser) != 0)
            PatrolDependencies();

        void PatrolDependencies()
        {
            if (_navMeshAgent == null) GetComponent<NavMeshAgent>();
            _pathArray = GetModule<SimpleNavMeshPatroller>()._patrolNodes.ToArray();
        }

        if (_animator != null) //todo will change
            _animator.SetTrigger("WALK"); //todo will change

        if (_lookAtManager != null)
            _lookAtManager.solver.SetLookAtWeight(0);
    }

    void LateUpdate()
    {
        //Chaser
        if ((_activeBehaviors & EnemyBehaviorFlags.Chaser) != 0) HandleChase();

        //Patroller
        if (_isPatrolling == false || (_activeBehaviors & EnemyBehaviorFlags.Patroller) == 0) return;
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
        _animator.SetBool("CHASE", m_isChasing);
        _navMeshAgent.speed = m_isChasing ? _chaseSpeed : _walkSpeed;
        _isPatrolling = m_isChasing == false;

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

    private IEnumerator BlinkRoutine(Action callback, float duration)
    {
        _container.SetActive(true);
        yield return new WaitForSeconds(duration);
        _container.SetActive(false);
        GetModule<ObserverModule>().SetObserving(false);
        _isObserving = false;
        callback?.Invoke(); // the definition of this action may come from the module.
    }

    private Sequence _lookSeq;
    private bool m_lastLookState;

    private void SetLookAtState(Transform target, bool look, float lookSpeed = 1)
    {
        if (m_lastLookState == look) return;
        if (_lookAtManager == null) return;


        _lookAtManager.solver.target = target;
        _lookSeq?.Kill();
        _lookSeq = DOTween.Sequence();

        _lookSeq.Append(
            DOVirtual.Float(look ? 0 : 1, look ? 1 : 0, lookSpeed, a => { _lookAtManager.solver.SetLookAtWeight(a); }));
    }

    [BoxGroup("Patroller")]
    [ShowIf("@(this._activeBehaviors & EnemyBehaviorFlags.Patroller) == EnemyBehaviorFlags.Patroller")]
    [Button]
    private void StartPatrol()
    {
        _isPatrolling = true;
    }

    [Flags]
    public enum EnemyBehaviorFlags
    {
        None = 0,
        Patroller = 1,
        Teleporter = 2,
        Chaser = 4,
        Attacker = 8,
    }
}