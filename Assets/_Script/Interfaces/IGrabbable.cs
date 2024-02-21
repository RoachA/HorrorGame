using System.Collections;
using System.Collections.Generic;
using Game.World;
using Game.World.Objects;
using UnityEngine;

namespace Game.World.Objects
{
    /// <summary>
    /// Objects that can be grabbed and carried by hand.
    /// </summary>
    public interface IGrabbable : IInteractable, IHaveIdentity
    {
        public bool IsGrabbed { get; set; }

        public void OnGrab();

        public void OnDrop();
    }
}