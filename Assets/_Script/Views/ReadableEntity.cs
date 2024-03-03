using System;
using Game.UI;
using UnityEngine;
using Zenject;

namespace Game.World.Objects
{
    public class ReadableEntity : ObtainableEntity, IInteractable, IObservable
    {
        [Inject] private readonly UIManager _uiManager;
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

        public override void OnObtained()
        {
            base.OnObtained();
            _uiManager.OpenPanel<InventoryDetailPanel>(new InventoryDetailParams(Data as ReadableItemData, false));
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.5f, "info_gizmo");
        }
    }
}