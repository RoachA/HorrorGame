using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Game.World
{
    public class ZoneBase : WorldEntity
    {
        [SerializeField] private List<LayoutRegistry> _layoutUnits;
        private Queue<LayoutRegistry> _layoutsQueue;

        /// todo use queue data type here! layout operations should go sequentially
        /// also we must declare these events globally as well. something happening here may affect something elsewhere.
        /// loose coupling is better.
        /// for this layouts may signal their event with an event id?
        /// listeners seek for event ids?
        /// how do we map those events? how to know who expects what?
        /// the best way for this again seems like direct coupling via component references :t
        ///
        /// Maybe a globally accessible layouts - zones manager? That would be reasonable but again.. containing the data in an accesible way...
        /// it is hard. Maybe some BP ? 
        protected override void Start()
        {
            base.Start();
            foreach (var unit in _layoutUnits)
            {
                _layoutsQueue.Enqueue(unit); //when I enquque, each should be turned off except the first. I should track layout activity.
            }
        }

        [Button]
        private void ResetAllLayouts()
        {
            foreach (var unit in _layoutUnits)
            {
                foreach (var layout in unit.GroupA)
                {
                    layout.gameObject.SetActive(true);
                }
                
                foreach (var layout in unit.GroupB)
                {
                    layout.gameObject.SetActive(false);
                }
            }
        }

        public void OperateSwapActionOn(LayoutBase targetLayout)
        {
            if (FindContainingRegistry(targetLayout, out var target) != false)
            {
                target.Swap();
            }
        }

        private bool FindContainingRegistry(LayoutBase targetLayout, out LayoutRegistry containingRegistry)
        {
            containingRegistry = new LayoutRegistry();
            bool wasFound = false;
            
            foreach (var unit in _layoutUnits)
            {
                foreach (var layout in unit.GroupA)
                {
                    if (targetLayout == layout)
                    {
                        wasFound = true;
                        containingRegistry = unit;
                    }
                }
            }
            
            return wasFound;
        }
    }

    [Serializable]
    public struct LayoutRegistry
    {
        public bool IsActive;
        public List<LayoutBase> GroupA;
        public List<LayoutBase> GroupB;

        [ExecuteAlways]
        [Button]
        public void Swap()
        {
            foreach (var layout in GroupA)
            {
                layout.gameObject.SetActive(layout.gameObject.activeSelf == false);
            }
            
            foreach (var layout in GroupB)
            {
                layout.gameObject.SetActive(layout.gameObject.activeSelf == false);
            }
        }
    }
}