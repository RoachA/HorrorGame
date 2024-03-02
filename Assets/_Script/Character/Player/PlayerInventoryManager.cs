using System.Collections.Generic;
using Game.World.Objects;
using Zenject;

namespace Game.Player
{
    public class PlayerInventoryManager : IInitializable
    {
        private Dictionary<ObtainableType, List<IObtainable>> m_inventory;
        
        public PlayerInventoryManager()
        {
            m_inventory = new Dictionary<ObtainableType, List<IObtainable>>();
        }

        public List<IObtainable> GetAllInventoryItems()
        {
            var fullList = new List<IObtainable>();

            foreach (var kv2 in m_inventory)
            {
                fullList.AddRange(kv2.Value);
            }

            return fullList;
        }

        public void RegisterToInventory(IObtainable obtainedObj)
        {
            var typeOfObj = obtainedObj.Type;
            
            if (m_inventory == null) m_inventory = new Dictionary<ObtainableType, List<IObtainable>>();
            
            if (m_inventory.ContainsKey(typeOfObj) == false)
            {
                m_inventory.Add(typeOfObj, new List<IObtainable>());
            }
            
            m_inventory[typeOfObj].Add(obtainedObj);
        }

        public void RemoveFromInventory(IObtainable item)
        {
            if (GetInventoryItem(item, out var targetItem))
            {
                if (GetList(targetItem.Type, out var objectOfTypeList)) objectOfTypeList.Remove(item);
            }
        }

        public bool GetInventoryItem(IObtainable obj, out IObtainable inventoryItem)
        {
            inventoryItem = null;

            if (GetList(obj.Type, out var objectsOfType))
            {
                foreach (var inventoryObj in objectsOfType)
                {
                    if (inventoryObj == obj)
                    {
                        inventoryItem = inventoryObj;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool GetInventoryItem(int id, out IObtainable inventoryItem)
        {
            inventoryItem = null;
            
            foreach (var kvp in m_inventory)
            {
                foreach (var item in kvp.Value)
                {
                    if (item.Id == id)
                    {
                        inventoryItem = item;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool GetList(ObtainableType type, out List<IObtainable> list)
        {
            list = new List<IObtainable>();
            
            if (m_inventory.ContainsKey(type))
            {
                list = m_inventory[type];
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Initialize()
        {
        }
    }
}