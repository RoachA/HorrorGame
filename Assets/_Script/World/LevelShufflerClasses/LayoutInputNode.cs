using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Game.World
{
    public class LayoutInputNode : MonoBehaviour
    {
        [Inject] private readonly AudioManager m_audioManager;
        [Inject] private readonly PlayerController m_player;
        [Inject] private readonly SignalBus _bus;
        [Sirenix.OdinInspector.ReadOnly] [SerializeField]
        private LayoutBase _parentNode;
        [SerializeField] private LayoutNodeType _nodeProperties;

        [BoxGroup("Feedbacks")]
        [SerializeField] private FeedbackBase _feedbackPlayer;
        private bool m_isfeedbackNull => _feedbackPlayer == null;

        [BoxGroup("Feedbacks")]
        [Sirenix.OdinInspector.ShowIf("m_isfeedbackNull")]
        [Button]
        private void CreatePlayer()
        {
            if (_feedbackPlayer == null)
                _feedbackPlayer = gameObject.AddComponent<FeedbackBase>();
        }
       
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
            if (_feedbackPlayer != null)
                _feedbackPlayer.PlayFeedback();
            
            _parentNode.OnNodesWereTriggered();
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
            if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnEnter)
            {
                //todo THERE ARE DIFFERENT TYPES OF CONDITIONALS - DYNAMIC BOOLS & 1 TIME ACTIVATIONS. I must find proper ways to keep mapping those at all times. and inform the parent.
                m_isTriggerActive = true;
                _conditionsFulfilled++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
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