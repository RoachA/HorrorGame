using System;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class LayoutInputNode : MonoBehaviour
    {
        [Inject] private readonly PlayerController m_player;
       [Sirenix.OdinInspector.ReadOnly] [SerializeField] 
       private LayoutBase _parentNode;
       [SerializeField] private LayoutNodeType _nodeProperties;

       [Sirenix.OdinInspector.ReadOnly]
       [SerializeField]
       [HorizontalGroup("Conditions")]
       private int _requiredConditionsNumber;
       
       [Sirenix.OdinInspector.ReadOnly]
       [SerializeField]
       [HorizontalGroup("Conditions")]
       private int _conditionsFulfilled;
       
       private bool _conditionsMet;
       
       //TRIGGER NODE:
       [ShowIf("@(this._nodeProperties & LayoutNodeType.OnTrigger) == LayoutNodeType.OnTrigger")]
       [BoxGroup("Trigger Check")]
       [SerializeField] private TriggerReadType _triggerType;
       [ShowIf("@(this._nodeProperties & LayoutNodeType.OnTrigger) == LayoutNodeType.OnTrigger")]
       [BoxGroup("Trigger Check")]
       [SerializeField] private BoxCollider _triggerCollider;
       private Action _triggerCallBack;
       private bool m_isTriggerActive;
       
       //ROTATION NODE:
       [ShowIf("@(this._nodeProperties & LayoutNodeType.OnPlayerRotation) == LayoutNodeType.OnPlayerRotation")]
       [BoxGroup("Rotation Check")]
       [SerializeField] private GameObject _rotationCheckingObject;
       [Range(-1, 1)]
       [Tooltip("1 means player looks at you, -1 means player looks right opposite way")]
       [SerializeField] private Vector2 _rotationAngle;
       private bool _checkingRotation;
       
       //OnPlayerRotation
       
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
           if(_checkingRotation)
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
           if ((_nodeProperties & LayoutNodeType.OnItemInteraction) == LayoutNodeType.OnItemInteraction) _requiredConditionsNumber++;
           if ((_nodeProperties & LayoutNodeType.OnPlayerRotation) == LayoutNodeType.OnPlayerRotation) _requiredConditionsNumber++;
       }

       ///SETUP METHODS
       /// TRIGGER------>
       public enum TriggerReadType
       {
           OnEnter = 0,
           OnExit = 1,
           OnActive = 2,
       }
       
       private void HandleTriggerCheck()
       {
           if ((_nodeProperties & LayoutNodeType.OnTrigger) != 0)
           {
               _triggerCallBack = _parentNode.NodeEnabled;
               
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
               _triggerCallBack = null;
           }
       }

       private void OnTriggerEnter(Collider other)
       {
           if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnEnter)
           {
               //todo THERE ARE DIFFERENT TYPES OF CONDITIONALS - DYNAMIC BOOLS & 1 TIME ACTIVATIONS. I must find proper ways to keep mapping those at all times. and inform the parent.
               m_isTriggerActive = true;
               _requiredConditionsNumber++;
           }
       }
       
       private void OnTriggerStay(Collider other)
       {
           if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnActive && m_isTriggerActive == false)
           {
               _requiredConditionsNumber++;
           }
       }

       private void OnTriggerExit(Collider other)
       {
           if (other.CompareTag("Player") && _triggerType == TriggerReadType.OnExit)
           {
               m_isTriggerActive = false;
               _requiredConditionsNumber++;
           }
       }

       ///ON CHECKING PLAYER ROTATION--->

       private void CheckPlayerRotation()
       {
           _rotationCheckingObject.transform.LookAt(m_player.transform, Vector3.up);
           var dotVal = Vector3.Dot(_rotationCheckingObject.transform.position, m_player.transform.position);
           bool conditionMet = dotVal <= _rotationAngle.x && dotVal >= _rotationAngle.y;

           if (conditionMet)
           {
               _requiredConditionsNumber++;
           }
       }
       
       private void HandlePlayerRotationCheck()
       {
           if ((_nodeProperties & LayoutNodeType.OnTrigger) != 0)
           {
               if (_rotationCheckingObject == null)
               {
                   var nodeTarget = new GameObject("Rotation Checker");
                   nodeTarget.transform.SetParent(transform);
                   nodeTarget.transform.position = transform.position;
                   nodeTarget.transform.rotation = quaternion.identity;
                   nodeTarget.transform.LookAt(m_player.transform, Vector3.up);
                   _rotationCheckingObject = nodeTarget;
                   _checkingRotation = true;
               }
           }
           else
           {
               if (_rotationCheckingObject != null) DestroyImmediate(_rotationCheckingObject.gameObject);
               _checkingRotation = false;
           }
       }
       
       /// GUI
       private Color GetConditionColor()
       {
           return _requiredConditionsNumber == _conditionsFulfilled ? Color.green : new Color(.8f, 0.3f, 0.3f);
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