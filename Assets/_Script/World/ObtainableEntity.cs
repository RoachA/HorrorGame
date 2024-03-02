using System;
using UnityEngine;

namespace Game.World.Objects
{
    public class ObtainableEntity : WorldEntity, IObtainable
    {
        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.TapInteraction;
        
        [SerializeField] private ObtainableType _obtainableType;
        
        public ObtainableType Type 
        { 
            get => _obtainableType; 
            set => _obtainableType = value; 
        }
        
        
        [SerializeField] private ObtainableItemData obtainableItemData;

        public ObtainableItemData Data 
        { 
            get => obtainableItemData; 
            set => obtainableItemData = value; 
        }

        
        bool IInteractable.IsActive { get; set; }
        MouseInteractionStats IInteractable.EndStats { get; set; }
        MouseInteractionStats IInteractable.StartStats { get; set; }

        public GameObject GetInteractionGameObject()
        {
            return gameObject;
        }

        public void InteractStart(MouseInteractionStats stats, Action callback = null)
        {
        }

        public void InteractEnd(MouseInteractionStats stats, Action callback = null)
        {
        }
        
        public void OnObtained()
        {
        }

        public void OnDiscarded()
        {
        }
    }
}