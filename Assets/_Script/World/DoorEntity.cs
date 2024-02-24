using System;
using DG.Tweening;
using Game.Player;
using Game.UI;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
#endif
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class DoorEntity : WorldEntity, IUnlockable
    {
        [Inject] private PlayerController _player;
        [Inject] private SignalBus _signalBus;
        [Inject] private PlayerInventoryManager _inventoryManager;
        [Inject] private AudioManager _audioManager;

        [Header("Door Parameters")]
        [BoxGroup("References")]
        [SerializeField] private Transform _door;
        [BoxGroup("References")]
        [SerializeField] private Transform _doorHandle;
        
        [BoxGroup("Door Main Parameters")]
        [SerializeField] private float _torqueForce = 140;
        [BoxGroup("Door Main Parameters")]
        [SerializeField] private float _inputRangeFromCenter = 5; //get center of screen, origin +- limits. -- in pixels

        [BoxGroup("Lock Settings")]
        [SerializeField] private bool _requiresUnlocking;
        [BoxGroup("Lock Settings")]
        [ShowIf("_requiresUnlocking")]
        [SerializeField] private LockType _lockerType;
        [BoxGroup("Lock Settings")]
        [ShowIf("_requiresUnlocking")]
        [SerializeField] private KeyEntity _keyEntity;

        private Vector2 _doorRotationRange;

        public LockType LockType
        {
            get => _lockerType;
            set => _lockerType = value;
        }
        public bool LockedState { get; set; }

        public KeyEntity Key
        {
            get => _keyEntity;
            set => _keyEntity = value;
        }

        [Header("Debug")]
        [SerializeField] private bool _isLocked;
        private MouseInteractionStats m_endStats;
        private MouseInteractionStats m_startStats;
        private bool m_isActive;
        private float m_input_delta;
        private Rigidbody m_rbDoor;
        private Sequence m_handleSeq;
        
        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.MouseInteraction;
        bool IInteractable.IsActive
        {
            get => m_isActive;
            set => m_isActive = value;
        }
        MouseInteractionStats IInteractable.EndStats
        {
            get => m_endStats;
            set => m_endStats = value;
        }
        MouseInteractionStats IInteractable.StartStats
        {
            get => m_startStats;
            set => m_startStats = value;
        }
        
        GameObject IInteractable.GetInteractionGameObject()
        {
            // todo can return Iinteractable info laters.
            return gameObject;
        }
        
        protected override void Start()
        {
            base.Start();
            m_rbDoor = _door.GetComponent<Rigidbody>();
            
            if (_requiresUnlocking == false) return;
            if (_keyEntity == null)
            {
                Debug.LogError("This door requires unlocking but no key was defined for this --> setting as unlocked", gameObject);
                return;
            }

            SetLockedState(true);
        }

        private void FixedUpdate()
        {
            if (m_isActive && m_startStats.Equals(default(MouseInteractionStats)) == false)
            {
                UpdateDoor();
            }
        }
        
        public void OnUnlockAttempt()
        {
            if (_inventoryManager.GetInventoryItem(_keyEntity.Id, out var key))
            {
                _inventoryManager.RemoveFromInventory(key);
                SetLockedState(false);
                ScreenDubegger._objectUsedDebug = gameObject.name + " but is unlocked because the player has a key!";
                return;
            }
            
            _audioManager.PlayDoorSound(DoorInteractionType.Locked, gameObject);
            ScreenDubegger._objectUsedDebug = gameObject.name + " is locked. You need the key.";
        }
        
        public void SetLockedState(bool isLocked)
        {
            _isLocked = isLocked;
        }

        private void UpdateDoor()
        {
            m_input_delta = m_startStats.MousePos.y - Input.mousePosition.y;
            var distanceDelta = Vector2.Distance(m_startStats.MousePos, Input.mousePosition);
            var clampedDelta = Mathf.Clamp(distanceDelta, -_inputRangeFromCenter, _inputRangeFromCenter);
            var interpolatedVal = Mathf.InverseLerp(-_inputRangeFromCenter, _inputRangeFromCenter, clampedDelta);
            Vector3 torqueDirection = (m_input_delta <= 0) ? Vector3.down + _player.transform.forward : Vector3.up + _player.transform.forward;

            Vector3 torque = torqueDirection * (interpolatedVal * _torqueForce);
            m_rbDoor.AddTorque(torque * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }

        private void UpdateHandle(bool isGrabbed)
        {
            m_handleSeq?.Kill();
            m_handleSeq = DOTween.Sequence();

            m_handleSeq.Insert(0,
                _doorHandle.DOLocalRotate(isGrabbed ? Vector3.forward * 35f : Vector3.zero, 0.35f).SetEase(Ease.InOutBack));
        }

        void IInteractable.InteractStart(MouseInteractionStats stats, Action callback)
        {
            UpdateHandle(true);
            if (_isLocked && _requiresUnlocking)
            {
                OnUnlockAttempt();
                ScreenDubegger._objectUsedDebug = gameObject.name + " is locked. Interaction failed. ";
                return;
            }
            
            ScreenDubegger._objectUsedDebug = "Started interacting with " + gameObject.name + "(" + Id + ")" + " at " + m_startStats.Time;
            _signalBus.Fire(new CoreSignals.DoorWasOpenedSignal(Id));
            _audioManager.PlayDoorSound(DoorInteractionType.Open, gameObject);
            m_isActive = true;
            m_startStats = stats;
        }

        void IInteractable.InteractEnd(MouseInteractionStats stats, Action callback)
        {
            ScreenDubegger._objectUsedDebug = "Finished interacting with " + gameObject.name + "(" + Id + ")" + " at " + m_endStats.Time;
            m_isActive = false;
            m_endStats = stats;
            UpdateHandle(false);
            _audioManager.StopSound(SfxType.Door);
        }

        public enum DoorInteractionType
        {
            Open = 0,
            Locked = 1,
            Shut = 2,
        }
        
#if UNITY_EDITOR     
       //gizmos 
       void OnSceneGUI() 
       {
       }

       private void OnDrawGizmos()
       {
           if (_isLocked == true)
           {
               Gizmos.DrawIcon(transform.parent.position + Vector3.up * 3.5f, "lock_gizmo", false);
               Gizmos.color = Color.red * 0.5f;
               Gizmos.DrawCube(transform.parent.position + Vector3.up, Vector3.one + Vector3.up * 2);
           }
       }

       private void OnDrawGizmosSelected()
       {
           Gizmos.color = Color.green * 0.5f;
           if (_keyEntity != null) Gizmos.DrawSphere(_keyEntity.transform.position, 1f);
       }
    }
#endif

}