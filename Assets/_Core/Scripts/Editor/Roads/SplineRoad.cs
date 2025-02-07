using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using UnityEditor;
using UnityEditor.ProBuilder;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

[ExecuteAlways]
public class SplineRoads : MonoBehaviour
{
    private class VertexGroup
    {
        public List<(Vector3 position, int splineIndex)> Vertices { get; private set; }

        public VertexGroup(List<(Vector3 position, int splineIndex)> vertices)
        {
            Vertices = new List<(Vector3 position, int splineIndex)>(vertices);
        }
    }

    private class UnionFind
    {
        private int[] parent;
        private int[] rank;

        public UnionFind(int size)
        {
            parent = new int[size];
            rank = new int[size];

            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        public int Find(int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]);
            }

            return parent[x];
        }

        public void Union(int x, int y)
        {
            int xRoot = Find(x);
            int yRoot = Find(y);

            if (xRoot == yRoot)
                return;

            if (rank[xRoot] < rank[yRoot])
            {
                parent[xRoot] = yRoot;
            }
            else if (rank[xRoot] > rank[yRoot])
            {
                parent[yRoot] = xRoot;
            }
            else
            {
                parent[yRoot] = xRoot;
                rank[xRoot]++;
            }
        }
    }

    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] [Range(2,100)]int resolution = 10;
    [SerializeField] [Range(0, 5)] private int subdivisions = 2;
    [SerializeField] private float gizmoSize = 0.5f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float connectionRadius = 1.0f;
    [SerializeField] private Material sharedMaterial;
    private bool needsMeshRegeneration = false;

    private List<VertexGroup> vertexGroups = new();

    private List<List<Vector3>> vertsP1List;
    private List<List<Vector3>> vertsP2List;
    private SplineSampler splineSampler;

    private List<GameObject> splineMeshObjects = new List<GameObject>();
    private List<GameObject> crossingMeshObjects = new List<GameObject>();

    private Vector3 p1, p2;

    private void Start()
    {
        splineSampler = GetComponent<SplineSampler>();
        if (splineSampler == null)
        {
            Debug.LogError("SplineSampler component not found!");
            return;
        }

        splineContainer = splineSampler.GetComponent<SplineContainer>();
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer not found!");
            return;
        }

        ClearMeshes();
        GetVerts();
        DetectSplineConnections();

        if (sharedMaterial == null)
        {
            sharedMaterial = new Material(Shader.Find("Standard"));
        }

        GenerateMeshes();
    }

    private void ClearMeshes()
    {
        List<Transform> childrenToRemove = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("SplineMesh_") || child.name.StartsWith("CrossingMesh_"))
            {
                childrenToRemove.Add(child);
            }
        }

        foreach (Transform child in childrenToRemove)
        {
            DestroyImmediate(child.gameObject);
        }

        splineMeshObjects.Clear();
        crossingMeshObjects.Clear();
    }

    private void Update()
    {
        if (needsMeshRegeneration && !Application.isPlaying)
        {
            needsMeshRegeneration = false;
            Start();
            GetVerts();
            DetectSplineConnections();
            GenerateMeshes();
        }
    }

    private void OnValidate()
    {
        needsMeshRegeneration = true;
    }

    private void Awake()
    {
        splineMeshObjects = new List<GameObject>();
        crossingMeshObjects = new List<GameObject>();

        splineSampler = GetComponent<SplineSampler>();
        if (splineSampler == null)
        {
            Debug.LogError("SplineSampler component not found!");
            return;
        }

        splineContainer = splineSampler.GetComponent<SplineContainer>();
        if (splineContainer == null)
        {
            Debug.LogError("SplineContainer not found!");
            return;
        }
    }
    
    private void OnDestroy()
    {
        List<Transform> childrenToRemove = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("SplineMesh_") || child.name.StartsWith("CrossingMesh_"))
            {
                childrenToRemove.Add(child);
            }
        }

        foreach (Transform child in childrenToRemove)
        {
            DestroyImmediate(child.gameObject);
        }
    }
    
    private void GetVerts()
    {
        if (splineSampler == null)
        {
            Debug.LogError("SplineSampler is null in GetVerts");
            return;
        }

        vertsP1List = new List<List<Vector3>>();
        vertsP2List = new List<List<Vector3>>();

        for (int j = 0; j < splineContainer.Splines.Count; j++)
        {
            List<Vector3> vertsP1 = new List<Vector3>();
            List<Vector3> vertsP2 = new List<Vector3>();

            bool isClosed = splineContainer.Splines[j].Closed;
            int pointCount = isClosed ? resolution : resolution + 1;
            float step = 1f / resolution;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i * step;
                splineSampler.SampleSplineWidth(j, t, out p1, out p2);
                vertsP1.Add(p1);
                vertsP2.Add(p2);
            }

            vertsP1List.Add(vertsP1);
            vertsP2List.Add(vertsP2);
        }
    }

    private int GetSubdividePointCount()
    {
        int result = 2;
        for (int i = 0; i < subdivisions; i++)
        {
            result = result * 2 - 1;
        }
        return result;
    }
    
    private void DetectSplineConnections()
    {
        vertexGroups.Clear();

        List<(Vector3 position, int splineIndex, int pointIndex)> allPoints = new List<(Vector3, int, int)>();

        for (int j = 0; j < vertsP1List.Count; j++)
        {
            List<Vector3> vertsP1 = vertsP1List[j];
            List<Vector3> vertsP2 = vertsP2List[j];

            allPoints.Add((vertsP1[0], j, 0));
            allPoints.Add((vertsP2[0], j, 1));
            allPoints.Add((vertsP1[^1], j, 2));
            allPoints.Add((vertsP2[^1], j, 3));
        }

        UnionFind unionFind = new UnionFind(allPoints.Count);

        Dictionary<(Vector3 position, int splineIndex, int pointIndex), int> pointToIndex =
            new Dictionary<(Vector3, int, int), int>();
        for (int i = 0; i < allPoints.Count; i++)
        {
            pointToIndex[allPoints[i]] = i;
        }

        for (int i = 0; i < allPoints.Count; i++)
        {
            var pointA = allPoints[i];
            for (int j = i + 1; j < allPoints.Count; j++)
            {
                var pointB = allPoints[j];
                if (GetDistance(pointA.position, pointB.position) <= connectionRadius)
                {
                    unionFind.Union(i, j);
                }
            }
        }

        Dictionary<int, List<(Vector3, int)>> groups = new Dictionary<int, List<(Vector3, int)>>();

        for (int i = 0; i < allPoints.Count; i++)
        {
            int root = unionFind.Find(i);
            if (!groups.ContainsKey(root))
            {
                groups[root] = new List<(Vector3, int)>();
            }

            var (position, splineIndex, _) = allPoints[i];
            groups[root].Add((position, splineIndex));
        }

        foreach (var group in groups.Values)
        {
            var uniqueSplines = group.Select(p => p.Item2).Distinct().ToList();
            if (uniqueSplines.Count > 1)
            {
                vertexGroups.Add(new VertexGroup(group));
            }
        }
    }
    
    private void GenerateMeshes()
    {
        GenerateSplineMeshes();
        GenerateCrossingsMeshes();
    }

    private void GenerateSplineMeshes()
    {
        int splineCount = splineContainer.Splines.Count;

        while (splineMeshObjects.Count < splineCount)
        {
            GameObject meshObj = new GameObject($"SplineMesh_{splineMeshObjects.Count}");
            meshObj.transform.parent = transform;
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localRotation = Quaternion.identity;
            
            // Přidáme ProBuilderMesh
            ProBuilderMesh pbMesh = meshObj.AddComponent<ProBuilderMesh>();

            // Nastavíme materiál přes MeshRenderer
            MeshRenderer mr = meshObj.GetComponent<MeshRenderer>();
            if(mr == null) mr = meshObj.AddComponent<MeshRenderer>();
            //meshObj.AddComponent<Solidify>();
            mr.sharedMaterial = sharedMaterial;

            splineMeshObjects.Add(meshObj);
        }

        for (int i = splineCount; i < splineMeshObjects.Count; i++)
        {
            splineMeshObjects[i].SetActive(false);
        }

        for (int j = 0; j < splineCount; j++)
        {
            GameObject meshObj = splineMeshObjects[j];
            meshObj.SetActive(true);

            ProBuilderMesh pbMesh = meshObj.GetComponent<ProBuilderMesh>();
            GenerateSplineMeshData(j, pbMesh);
        }
    }
    
    private void GenerateSplineMeshData(int splineIndex, ProBuilderMesh pbMesh)
{
    var vertsP1 = vertsP1List[splineIndex];
    var vertsP2 = vertsP2List[splineIndex];

    int vertexNumberLine = GetSubdividePointCount();
    int triCountPerRow = (vertsP1.Count - 1) * 2 * (vertexNumberLine - 1);

    List<Vector3> allVertices = new List<Vector3>(vertexNumberLine * vertsP1.Count);
    List<Vector2> uvs = new List<Vector2>(vertexNumberLine * vertsP1.Count);
    List<int> triangles = new List<int>(triCountPerRow * 3);

    // Create the grid of vertices
    for (int i = 0; i < vertexNumberLine; i++)
    {
        float t = (float)i / (vertexNumberLine - 1);
        for (int j = 0; j < vertsP1.Count; j++)
        {
            Vector3 interpolated = Vector3.Lerp(vertsP1[j], vertsP2[j], t);
            allVertices.Add(transform.InverseTransformPoint(interpolated));

            Vector2 uv = new Vector2((float)j / (vertsP1.Count - 1), t);
            uvs.Add(uv);
        }
    }

    // Create triangles
    for (int i = 0; i < vertexNumberLine - 1; i++)
    {
        for (int j = 0; j < vertsP1.Count - 1; j++)
        {
            int index0 = i * vertsP1.Count + j;
            int index1 = i * vertsP1.Count + (j + 1);
            int index2 = (i + 1) * vertsP1.Count + j;
            int index3 = (i + 1) * vertsP1.Count + (j + 1);

            // First triangle
            triangles.Add(index0);
            triangles.Add(index2);
            triangles.Add(index1);

            // Second triangle
            triangles.Add(index1);
            triangles.Add(index2);
            triangles.Add(index3);
        }
    }

    // Vyčistíme existující mesh
    pbMesh.Clear();
    
    // Nastavíme vertices a vytvoříme faces
    pbMesh.positions = allVertices;
    
    // Vytvoříme faces s ProBuilder API
    List<Face> faceList = new List<Face>();
    for (int i = 0; i < triangles.Count; i += 3)
    {
        var face = new Face(new int[] { triangles[i], triangles[i + 1], triangles[i + 2] });
        face.smoothingGroup = 1;
        face.manualUV = true; // Explicitně nastavíme, že používáme manuální UV
        faceList.Add(face);
    }
    
    pbMesh.faces = faceList;
    pbMesh.SetUVs(0, uvs.Select(u => new Vector4(u.x, u.y, 0f, 0f)).ToList());
    
    // Aplikujeme změny a aktualizujeme mesh
    pbMesh.Refresh();
    pbMesh.ToMesh();
    pbMesh.Optimize(); // Optimalizujeme mesh pro lepší výkon
    
    // Vytvoříme nebo aktualizujeme collider
    MeshCollider collider = pbMesh.GetComponent<MeshCollider>();
    if (collider == null)
    {
        collider = pbMesh.gameObject.AddComponent<MeshCollider>();
    }
    MeshFilter meshFilter = pbMesh.GetComponent<MeshFilter>();
    collider.sharedMesh = meshFilter.sharedMesh;
    collider.convex = false;
}
    
    private void GenerateCrossingsMeshes()
    {
        int crossingCount = vertexGroups.Count;

        while (crossingMeshObjects.Count < crossingCount)
        {
            GameObject meshObj = new GameObject($"CrossingMesh_{crossingMeshObjects.Count}");
            meshObj.transform.parent = transform;
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localRotation = Quaternion.identity;

            ProBuilderMesh pbMesh = meshObj.AddComponent<ProBuilderMesh>();
            MeshRenderer mr = meshObj.GetComponent<MeshRenderer>();
            if(mr == null) mr = meshObj.AddComponent<MeshRenderer>();
            //meshObj.AddComponent<Solidify>();
            mr.sharedMaterial = sharedMaterial;

            crossingMeshObjects.Add(meshObj);
        }

        for (int i = crossingCount; i < crossingMeshObjects.Count; i++)
        {
            crossingMeshObjects[i].SetActive(false);
        }

        for (int i = 0; i < crossingCount; i++)
        {
            GameObject meshObj = crossingMeshObjects[i];
            meshObj.SetActive(true);

            ProBuilderMesh pbMesh = meshObj.GetComponent<ProBuilderMesh>();
            GenerateCrossingMeshData(i, pbMesh);
        }
    }

    private void GenerateCrossingMeshData(int crossingIndex, ProBuilderMesh pbMesh)
    {
        VertexGroup group = vertexGroups[crossingIndex];
        List<Vector3> positions = group.Vertices.Select(v => v.position).ToList();

        if (positions.Count >= 3)
        {
            Vector3 center = Vector3.zero;
            foreach (var pos in positions)
                center += pos;
            center /= positions.Count;

            // Sort vertices around the center
            positions = positions.OrderByDescending(v => Mathf.Atan2(v.z - center.z, v.x - center.x)).ToList();

            center = transform.InverseTransformPoint(center);
            for (int j = 0; j < positions.Count; j++)
            {
                positions[j] = transform.InverseTransformPoint(positions[j]);
            }

            Vector3[] verts = new Vector3[positions.Count + 1];
            verts[0] = center;
            for (int j = 0; j < positions.Count; j++)
                verts[j + 1] = positions[j];

            // Triangles
            List<int> triangles = new List<int>(positions.Count * 3);
            for (int j = 0; j < positions.Count; j++)
            {
                triangles.Add(0);
                triangles.Add(j + 1);
                triangles.Add(j + 1 == positions.Count ? 1 : j + 2);
            }

            // UVs
            Vector2[] uvs = new Vector2[verts.Length];
            for (int j = 0; j < verts.Length; j++)
            {
                uvs[j] = new Vector2(verts[j].x, verts[j].z);
            }

            pbMesh.Clear();
            pbMesh.positions = verts;
            List<Face> faceList = new List<Face>(triangles.Count / 3);
            for (int i = 0; i < triangles.Count; i += 3)
            {
                faceList.Add(new Face(new int[] { triangles[i], triangles[i + 1], triangles[i + 2] }));
            }

            List<Vector4> uvs4 = uvs.Select(u => new Vector4(u.x, u.y, 0f, 0f)).ToList();
            
            pbMesh.faces = faceList;
            pbMesh.SetUVs(0, uvs4);
            pbMesh.ToMesh();
            pbMesh.Refresh();
        }
    }

    private float GetDistance(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos || vertsP1List == null || vertsP2List == null)
            return;

        DrawSplineEdges();
    }

    private void DrawSplineEdges()
    {
        for (int j = 0; j < vertsP1List.Count; j++)
        {
            List<Vector3> vertsP1 = vertsP1List[j];
            List<Vector3> vertsP2 = vertsP2List[j];

            for (int i = 0; i < vertsP1.Count - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(vertsP1[i], vertsP1[i + 1]);
                Gizmos.DrawLine(vertsP2[i], vertsP2[i + 1]);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(vertsP1[i], gizmoSize * 0.5f);
                Gizmos.DrawSphere(vertsP2[i], gizmoSize * 0.5f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(vertsP1[0], gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP2[0], gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP1[^1], gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP2[^1], gizmoSize * 0.5f);
            Gizmos.color = Color.cyan;

            Gizmos.color = Color.magenta;
            DrawWireCircle(vertsP1[0], connectionRadius);
            DrawWireCircle(vertsP2[0], connectionRadius);
            DrawWireCircle(vertsP1[^1], connectionRadius);
            DrawWireCircle(vertsP2[^1], connectionRadius);
        }
    }

    private void DrawWireCircle(Vector3 worldCenter, float radius)
    {
        int segments = 32;
        float angleStep = 2f * Mathf.PI / segments;

        Vector3 firstOffset = new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);
        firstOffset = transform.TransformDirection(firstOffset);
        Vector3 previousPoint = worldCenter + firstOffset;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            offset = transform.TransformDirection(offset);
            Vector3 nextPoint = worldCenter + offset;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
    
    public void RefreshMesh()
    {
        needsMeshRegeneration = true;
    }
}

[CustomEditor(typeof(SplineRoads))]
public class MyComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        SplineRoads myComponent = (SplineRoads)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Update meshes"))
        {
            myComponent.RefreshMesh();
        }
    }
}
