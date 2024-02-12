using System;
using UnityEditor;
using UnityEngine;

public class FloorHandler : MonoBehaviour
{
    [SerializeField] private FloorSurfaceType _surfaceType;
    private void Start()
    {
        gameObject.tag = "Floor";
    }

    public FloorSurfaceType GetSurfaceType()
    {
        return _surfaceType;
    }
}

public enum FloorSurfaceType
{
    Wood = 0,
    Concrete = 1,
    Carpet = 2,
}