using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game.World.Objects
{
    public class DoorHandler : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform _door;
        [SerializeField] private float _doorAreaStart;
        [SerializeField] private float _doorAreaEnd;
        //[SerializeField] private 
        void IInteractable.InteractStart()
        {
           
        }

        void IInteractable.InteractEnd()
        {
            
        }
#if UNITY_EDITOR     
       //gizmos 
       void OnSceneGUI() 
       {
         
       }
    }
#endif

}