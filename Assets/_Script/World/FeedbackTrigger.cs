using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.World
{
    [RequireComponent(typeof(BoxCollider))]
    [Serializable]
    public class FeedbackTrigger : FeedbackBase
    {
        [BoxGroup("Trigger Check")]
        [SerializeField] private BoxCollider _collider;
        [BoxGroup("Trigger Check")]
        [SerializeField] private LayoutInputNode.TriggerReadType _triggerType = LayoutInputNode.TriggerReadType.OnEnter;

        private void Start()
        {
            if (_collider != null) _collider.isTrigger = true;
        }

        private void OnValidate()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
        }
        
         private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && _triggerType == LayoutInputNode.TriggerReadType.OnEnter)
            {
                base.PlayFeedback();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && _triggerType == LayoutInputNode.TriggerReadType.OnExit)
            {
                base.PlayFeedback();
            }
        }
    }
}