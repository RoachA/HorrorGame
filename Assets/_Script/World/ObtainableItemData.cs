using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.World.Objects
{
    [CreateAssetMenu(fileName = "ObtainableItemData", menuName = "GameStaticData/Objects/ItemData", order = 0)]
    public class ObtainableItemData : ScriptableObject
    {
        [Header("3D Data")]
        [SerializeField] public GameObject _3dObject;
        
        [Space]
        [SerializeField] public Sprite _inventorySprite;
        [SerializeField] public String _ItemName;
        [ShowInInspector]
        [MultiLineProperty(10)]
        [SerializeField] public String _Definition;
    }
}