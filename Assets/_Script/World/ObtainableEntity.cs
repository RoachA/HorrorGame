using System;
using Game.Player;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class ObtainableEntity : WorldEntity, IObtainable
    {
        [Inject] private PlayerInventoryManager _inventoryManager;
        [Inject] private WorldObjectsContainer _worldObjectsContainer;
        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.TapInteraction;
        protected bool _wasObtained;
        
        [SerializeField] private ObtainableType _obtainableType;
        [SerializeField] private ObtainableItemData obtainableItemData;
        
        public ObtainableItemData Data 
        { 
            get => obtainableItemData; 
            set => obtainableItemData = value; 
        }
        
        protected override void Start()
        {
            base.Start();
            Init();
        }
        
        public virtual void Init()
        {
            if (Data._3dObject == null) return;

            var obj = Instantiate(Data._3dObject, Vector3.zero, Quaternion.identity, transform.GetChild(0));
            obj.transform.localPosition = Vector3.zero;
        }

        public ObtainableType Type 
        { 
            get => _obtainableType; 
            set => _obtainableType = value; 
        }
        
        bool IInteractable.IsActive { get; set; }
        MouseInteractionStats IInteractable.EndStats { get; set; }
        MouseInteractionStats IInteractable.StartStats { get; set; }

        public GameObject GetInteractionGameObject()
        {
            return gameObject;
        }

        
        void IInteractable.InteractStart(MouseInteractionStats stats, Action callback = null)
        {
            if (_wasObtained) return;
            OnObtained();
            OnDiscarded();
        }

        void IInteractable.InteractEnd(MouseInteractionStats stats, Action callback = null)
        {
        }
        
        public virtual void OnObtained()
        {
            _wasObtained = true;
            _inventoryManager.RegisterToInventory(this);
            _worldObjectsContainer.RemoveObject(Id);
        }

        public virtual void OnDiscarded()
        {
            Destroy(gameObject);
        }

        protected virtual void OnDrawGizmos()
        {
            Handles.Label(transform.position, Data._ItemName);
            Gizmos.DrawWireSphere(transform.position, .5f);
        }
    }
}