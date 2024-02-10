using System.Collections.Generic;
using UnityEngine;

namespace Game.World
{
    public class UniqueObjectsContainer
    {
        private Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();

        public void RegisterObject(IHaveIdentity obj)
        {
            if (!_objects.ContainsKey(obj.Id))
            {
                _objects.Add(obj.Id, obj.Object);
            }
            else
            {
                Debug.LogError($"Object with ID {obj.Object} is already registered.");
            }
        }

        public GameObject GetObjectByID(int id)
        {
            if (_objects.TryGetValue(id, out GameObject obj))
            {
                return obj;
            }
            else
            {
                Debug.LogError($"Object with ID {id} is not registered.");
                return null;
            }
        }
    }
}