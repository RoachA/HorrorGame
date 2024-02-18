using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportsManager 
{
    private Dictionary<EnemyController, List<TeleportNode>> m_teleportNodesMap;

    public void RegisterEnemyController(EnemyController enemy, List<TeleportNode> nodes)
    {
        if (m_teleportNodesMap == null) m_teleportNodesMap = new Dictionary<EnemyController, List<TeleportNode>>();
        
        if (m_teleportNodesMap.ContainsKey(enemy))
        {
            m_teleportNodesMap[enemy].Clear();
            m_teleportNodesMap[enemy] = new List<TeleportNode>();
            m_teleportNodesMap[enemy] = nodes;
        }
        else
        {
            m_teleportNodesMap.Add(enemy, nodes);
        }
    }

    public List<TeleportNode> GetNodesOfTheEnemy(EnemyController enemy)
    {
        List<TeleportNode> teleportNodes = null;
        
        if (m_teleportNodesMap.TryGetValue(enemy, out teleportNodes) == false)
        {
            Debug.LogError(enemy.name + " contains no teleport nodes.");
        }

        return teleportNodes;
    }

    public void RemoveEnemyFromTheList(EnemyController enemy)
    {
        if (m_teleportNodesMap.ContainsKey(enemy))
        {
            m_teleportNodesMap.Remove(enemy);
        }
    }
}
