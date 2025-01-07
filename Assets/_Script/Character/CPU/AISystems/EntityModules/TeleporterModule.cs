using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using Zenject;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public enum AfterTeleportBehavior
{
    Blink = 0,
    SwitchToChase = 1,
    MoveAToB = 2,
    StayUntil = 3,
    PlayAnim = 4,
}

[RequireComponent(typeof(EnemyEntity))]
public class TeleporterModule : DynamicEntityModuleBase
{
    [Inject] private readonly EnemyNodesManager _enemyNodesManager;

    [SerializeField] private TeleportNode _nodePrefab;
    [SerializeField] private List<TeleportNode> _nodes;

    [HideInInspector] public Vector3 _nodeSpawnPos;
    private Transform m_nodesContainer;

    private List<EnemyActionNode> GetEnemyActionNodes()
    {
        return _nodes.Cast<EnemyActionNode>().ToList();
    }


    protected override void Awake()
    {
        base.Awake();

        _nodeSpawnPos = transform.localPosition + Vector3.forward;
        if (ParentController == null) GetComponent<EnemyEntity>();
        _enemyNodesManager.RegisterEnemyController(ParentController as EnemyEntity, GetEnemyActionNodes());
    }

    //editor only
    public void AddNewNode()
    {
        if (ParentController == null) GetComponent<EnemyEntity>();
        if (m_nodesContainer == null)
        {
            m_nodesContainer = new GameObject("Teleport Nodes Container").transform;
            m_nodesContainer.position = transform.position;
            m_nodesContainer.transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
        }

        var newNode = Instantiate(_nodePrefab, _nodeSpawnPos, Quaternion.identity, m_nodesContainer.transform);
        newNode.Init(ParentController as EnemyEntity);
        _nodes.Add(newNode);
    }

    public void ResetSpawnerPosition()
    {
        _nodeSpawnPos = transform.position + Vector3.forward;
    }

    public void SelectNodesContainer()
    {
        if (m_nodesContainer == null) return;
        Selection.activeObject = m_nodesContainer;
    }

    public void ClearNodes()
    {
        foreach (var node in _nodes)
        {
            if (node != null)
                DestroyImmediate(node.gameObject);
        }

        _nodes.Clear();
    }

    private bool m_nodeGizmosState = false;

    public void ShowGizmos()
    {
        if (m_nodesContainer == null) return;
        if (_nodes.Count == 0) return;

        foreach (var node in _nodes)
        {
            node.SetGizmosState(!m_nodeGizmosState);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white * .7f;
        Gizmos.DrawCube(_nodeSpawnPos + Vector3.up, Vector3.one + Vector3.up);
    }
}

[CustomEditor(typeof(TeleporterModule))]
public class TeleporterModuleEditor : Editor
{
    private bool _stylesReady;
    private GUIStyle greenStyle;
    private GUIStyle redStyle;
    private GUIStyle yellowStyle;

    public GUIStyle GetButtonWithColor(Color color)
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.background = MakeTex(1, 1, color);
        style.hover.background = MakeTex(1, 1, color / 2);

        return style;
    }

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = color;
        }

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_stylesReady == false)
        {
            greenStyle = GetButtonWithColor(Color.green * 0.65f);
            redStyle = GetButtonWithColor(Color.red * .65f);
            yellowStyle = GetButtonWithColor(Color.yellow * .65f);
            _stylesReady = true;
        }

        TeleporterModule myScript = (TeleporterModule) target;

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Add New Nod At Position", greenStyle))
        {
            myScript.AddNewNode();
        }

        if (GUILayout.Button("Reset Spawn Position", yellowStyle))
        {
            myScript.ResetSpawnerPosition();
        }


        if (GUILayout.Button("Clear All Nodes", redStyle))
        {
            myScript.ClearNodes();
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Select Nodes Container"))
        {
            myScript.SelectNodesContainer();
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Gizmos"))
        {
            myScript.ShowGizmos();
        }

        GUILayout.EndHorizontal();
    }

    public void OnSceneGUI()
    {
        var teleportModule = target as TeleporterModule;

        EditorGUI.BeginChangeCheck();
        Vector3 newTargetPosition = Handles.PositionHandle(teleportModule._nodeSpawnPos, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Move Patrol Handle");
            teleportModule._nodeSpawnPos = newTargetPosition;
        }
    }
}