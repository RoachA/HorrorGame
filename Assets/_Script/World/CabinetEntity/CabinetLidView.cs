using System;
using DG.Tweening;
using RootMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public enum LidType
    {
        Moving = 0,
        Rotating = 1,
    }

    [Serializable]
    public struct LidParams
    {
        public LidType _lidType;
        [Range(0, 1)]
        public float _openVal;
        public Vector2 _bounds;

        public LidParams(LidType lidType, Vector2 bounds, float openVal)
        {
            _lidType = lidType;
            _bounds = bounds;
            _openVal = openVal;
        }
    }
    
    [RequireComponent(typeof(BoxCollider))]
    public class CabinetLidView : MonoBehaviour, IInteractable
    {
        [Inject] private readonly AudioManager _audioManager;
        [SerializeField] private LidParams _lidParams;
        [Header("Inventory")]
        [SerializeField] private WorldEntity[] _lidInventory;
        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.TapInteraction;
        bool IInteractable.IsActive { get; set; }
        MouseInteractionStats IInteractable.EndStats { get; set; }
        MouseInteractionStats IInteractable.StartStats { get; set; }
        private Vector3 m_initOrientation;
        private bool m_wasCached;
        private BoxCollider m_collider;
        private bool m_isOpen;
        private const float sequence_duration = 1;

        private Sequence m_lidSeq;

        public GameObject GetInteractionGameObject()
        {
            return gameObject;
        }
        
        private void Awake()
        {
            if (Application.isPlaying == false) return;
            Init();
        }

        [Button]
        private void Init()
        {
            m_isOpen = false;
            
            if (_lidParams._lidType == LidType.Moving)
                m_initOrientation = transform.localPosition;
            if (_lidParams._lidType == LidType.Rotating)
                m_initOrientation = transform.localEulerAngles;
            m_wasCached = true;

            if (GetComponent<BoxCollider>() == false) gameObject.AddComponent<BoxCollider>();

            m_collider = GetComponent<BoxCollider>();
            m_collider.isTrigger = true;

            HandleLid(0);
        }

        private void OnValidate()
        {
            if (m_wasCached == false) return;
            HandleLid(_lidParams._openVal);
        }

        private void SwitchState()
        {
            m_lidSeq?.Kill();
            m_lidSeq = DOTween.Sequence();
            
            m_lidSeq.Append(DOVirtual.Float(m_isOpen ? 1 : 0, m_isOpen ? 0 : 1, sequence_duration, HandleLid)
                .SetEase(Ease.OutBack));
        }

        private void HandleSfx()
        {
            _audioManager.PlayObjectInteractionSfx(gameObject, m_isOpen,
                _lidParams._lidType == LidType.Moving ? BoolObjectType.wood_drawer : BoolObjectType.wood_lid);
        }
        
        private void HandleLid(float openVal)
        {
            openVal = Mathf.Clamp01(openVal);
            
            if (_lidParams._lidType == LidType.Moving)
            {
                var newZ = Mathf.Lerp(0, _lidParams._bounds.y, openVal);
                transform.localPosition = new Vector3(m_initOrientation.x, m_initOrientation.y, m_initOrientation.z + newZ);
            }
            
            if (_lidParams._lidType == LidType.Rotating)
            {
                var newY = Mathf.Lerp(0, _lidParams._bounds.y, openVal);
                transform.localEulerAngles = new Vector3(m_initOrientation.x, m_initOrientation.y + newY, m_initOrientation.z);
            }
        }

        public void InteractStart(MouseInteractionStats stats, Action callback = null)
        {
            SwitchState();
            Debug.Log(gameObject.name + " was interacted with");
        }

        public void InteractEnd(MouseInteractionStats stats, Action callback = null)
        {
            m_isOpen = !m_isOpen;
            HandleSfx();
        }
    }
}