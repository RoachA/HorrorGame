using System.Collections;
using System.Collections.Generic;
using Game.World.Objects;
using UnityEngine;

public class UniqueIDHelper : MonoBehaviour
{
    private static HashSet<int> usedIds = new HashSet<int>();
    private static Dictionary<int, IHaveIdentity> idToObjectMap = new Dictionary<int, IHaveIdentity>();

    public static int GenerateUniqueId(IHaveIdentity obj)
    {
        int id;
        do
        {
            // Generate a random ID until it's unique
            id = UnityEngine.Random.Range(1, int.MaxValue);
        } while (usedIds.Contains(id));

        usedIds.Add(id);
        idToObjectMap[id] = obj; // Map the ID to the object
        return id;
    }

    
    public static IHaveIdentity GetObjectById(int id)
    {
        if (idToObjectMap.ContainsKey(id))
        {
            return idToObjectMap[id];
        }
        else
        {
            Debug.LogWarning($"No object found with ID: {id}");
            return null;
        }
    }
    
}