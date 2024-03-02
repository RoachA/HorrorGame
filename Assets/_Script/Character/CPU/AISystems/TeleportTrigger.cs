using System.ComponentModel;
using Game.World;
using Game.World.Objects;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour, IHaveIdentity
{
    [SerializeField] private Collider _collider;
    [Range(0, 1)]
    [SerializeField] private float _triggerWeight = .5f;

    [SerializeField] private TeleportNode _parentNode;

    public int Id { get; set; }
    [Sirenix.OdinInspector.ReadOnly]
    public int _idLabel; 
    public GameObject Object { get; set; }
    
    [ExecuteAlways]
    public void Init(TeleportNode nodeBase)
    {
        _collider = gameObject.AddComponent<BoxCollider>();
        _collider.isTrigger = true;
        _parentNode = nodeBase;
        Id = UniqueIDHelper.GenerateUniqueId(this);
        _idLabel = Id;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _parentNode.OnBoundTriggerActivated(this, _triggerWeight);
        }
    }

    private void OnDrawGizmosSelected()
    {
        var colliderPos = _collider.transform.position;
        var parentPos = _parentNode.transform.position;
        Gizmos.color = Color.red * 0.3f;
        Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        
#if UNITY_EDITOR
        // Ensure continuous Update calls.
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
            time += Time.deltaTime / Vector3.Distance(colliderPos, parentPos);
        }
#endif     
        Gizmos.color = Color.green * 0.95f;
        Gizmos.DrawLine(transform.position, parentPos);
        var arrowPos = Vector3.Lerp(colliderPos, parentPos, time);
        if (time > 1) time = 0;
        DrawSphere(arrowPos);
        Handles.Label(transform.position + Vector3.up + Vector3.right, _triggerWeight.ToString("f2"));
        Handles.Label(transform.position + Vector3.down, _idLabel.ToString());
    }

    private float time = 0;
    private float offsetSpeed = 2f;

    private void DrawSphere(Vector3 pos)
    { 
        Gizmos.color = Color.green * 0.8f;
        Gizmos.DrawSphere(pos, 0.15f);
    }
}
