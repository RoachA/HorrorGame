using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class LayoutBase : WorldEntity
    {
        [Inject] private readonly SignalBus _bus;
        [SerializeField] private List<LayoutInputNode> _layoutInputNodes;

        public bool IsActive;
        private GameObject m_nodesContainer;
        private ZoneBase m_myZoneBase;
        
        [Button]
        private void CreateInputNode()
        {
            if (m_nodesContainer == null)
            {
                var container = new GameObject("Input Nodes Container");
                container.transform.position = transform.position;
                container.transform.SetParent(transform);
                m_nodesContainer = container;
            }
            
            var nodeObj = new GameObject("Input Node_" + _layoutInputNodes.Count + 1);
            nodeObj.transform.position = transform.position;
            nodeObj.transform.SetParent(m_nodesContainer.transform);
            var inputNode = nodeObj.AddComponent<LayoutInputNode>();
            inputNode.InitNode(this);
            _layoutInputNodes.Add(inputNode);
            Selection.activeObject = nodeObj;
        }

        protected override void Start()
        {
            base.Start();
            m_myZoneBase = GetComponentInParent<ZoneBase>();

            _bus.Subscribe<CoreSignals.OnLayoutStateUpdateSignal>(OnLayoutUpdateHappened);
        }

        private void OnDestroy()
        {
            _bus.TryUnsubscribe<CoreSignals.OnLayoutStateUpdateSignal>(OnLayoutUpdateHappened);
        }

        private void OnLayoutUpdateHappened(CoreSignals.OnLayoutStateUpdateSignal signal)
        {
            foreach (var node in _layoutInputNodes)
            {
                node.gameObject.SetActive(signal.Layout == this && signal.State);
            }
        }

        public void OnNodesWereTriggered()
        {
            if (isActiveAndEnabled == false) return;
            if (m_myZoneBase != null) m_myZoneBase.ActivateTheNextLayout();
        }

        [Button]
        private void ClearNodes()
        {
            foreach (var node in _layoutInputNodes)
            {
               DestroyImmediate(node.gameObject);
            }
            
            _layoutInputNodes.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            if (_layoutInputNodes == null || _layoutInputNodes.Count == 0) return;
            foreach (var node in _layoutInputNodes)
            {
                Gizmos.color = Color.yellow * .85f;
                Gizmos.DrawSphere(node.transform.position, 1);
            }
        }
    }
}