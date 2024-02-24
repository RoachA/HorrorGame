using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private UnityEvent[] _registeredEvents;

    public void CallForAnimationEvents()
    {
        foreach (var eEvent in _registeredEvents)
        {
            if (eEvent != null) eEvent.Invoke();
        }
    }
}
