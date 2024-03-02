using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.World.Objects
{
    [CreateAssetMenu(fileName = "ReadableItemData", menuName = "GameStaticData/Objects/ReadableItemData", order = 0)]
    public class ReadableItemData : ObtainableItemData
    {
        [SerializeField] public Sprite _readableAssetSprite;
        
        [ShowInInspector]
        [MultiLineProperty(10)]
        [SerializeField] public string _readableText;
    }
}