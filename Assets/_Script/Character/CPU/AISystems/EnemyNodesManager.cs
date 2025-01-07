using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyNodesManager 
{
    private Dictionary<EnemyEntity, List<EnemyActionNode>> m_teleportNodesMap;

    public void RegisterEnemyController(EnemyEntity enemy, List<EnemyActionNode> nodes)
    {
        if (m_teleportNodesMap == null) m_teleportNodesMap = new Dictionary<EnemyEntity, List<EnemyActionNode>>();
        
        if (m_teleportNodesMap.ContainsKey(enemy))
        {
            m_teleportNodesMap[enemy].Clear();
            m_teleportNodesMap[enemy] = new List<EnemyActionNode>();
            m_teleportNodesMap[enemy] = nodes;
        }
        else
        {
            m_teleportNodesMap.Add(enemy, nodes);
        }
    }

    public List<EnemyActionNode> GetNodesOfTheEnemy(EnemyEntity enemy)
    {
        List<EnemyActionNode> teleportNodes = null;
        
        if (m_teleportNodesMap.TryGetValue(enemy, out teleportNodes) == false)
        {
            Debug.LogError(enemy.name + " contains no teleport nodes.");
        }

        return teleportNodes;
    }

    public void RemoveEnemyFromTheList(EnemyEntity enemy)
    {
        if (m_teleportNodesMap.ContainsKey(enemy))
        {
            m_teleportNodesMap.Remove(enemy);
        }
    }
}
