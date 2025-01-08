using System;
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
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private LookAtIK _lookAtManager;

    private bool m_isChasing;

    private bool _isObserving;
    private Vector3[] _pathArray;
    private bool _isPatrolling = true;
    private int m_currentNodeIndex = 0;
    private Vector3 m_target_patrolPoint;

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
        }
    }

    private void OnPlayerSightLost(CoreSignals.PlayerSightWasLostSignal signal)
    {
        if (GetModule<ObserverModule>() == false) return;
        if (GetModule<ObserverModule>().isActiveAndEnabled == false) return;
    }

    private void SetupDependencies()
    {
        if (_animator == null) GetComponent<Animator>();
        if (_lookAtManager == null) GetComponent<LookAtIK>();
        PatrolDependencies();

        void PatrolDependencies()
        {
            if (_navMeshAgent == null) GetComponent<NavMeshAgent>();
            _pathArray = GetModule<SimpleNavMeshPatroller>()._patrolNodes.ToArray();
        }

        if (_animator != null)
            _animator.SetTrigger("WALK");

        if (_lookAtManager != null)
            _lookAtManager.solver.SetLookAtWeight(0);
    }

    void LateUpdate()
    {
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
}