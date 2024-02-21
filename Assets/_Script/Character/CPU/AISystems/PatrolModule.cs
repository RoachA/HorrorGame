using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class PatrolModule : DynamicEntityModuleBase
{
   [SerializeField] public List<Vector3> _patrolNodes = new List<Vector3>();
   [SerializeField] private bool SetAsLoop = true;

   void OnDrawGizmos()
   {
      // Draw gizmos for each position in the list
      Gizmos.color = Color.green;
      foreach (Vector3 position in _patrolNodes)
      {
         Gizmos.DrawSphere(position, 0.1f);
      }
      
      //draw lines
      for (int i = 0; i < _patrolNodes.Count; i++)
      {
         if (i < _patrolNodes.Count - 1)
         {
            Gizmos.DrawLine(_patrolNodes[i], _patrolNodes[i + 1]);
         }
      }
    
      Gizmos.color = Color.white;
      Gizmos.DrawWireCube(_patrolNodes[_patrolNodes.Count - 1], Vector3.one * 0.25f);

      if (SetAsLoop)
      {
         Gizmos.DrawLine(_patrolNodes[_patrolNodes.Count - 1], _patrolNodes[0]);
      }
   }
   
   private void AddNewNodeAtCenter()
   { 
      Vector3 newNode = transform.TransformPoint(Vector3.zero + Vector3.forward * 0.5f);
      
      if (_patrolNodes.Count > 0)
      {
         newNode = _patrolNodes[_patrolNodes.Count - 1] + Vector3.forward * 0.5f;
      }
      
      _patrolNodes.Add(newNode);
   }
   
   private void AddPosition(Vector3 position)
      {
         _patrolNodes.Add(position);
      }
   
   private void ClearNodes()
   {
      _patrolNodes.Clear();
   }
   
#if UNITY_EDITOR

   [CustomEditor(typeof(PatrolModule))]
   public class PatrolManagerEditor : Editor
   {
      private const float handleSize = 2;
      
      public override void OnInspectorGUI()
      {
         base.OnInspectorGUI();
     
         PatrolModule myScript = (PatrolModule) target;

         GUILayout.BeginHorizontal();
         if (GUILayout.Button("Add New Nod At Center"))
         {
            myScript.AddNewNodeAtCenter();
         }
         
         if (GUILayout.Button("Clear All Nodes"))
         {
            myScript.ClearNodes();
         }
         
         GUILayout.EndHorizontal();
      }
      
      public void OnSceneGUI()
      {
         var patrolClass = target as PatrolModule;

         EditorGUI.BeginChangeCheck();
         for (int i = 0; i < patrolClass._patrolNodes.Count; i++)
         {
            Vector3 newTargetPosition = Handles.PositionHandle(patrolClass._patrolNodes[i], Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
               Undo.RecordObject(this, "Move Patrol Handle");
               patrolClass._patrolNodes[i] = newTargetPosition;
            }
         }
      }
   }
#endif 
}
