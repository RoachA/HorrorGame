using System;
using System.Threading.Tasks;
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

        [Space]
        [SerializeField] private bool _destroyAfterTrigger;
        [Range(0, 5f)]
        [SerializeField] private float _destroyDelay;

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
                PlayFeedback();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && _triggerType == LayoutInputNode.TriggerReadType.OnExit)
            {
                PlayFeedback();
            }
        }

        public override void PlayFeedback()
        {
            base.PlayFeedback();
            if (_destroyAfterTrigger) DestroyAfterDelay();
        }

        private async void DestroyAfterDelay()
        {
            var delay = Mathf.RoundToInt(_destroyDelay * 1000);
            await Task.Delay(delay);
            Destroy(gameObject);
        }
    }
}