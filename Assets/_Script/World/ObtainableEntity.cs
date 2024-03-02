using System;
using Game.Player;
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
    }
}