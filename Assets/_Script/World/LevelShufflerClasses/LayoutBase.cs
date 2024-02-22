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

        private GameObject _nodesContainer;
        
        [Button]
        private void CreateInputNode()
        {
            if (_nodesContainer == null)
            {
                var container = new GameObject("Input Nodes Container");
                container.transform.position = transform.position;
                container.transform.SetParent(transform);
                _nodesContainer = container;
            }
            
            var nodeObj = new GameObject("Input Node_" + _layoutInputNodes.Count + 1);
            nodeObj.transform.position = transform.position;
            nodeObj.transform.SetParent(_nodesContainer.transform);
            var inputNode = nodeObj.AddComponent<LayoutInputNode>();
            inputNode.InitNode(this);
            _layoutInputNodes.Add(inputNode);
            Selection.activeObject = nodeObj;
        }

        public void NodeEnabled()
        {
            
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

        private void OnDrawGizmos()
        {
            if (_layoutInputNodes == null || _layoutInputNodes.Count == 0) return;
            foreach (var node in _layoutInputNodes)
            {
                Gizmos.color = Color.yellow * 0.25f;
                Gizmos.DrawSphere(node.transform.position, 1);
            }
        }
    }
}