using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class ZoneBase : WorldEntity
    {
        [Inject] private readonly SignalBus _bus;
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
            
            if (_layoutsQueue == null)
                _layoutsQueue = new Queue<LayoutRegistry>();
            
            foreach (var unit in _layoutUnits)
            {
                if (_layoutUnits.IndexOf(unit) == 0)
                {
                    unit.IsActive = true;
                    _bus.Fire(new CoreSignals.OnLayoutStateUpdateSignal(unit.GroupA[0]));
                }
                else unit.IsActive = false;
                
                _layoutsQueue.Enqueue(unit);
            }
        }
        
        public void ActivateTheNextLayout()
        {
            if (_layoutsQueue == null || _layoutsQueue.Count == 0) return;
            
            LayoutRegistry nextLayoutRegistry = _layoutsQueue.Peek();

            if (nextLayoutRegistry != null)
            {
                if (nextLayoutRegistry.KeepGroupA == false)
                    nextLayoutRegistry.Swap();
                _layoutsQueue.Dequeue();
            }
            
            nextLayoutRegistry = _layoutsQueue?.Peek();
            if (nextLayoutRegistry == null) return;
            _bus.Fire(new CoreSignals.OnLayoutStateUpdateSignal(nextLayoutRegistry.GroupA[0]));
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
    public class LayoutRegistry
    {
        public bool KeepGroupA;
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