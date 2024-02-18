using System;
using System.Collections;
using Game.World;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(EnemyController))]
public class ObserverModule : EntityModuleBase
{
    [SerializeField] private LayerMask m_targetMask;
    [SerializeField] private LayerMask m_obstructionMask;
    [SerializeField] public Transform _inputOrgan;
    [SerializeField] public bool _isObserving;
    
    
    // this map is used to add multipliers for detection algorithm. certain situations make it easier for player to get detected.
    [SerializeField] private ClueWeights _clueWeights;

    [SerializeField] public float _hearingRange;
    
    [SerializeField] public SightParameters _sightFurstrum;

    private const float m_updatePerSecond = .25f;
    private Coroutine m_fovRoutine;
    public bool _canSeePlayer;
    public GameObject _localPlayerRef;
    
    
    protected override void Start()
    {
        base.Start();
        _localPlayerRef = PlayerRef;
    }

    private void OnDisable()
    {
        if (m_fovRoutine != null) StopCoroutine(m_fovRoutine);
    }

    public void SetObserving(bool state)
    {
        if (state && m_fovRoutine != null) StopCoroutine(m_fovRoutine);
        else m_fovRoutine = StartCoroutine(FOVRoutine());
    }
    
    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(m_updatePerSecond);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }
    
    private void FieldOfViewCheck()
    {
        int bufferSize = 10;
        Collider[] hitColliders = new Collider[bufferSize];
        var rangeChecks = Physics.OverlapSphereNonAlloc(transform.position, _sightFurstrum.SightRange, hitColliders, m_targetMask);
        bool wasSeen = _canSeePlayer;

        if (rangeChecks != 0)
        {
            Transform target = hitColliders[0].transform;
            Vector3 directionToTarget = (target.position - _inputOrgan.transform.position).normalized;

            if (Vector3.Angle(_inputOrgan.transform.forward, directionToTarget) < _sightFurstrum.ConeAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(_inputOrgan.transform.position, target.position);

                if (!Physics.Raycast(_inputOrgan.transform.position, directionToTarget, distanceToTarget, m_obstructionMask))
                {
                    if (_canSeePlayer == false) _bus.Fire(new CoreSignals.PlayerWasSightedSignal(ParentController, Time.time));
                    _canSeePlayer = true;
                }
                else
                {
                    _canSeePlayer = false;
                }
            }
            else
                _canSeePlayer = false;
        }
        else if (_canSeePlayer)
            _canSeePlayer = false;
        
        if (wasSeen && _canSeePlayer == false) _bus.Fire(new CoreSignals.PlayerSightWasLostSignal(ParentController, Time.time));
    }
    
    /// <summary>
    /// always hear a spherical area
    /// if hears something - detects source -> check magnitude : this affects the chances of the enemy getting awared or not. further away > less likely use InverseSquare
    /// If enemy hears a thing from a direction and is warned. enemy starts actively looking for it
    /// Enemy turns to the target position : sphere casts and checks within its line of sight
    /// if > player was found, this event returns a target found state and the target's reference
    /// the deletion of the target from the cache is bound to a different module, until then, enemy targets the player.
    /// </summary>
    [Serializable]
    public struct SightParameters
    {
        public float SightRange;
        [Range(0, 360)]
        public float ConeAngle;
    }

    [Serializable]
    public struct ClueWeights
    {
        [Range(0, 1f)]
        public float _lightIntensity;
        [Range(0, 1f)]
        public float _proximity;
        [Range(0, 1f)]
        public float _playerMovementSpeed;
    }
}

