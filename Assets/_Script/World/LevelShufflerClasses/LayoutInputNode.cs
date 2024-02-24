using System;
using System.Runtime.InteropServices;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Game.World
{
    public enum PlayerEffectType
    {
        SwitchFlashLight = 0,
        JitterFlashLight = 1,
    }
    
    public class LayoutInputNode : MonoBehaviour
    {
        [Inject] private readonly AudioManager m_audioManager;
        [Inject] private readonly PlayerController m_player;
        [Inject] private readonly SignalBus _bus;
        [Sirenix.OdinInspector.ReadOnly] [SerializeField]
        private LayoutBase _parentNode;
        [SerializeField] private LayoutNodeType _nodeProperties;

        [BoxGroup("Affect Player")]
        [SerializeField] private bool _affectPlayer;

        [Sirenix.OdinInspector.ShowIf("_affectPlayer")]
        [BoxGroup("Affect Player")]
        [SerializeField] private PlayerEffectType _effectType;

        [BoxGroup("Audio Cue")]
        [SerializeField] private bool _audioCue;
        [Sirenix.OdinInspector.ShowIf("_audioCue")]
        [BoxGroup("Audio Cue")]
        [SerializeField] private GameObject _audioSourceObject;
        [Sirenix.OdinInspector.ShowIf("_audioCue")]
        [BoxGroup("Audio Cue")]
        [SerializeField] private ExclamationType _cueType;

        [Sirenix.OdinInspector.ReadOnly]
        [SerializeField]
        [HorizontalGroup("Conditions")]
        private int _requiredConditionsNumber;

        [Sirenix.OdinInspector.ReadOnly]
        [SerializeField]
        [HorizontalGroup("Conditions")]
        private int _conditionsFulfilled;

        //TRIGGER NODE:
        [Sirenix.OdinInspector.ShowIf("@(this._nodeProperties & LayoutNodeType.OnTrigger) == LayoutNodeType.OnTrigger")]
        [BoxGroup("Trigger Check")]
        [SerializeField] private TriggerReadType _triggerType;
        [Sirenix.OdinInspector.ShowIf("@(this._nodeProperties & LayoutNodeType.OnTrigger) == LayoutNodeType.OnTrigger")]
        [BoxGroup("Trigger Check")]
        [SerializeField] private BoxCollider _triggerCollider;
        private Action _triggerCallBack;
        private bool m_isTriggerActive;

        //ROTATION NODE:
        [Sirenix.OdinInspector.ShowIf("@(this._nodeProperties & LayoutNodeType.OnPlayerRotation) == LayoutNodeType.OnPlayerRotation")]
        [BoxGroup("Rotation Check")]
        [SerializeField] private GameObject _rotationCheckingObject;

        [Sirenix.OdinInspector.ShowIf("@(this._nodeProperties & LayoutNodeType.OnPlayerRotation) == LayoutNodeType.OnPlayerRotation")]
        [BoxGroup("Rotation Check")]
        [SerializeField] private RotationCheckCondition _rotationCondition;

        //OnItemGrab

        //OnItemInteraction

        //OnTimer

        private void OnValidate()
        {
            HandleTriggerCheck();
            HandlePlayerRotationCheck();

            SetConditionRequirement();
        }

        private void Update()
        {
            CheckPlayerRotation();
            if ((_nodeProperties & LayoutNodeType.OnPlayerRotation) != 0 && m_rotationCheckSuccess == false) return;


            if (_requiredConditionsNumber == _conditionsFulfilled) FinalizeAndTerminateThisNode();
        }

        private void FinalizeAndTerminateThisNode()
        {
            if (_audioCue) CallForAudioFeedback();
            if (_affectPlayer) CallForEffectOnPlayer();
            _parentNode.OnNodesWereTriggered();
        }

        private void CallForEffectOnPlayer()
        {
            switch (_effectType)
            {
                case PlayerEffectType.SwitchFlashLight:
                    _bus.Fire(new CoreSignals.OnAffectFlashLightSignal(FlashLightAction.Switch));
                    break;
                case PlayerEffectType.JitterFlashLight:
                    _bus.Fire(new CoreSignals.OnAffectFlashLightSignal(FlashLightAction.TriggerJitter, 4));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        [Sirenix.OdinInspector.ShowIf("_audioCue")]
        [BoxGroup("Audio Cue")]
        [Button]
        private void CreateAudioObject()
        {
            if (_audioSourceObject != null) return;
            var newSource = new GameObject("audio source");
            newSource.transform.position = transform.position;
            newSource.transform.SetParent(transform);

            Selection.activeGameObject = _audioSourceObject;
            _audioSourceObject = newSource;
        }

        private void CallForAudioFeedback()
        {
            if (_audioSourceObject == null)
            {
                _audioSourceObject = gameObject;
                Debug.LogWarning(_parentNode.name + " has no audio source object, so it uses itself.");
            }
            
            m_audioManager.PlayExclamation(_cueType, _audioSourceObject);
        }

        public void InitNode(LayoutBase layoutTarget)
        {
            _parentNode = layoutTarget;
        }

        private void SetConditionRequirement()
        {
            _requiredConditionsNumber = 0;

            if ((_nodeProperties & LayoutNodeType.OnTimer) == LayoutNodeType.OnTimer) _requiredConditionsNumber++;
            if (((_nodeProperties & LayoutNodeType.OnTrigger) == LayoutNodeType.OnTrigger)) _requiredConditionsNumber++;
            if ((_nodeProperties & LayoutNodeType.OnItemGrab) == LayoutNodeType.OnItemGrab) _requiredConditionsNumber++;
            if ((_nodeProperties & LayoutNodeType.OnItemInteraction) == LayoutNodeType.OnItemInteraction)
                _requiredConditionsNumber++;
        }

        ///SETUP METHODS
        /// TRIGGER------>
        public enum TriggerReadType
        {
            OnEnter = 0,
            OnExit = 1,
        }

        private bool m_triggerCheckSuccess;

        private void HandleTriggerCheck()
        {
            if ((_nodeProperties & LayoutNodeType.OnTrigger) != 0)
            {
                if (GetComponent<Collider>())
                {
                    _triggerCollider = GetComponent<BoxCollider>();
                    _triggerCollider.isTrigger = true;
                    return;
                }

                var newCollider = gameObject.AddComponent<BoxCollider>();
                _triggerCollider = newCollider;
                _triggerCollider.isTrigger = true;
            }
            else
            {
                if (_triggerCollider != null) DestroyImmediate(_triggerCollider);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_triggerCheckSuccess) return;

            if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnEnter)
            {
                //todo THERE ARE DIFFERENT TYPES OF CONDITIONALS - DYNAMIC BOOLS & 1 TIME ACTIVATIONS. I must find proper ways to keep mapping those at all times. and inform the parent.
                m_isTriggerActive = true;
                _conditionsFulfilled++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_triggerCheckSuccess) return;

            if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnExit)
            {
                m_isTriggerActive = false;
                _conditionsFulfilled++;
            }
        }

        ///ON CHECKING PLAYER ROTATION--->
        private bool m_rotationCheckSuccess;

        public enum RotationCheckCondition
        {
            OnFacing = 0,
            OnNotFacing = 1,
        }

        private void CheckPlayerRotation()
        {
            if (_rotationCheckingObject == null) return;
            m_rotationCheckSuccess = false;
            _rotationCheckingObject.transform.LookAt(m_player.transform, Vector3.up);
            Vector3 forward = m_player.transform.forward;
            Vector3 toOther = _rotationCheckingObject.transform.position - m_player.transform.position;
            var dotVal = Vector3.Dot(forward, toOther);

            if (_rotationCondition == RotationCheckCondition.OnFacing && (dotVal <= 0.6f || dotVal >= -0.6f))
                m_rotationCheckSuccess = true;
            if (_rotationCondition == RotationCheckCondition.OnNotFacing && (dotVal <= -3f || dotVal >= 3f))
                m_rotationCheckSuccess = true;
        }

        private void HandlePlayerRotationCheck()
        {
            if ((_nodeProperties & LayoutNodeType.OnPlayerRotation) != 0)
            {
                if (_rotationCheckingObject == null)
                {
                    var nodeTarget = new GameObject("Rotation Checker");
                    nodeTarget.transform.SetParent(transform);
                    nodeTarget.transform.position = transform.position;
                    nodeTarget.transform.rotation = quaternion.identity;
                    if (m_player != null)
                        nodeTarget.transform.LookAt(m_player.transform, Vector3.up);
                    _rotationCheckingObject = nodeTarget;
                }
            }
            else
            {
                if (_rotationCheckingObject != null) DestroyImmediate(_rotationCheckingObject.gameObject);
            }
        }

        /// GUI
        private Color GetConditionColor()
        {
            return _requiredConditionsNumber == _conditionsFulfilled ? Color.green : new Color(.8f, 0.3f, 0.3f);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up, _parentNode.transform.position + Vector3.up);

            if (_audioSourceObject != null)
            {
                Gizmos.DrawIcon(_audioSourceObject.transform.position + Vector3.up, "audio_gizmo");
                Gizmos.color = Color.green * 0.8f; 
                Gizmos.DrawWireSphere(_audioSourceObject.transform.position + Vector3.up, 3);
            }
        }
    }

    [Flags]
    public enum LayoutNodeType
    {
        None = 0,
        OnTrigger = 1 << 0,
        OnPlayerRotation = 1 << 1,
        OnItemGrab = 1 << 2,
        OnItemInteraction = 1 << 3,
        OnTimer = 1 << 4,
    }
}