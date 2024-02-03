using UnityEngine;
using UnityEngine.Rendering;

public class LightSourceMesher : MonoBehaviour
{
    [SerializeField] private LightType _lightType;
    [SerializeField] private Light _light;
    private Mesh m_lightMesh;
    private MeshRenderer m_mRenderer;
    private MeshFilter m_mFilter;
    private GameObject m_container;
    private Vector3 m_initialPivotOffset;
    private Material m_lightMat;

    private void Start()
    {
        if (m_container == null) m_container = Instantiate(new GameObject(), transform);
        m_container.name = "light mesh";
        m_container.transform.localEulerAngles = Vector3.zero;
        m_container.transform.localPosition = Vector3.zero;
        int layerIndex = LayerMask.NameToLayer($"LightData");
        m_container.layer = layerIndex;
            
        if (_light == null) _light = GetComponent<Light>();
        _lightType = _light.type;
        
        if (m_container.GetComponent<MeshFilter>() == false) m_container.AddComponent<MeshFilter>();
        if (m_mFilter == null) m_mFilter = m_container.GetComponent<MeshFilter>();
        m_initialPivotOffset = m_mFilter.transform.position - transform.parent.position;
        
        if (m_container.GetComponent<MeshRenderer>() == false) m_container.AddComponent<MeshRenderer>();
        if (m_mRenderer == null) m_mRenderer = m_container.GetComponent<MeshRenderer>();

        m_mFilter.sharedMesh = m_lightMesh;
        m_mRenderer.material = CoreUtils.CreateEngineMaterial(Shader.Find("Universal Render Pipeline/Unlit"));
        m_lightMat = m_mRenderer.material;

        if (_lightType == LightType.Spot)
        {
            //0.5f, 0, 3f, 12 was realistic.
            Mesh mesh = GenerateFrustumMesh(_light.spotAngle / 200, 0, _light.intensity / 2, 10);
            m_mFilter.mesh = mesh;
            m_container.transform.localEulerAngles += new Vector3(0, -90, -90);
        }

        if (_lightType == LightType.Point)
        {
            
        }

        m_mRenderer.sharedMaterial.color = _light.color;
    }

    private void OnValidate()
    {
        UpdateLight();
    }

    private bool CheckReferences()
    {
        bool checks;
        
        checks = !(_light == null);
        checks = !(m_mFilter == null);
        checks = !(m_mRenderer == null);
        
        return checks;
    }

    public void UpdateLight()
    {
        if (CheckReferences() == false) return;
        
        _lightType = _light.type;
        
        if (_lightType == LightType.Spot)
        {
            Mesh mesh = GenerateFrustumMesh(_light.spotAngle / 200, 0, _light.intensity / 2, 10);
            m_lightMat.color = _light.color * Mathf.Clamp01(_light.intensity); 
            m_mFilter.mesh = mesh;
        }
        
        if (_lightType == LightType.Point)
        {
            
        }
    }

    
    public Mesh GenerateFrustumMesh(float topRadius, float bottomRadius, float height, int numSegments)
     {
         // Create a new mesh
         Mesh mesh = new Mesh();

         // Calculate vertices, triangles, and normals
         Vector3[] vertices = new Vector3[numSegments * 2];
         int[] triangles = new int[(numSegments - 1) * 6];
         Vector3[] normals = new Vector3[numSegments * 2];

         float angleIncrement = Mathf.PI * 2f / numSegments;
         float currentAngle = 0f;

         for (int i = 0; i < numSegments; i++)
         {
             float x = Mathf.Cos(currentAngle);
             float z = Mathf.Sin(currentAngle);
             float y = height;

             vertices[i] = new Vector3(x * topRadius, y, z * topRadius);
             vertices[i + numSegments] = new Vector3(x * bottomRadius, 0, z * bottomRadius);

             normals[i] = Vector3.up;
             normals[i + numSegments] = Vector3.down;

             currentAngle += angleIncrement;
         }

         for (int i = 0; i < numSegments - 1; i++)
         {
             int baseIndex = i * 6;

             triangles[baseIndex] = i;
             triangles[baseIndex + 1] = i + 1;
             triangles[baseIndex + 2] = i + numSegments;

             triangles[baseIndex + 3] = i + 1;
             triangles[baseIndex + 4] = i + numSegments + 1;
             triangles[baseIndex + 5] = i + numSegments;
         }

         // Assign vertices, triangles, and normals to the mesh
         mesh.vertices = vertices;
         mesh.triangles = triangles;
         mesh.normals = normals;

         return mesh;
     }
     
    
    private void AlignPivotWithParent()
    {
        // Get the parent's rotation
        Quaternion parentRotation = transform.parent.rotation;

        // Calculate the offset needed to align the pivot points, considering parent's rotation
        Vector3 pivotOffset = parentRotation * m_initialPivotOffset;

        // Apply the offset to the vertices of the mesh
        Vector3[] vertices = m_mFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] += pivotOffset;
        }
        m_mFilter.mesh.vertices = vertices;

        // Reset the local position of the mesh to align with the parent's pivot point
        m_mFilter.transform.localPosition = m_initialPivotOffset;
    }
}
