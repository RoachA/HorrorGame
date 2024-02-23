using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public class LayoutBase : WorldEntity
    { 
        [SerializeField] private List<LayoutInputNode> _layoutInputNodes;

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
        }

        public void OnNodesWereTriggered()
        {
            if (m_myZoneBase != null) m_myZoneBase.OperateSwapActionOn(this);
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