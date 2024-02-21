using System;
using System.Collections.Generic;
using Game.AI;
using Game.World;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

public class TeleportNode : MonoBehaviour
{
    [Inject] private readonly SignalBus _bus;
    [Inject] private readonly WorldObjectsContainer m_worldObjectsContainer;
    
    [ReadOnly]
    [SerializeField] private EnemyController m_targetEntityModule;
    
    [Range(0, 1)]
    [SerializeField] private float _probabilityOfSpawn = 1f;
    [SerializeField] private float _cooldown;
    [SerializeField] private AfterTeleportBehavior _afterTeleport;
    //enum specific action params:
    [SerializeField] private float _blinkWaitTime = 2f;
    [SerializeField] private bool _hasAudio;
    [Header("Teleport Triggers")]
    [SerializeField] private List<TeleportTrigger> _triggers;

   
    private bool m_showGizmo;
    private bool m_isOnCooldown;

    public void Init(EnemyController parentModule)
    {
        m_targetEntityModule = parentModule;
    }

    public void OnBoundTriggerActivated(IHaveIdentity areaId, float triggerInfluence)
    {
        m_worldObjectsContainer.RegisterObject(areaId);
        //this is for if we want to hear this from elsewhere.
        _bus.Fire(new CoreSignals.PlayerTriggeredTeleportZoneSignal(areaId, Time.time));
        
        if (CheckForTeleport() == false) return;
        
        OnTeleportApproved();
    }

    private void OnTeleportApproved()
    {
        IAction actionVar = null;
        
        switch (_afterTeleport)
        {
            case AfterTeleportBehavior.Blink:
                actionVar = new BlinkAction(_blinkWaitTime, BlinkStartType.OnPlayerView);
                break;
            case AfterTeleportBehavior.SwitchToChase:
                break;
            case AfterTeleportBehavior.MoveAToB:
                break;
            case AfterTeleportBehavior.StayUntil:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if (actionVar == null) return;
        
        _bus.Fire(new CoreSignals.OnTeleportApprovedSignal(m_targetEntityModule, transform.position, actionVar));
    }

    private bool CheckForTeleport()
    {
        //todo : we will make calculations there to determine if the host enemy unit should be called here. always return true now.
        return true;
    }

    public void SetGizmosState(bool state)
    {
        m_showGizmo = state;
    }

    [Button]
    private void CreateNewTriggerZone()
    {
        var newTrigger = new GameObject("TriggerZone");
        newTrigger.transform.parent = transform;
        newTrigger.transform.position = transform.position;
        var comp = newTrigger.AddComponent<TeleportTrigger>();
        comp.Init(this);
        _triggers.Add(comp);
    }

    [Button]
    private void ClearZones()
    {
        if (_triggers == null || _triggers.Count == 0) return;
        
        foreach (var triggerObj in _triggers)
        {
            DestroyImmediate(triggerObj.gameObject);
        }

        _triggers.Clear();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position + Vector3.up * 2f, "teleportNode_gizmo");
        if (m_showGizmo == false) return;
        if (Selection.activeObject == this) return;
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawCube(transform.position + Vector3.up, Vector3.one + Vector3.up);
    }
    
    private void OnDrawGizmosSelected()
    {
        var pos = transform.position;
        Gizmos.color = Color.green * 0.8f;
        Gizmos.DrawCube(pos + Vector3.up, Vector3.one + Vector3.up);
        
        Handles.Label(pos + Vector3.up + Vector3.right, _probabilityOfSpawn.ToString("f2"));
        Handles.Label(pos + Vector3.up * .5f + Vector3.right, _cooldown.ToString("f2"));
        Handles.Label(pos + Vector3.up * .5f + Vector3.left * 1.5f, _afterTeleport.ToString());
        
        if (_hasAudio)
            Gizmos.DrawIcon(pos + Vector3.up * 1.5f, "audio_gizmo", true);
    }
}
