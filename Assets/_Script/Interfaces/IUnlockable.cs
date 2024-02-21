using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.World.Objects
{
    public interface IUnlockable : IInteractable
    {
        public LockType LockType { get; set; }
        public bool LockedState { get; set; }
        public void OnUnlockAttempt();
        
        public KeyEntity Key { get; set; }
    }
    
    public enum LockType
    {
        Key = 0,
        Code = 1,
        Blocker = 2,
    }
}