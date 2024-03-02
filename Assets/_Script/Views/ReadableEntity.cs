using System;
using UnityEngine;

namespace Game.World.Objects
{
    public class ReadableEntity : ObtainableEntity, IInteractable, IObservable
    {
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
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "info_gizmo");
        }
    }
}