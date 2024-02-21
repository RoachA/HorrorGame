using System;
using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class KeyEntity : WorldEntity, IObtainable, IUnlock
    {
        [Inject] private PlayerInventoryManager _inventoryManager;
        [Inject] private WorldObjectsContainer _worldObjectsContainer;
        private InteractionMethod m_interactionType;
        public ObtainableType Type { get; set; } = ObtainableType.Key;

        InteractionMethod IInteractable.InteractionType { get; set; } = InteractionMethod.TapInteraction;

        bool IInteractable.IsActive { get; set; }

        MouseInteractionStats IInteractable.EndStats { get; set; }

        MouseInteractionStats IInteractable.StartStats { get; set; }

        private bool _wasObtained;

        public GameObject GetInteractionGameObject()
        {
            return gameObject;
        }

        public void InteractStart(MouseInteractionStats stats, Action callback = null)
        {
            if (_wasObtained) return;
            OnObtained();
            OnDiscarded();
        }

        public void InteractEnd(MouseInteractionStats stats, Action callback = null)
        {
        }
        
        public void OnObtained()
        {
            _wasObtained = true;
            _inventoryManager.RegisterToInventory(this);
            _worldObjectsContainer.RemoveObject(Id);
        }

        public void OnDiscarded()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "key_gizmo");
        }
    }
}