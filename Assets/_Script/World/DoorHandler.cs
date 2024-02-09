using System;
using DG.Tweening;
using Game.UI;
#if UNITY_EDITOR
#endif
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class DoorHandler : MonoBehaviour, IInteractable, IHaveIdentity
    {
        [Inject] private SignalBus _signalBus;

        [Header("Door Parameters")]
        [SerializeField] private Transform _door;
        [SerializeField] private Transform _doorHandle;
        [SerializeField] private bool _isLocked;
        private Vector2 _doorRotationRange;
        [SerializeField] private float _torqueForce = 140;
        [SerializeField] private float _inputRangeFromCenter = 5; //get center of screen, origin +- limits. -- in pixels
   
        private InteractionStat m_endStat;
        private InteractionStat m_startStat;
        private bool m_isActive;
        private float m_input_delta;
        private Rigidbody m_rbDoor;
        private Sequence m_handleSeq;
        
        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.ControlledWithX;
        bool IInteractable.IsActive
        {
            get => m_isActive;
            set => m_isActive = value;
        }
        InteractionStat IInteractable.EndStat
        {
            get => m_endStat;
            set => m_endStat = value;
        }
        InteractionStat IInteractable.StartStat
        {
            get => m_startStat;
            set => m_startStat = value;
        }

        GameObject IInteractable.GetInteractionGameObject()
        {
            // todo can return Iinteractable info laters.
            return gameObject;
        }
        
        public int Id { get; set; }
        
        public void GenerateUniqueId()
        {
          Id = UniqueIDHelper.GenerateUniqueId(this);
        }

        private void Start()
        {
            GenerateUniqueId();
            m_rbDoor = _door.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (m_isActive && m_startStat.Equals(default(InteractionStat)) == false)
            {
                UpdateDoor();
            }
        }

        public void SetLockedState(bool isLocked)
        {
            _isLocked = isLocked;
        }

        private void OnLocked()
        {
            ScreenDubegger._objectUsedDebug = gameObject.name + " is locked.";
        }

        private void UpdateDoor()
        {
            m_input_delta = m_startStat.MousePos.x - Input.mousePosition.x;
            var distanceDelta = Vector2.Distance(m_startStat.MousePos, Input.mousePosition);
            var clampedDelta = Mathf.Clamp(distanceDelta, -_inputRangeFromCenter, _inputRangeFromCenter);
            var interpolatedVal = Mathf.InverseLerp(-_inputRangeFromCenter, _inputRangeFromCenter, clampedDelta);
            Vector3 torqueDirection = (m_input_delta >= 0) ? Vector3.up : Vector3.down;

            Vector3 torque = torqueDirection * (interpolatedVal * _torqueForce);
            m_rbDoor.AddTorque(torque * Time.fixedDeltaTime, ForceMode.VelocityChange);
            
            //todo check door angle and fire signal if angle meets opening condition:
            _signalBus.Fire(new CoreSignals.DoorWasOpenedSignal(Id));
        }

        private void UpdateHandle(bool isGrabbed)
        {
            m_handleSeq?.Kill();
            m_handleSeq = DOTween.Sequence();

            m_handleSeq.Insert(0,
                _doorHandle.DOLocalRotate(isGrabbed ? Vector3.forward * 35f : Vector3.zero, 0.35f).SetEase(Ease.InOutBack));
        }

        void IInteractable.InteractStart(InteractionStat stat, Action callback)
        {
            UpdateHandle(true);
            if (_isLocked)
            {
                OnLocked();
                ScreenDubegger._objectUsedDebug = gameObject.name + " is locked. Interaction failed. ";
                return;
            }
            
            ScreenDubegger._objectUsedDebug = "Started interacting with " + gameObject.name + "(" + Id + ")" + " at " + m_startStat.Time;
            m_isActive = true;
            m_startStat = stat;
        }

        void IInteractable.InteractEnd(InteractionStat stat, Action callback)
        {
            ScreenDubegger._objectUsedDebug = "Finished interacting with " + gameObject.name + "(" + Id + ")" + " at " + m_endStat.Time;
            m_isActive = false;
            m_endStat = stat;
            UpdateHandle(false);
        }
        
#if UNITY_EDITOR     
       //gizmos 
       void OnSceneGUI() 
       {
       }
    }
#endif

}