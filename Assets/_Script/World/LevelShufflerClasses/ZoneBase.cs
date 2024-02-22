using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class ZoneBase : WorldEntity
    {
        [SerializeField] private List<LayoutBase> _layoutUnits;
    }
}