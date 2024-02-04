using System;
using DG.Tweening;
using Game.World;
using RootMotion.FinalIK;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour, IHaveIdentity
{
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private PatrolManager _patrolManager;
    [SerializeField] private LookAtIK _lookAtManager;
    
    [SerializeField] private Transform _lookAtTarget;

    private Vector3[] _pathArray; 
    private bool _isPatrolling = true;
    private int m_currentNodeIndex = 0;
    private Vector3 m_target_patrolPoint;

    public int Id { get; set; }

    public void GenerateUniqueId()
    {
        Id = UniqueIDHelper.GenerateUniqueId(this);
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
        _animator.SetTrigger("IDLE");
        _lookAtManager.solver.SetLookAtWeight(0);
    }

    void LateUpdate()
    {
        if (_isPatrolling == false) return;
        UpdateDestination();

        if (Vector3.Distance(transform.position, m_target_patrolPoint) < 0.5f)
        {
            IterateDestinationPointIndex();
            UpdateDestination();
        }
    }

    private void UpdateDestination()
    {
        m_target_patrolPoint = _pathArray[m_currentNodeIndex];
        _navMeshAgent.SetDestination(m_target_patrolPoint);
        _animator.SetTrigger("WALK");
    }

    private void IterateDestinationPointIndex()
    {
        m_currentNodeIndex++;
        if (m_currentNodeIndex > _pathArray.Length - 1)
        {
            m_currentNodeIndex = 0;
        }
    }

    private Sequence _lookSeq;
    //todo placeholder
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You are detected!");
            _lookAtManager.solver.target = other.transform;
            _lookSeq?.Kill();
            _lookSeq = DOTween.Sequence();

            _lookSeq.Append(DOVirtual.Float(0, 1, 1, a =>
            {
                _lookAtManager.solver.SetLookAtWeight(a);
            }));
        }
    }

    [Button]
    private void StartPatrol()
    {
        _isPatrolling = true;
    }
}