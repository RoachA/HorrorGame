using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World.Objects
{
    public class CabinetEntity : WorldEntity
    {
        [SerializeField] private List<CabinetLidView> _availableLids;
        
        [Button]
        private void GetAvailableLids()
        {
            _availableLids.Clear();
            _availableLids = GetComponentsInChildren<CabinetLidView>().ToList();
        }
    }
}