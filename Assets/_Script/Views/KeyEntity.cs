using System;
using Game.Player;
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class KeyEntity : ObtainableEntity, IUnlock, IInteractable
    {
        [Inject] private PlayerInventoryManager _inventoryManager;
        [Inject] private WorldObjectsContainer _worldObjectsContainer;
        private InteractionMethod m_interactionType;
        
        private bool _wasObtained;

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