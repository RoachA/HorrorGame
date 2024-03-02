using System.Collections.Generic;
using Game.World.Objects;
using UnityEngine;
using Zenject;

namespace Game.World
{
    public class WorldObjectsContainer : IInitializable
    {
        private Dictionary<int, GameObject> m_objects;
        
        public void Initialize()
        {
        }

        public WorldObjectsContainer()
        {
            m_objects = new Dictionary<int, GameObject>();
        }
        

        public void RegisterObject(IHaveIdentity obj)
        {
           // if (m_objects == null) m_objects = new Dictionary<int, GameObject>();
            
            if (!m_objects.ContainsKey(obj.Id))
            {
                m_objects.Add(obj.Id, obj.Object);
            }
        }

        public void RemoveObject(int id)
        {
            int target = 0;
            foreach (var kvp in m_objects)
            {
                if (kvp.Key == id) target = kvp.Key;
                break;
            }
            
            m_objects.Remove(target);
        }

        public GameObject GetObjectByID(int id)
        {
            if (m_objects.TryGetValue(id, out GameObject obj))
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