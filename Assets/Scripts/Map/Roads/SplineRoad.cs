using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;

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

        public void AddVertex(Vector3 position, int splineIndex)
        {
            Vertices.Add((position, splineIndex));
        }

        public bool ContainsSpline(int splineIndex)
        {
            return Vertices.Any(v => v.splineIndex == splineIndex);
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

            // Initially, each node is its own parent (self-rooted tree)
            for (int i = 0; i < size; i++)
            {
                parent[i] = i;
                rank[i] = 0;
            }
        }

        // Find the root parent of a node with path compression
        public int Find(int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]);
            }
            return parent[x];
        }

        // Union two sets by rank
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



    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] int resolution = 10;
    [SerializeField] private float m_gizmoSize = 0.5f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float connectionRadius = 1.0f;
    [SerializeField] private Material sharedMaterial;

    private List<VertexGroup> vertexGroups = new();


    private List<List<Vector3>> m_vertsP1List;
    private List<List<Vector3>> m_vertsP2List;
    private SplineSampler splineSampler;
    
    private List<GameObject> splineMeshObjects = new List<GameObject>();
    private List<GameObject> crossingMeshObjects = new List<GameObject>();


    private Vector3 p1, p2;
    
    [SerializeField]private bool needsMeshRegeneration = false;


    private void Start()
    {
        splineSampler = GetComponent<SplineSampler>();
        if (splineSampler == null)
        {
            Debug.LogError("SplineSampler component not found!");
            return;
        }

        m_splineContainer = splineSampler.GetComponent<SplineContainer>();
        if (m_splineContainer == null)
        {
            Debug.LogError("SplineContainer not found!");
            return;
        }

        ClearMeshes(); // Přidáno vyčištění meshů
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
        // Destroy all child GameObjects
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

        // Clear the lists
        splineMeshObjects.Clear();
        crossingMeshObjects.Clear();
    }

    private void Update()
    {
        if (needsMeshRegeneration && !Application.isPlaying)
        {
            needsMeshRegeneration = false;
            ClearMeshes(); // Přidáno vyčištění meshů
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

        m_splineContainer = splineSampler.GetComponent<SplineContainer>();
        if (m_splineContainer == null)
        {
            Debug.LogError("SplineContainer not found!");
            return;
        }
    }



    private void OnDestroy()
    {
        // Destroy all child GameObjects
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

        m_vertsP1List = new List<List<Vector3>>();
        m_vertsP2List = new List<List<Vector3>>();

        for (int j = 0; j < m_splineContainer.Splines.Count; j++)
        {
            List<Vector3> vertsP1 = new List<Vector3>();
            List<Vector3> vertsP2 = new List<Vector3>();

            bool isClosed = m_splineContainer.Splines[j].Closed;
            int pointCount = isClosed ? resolution : resolution + 1;
            float step = 1f / resolution;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i * step;
                splineSampler.SampleSplineWidth(j, t, out p1, out p2);
                vertsP1.Add(p1);
                vertsP2.Add(p2);
            }

            m_vertsP1List.Add(vertsP1);
            m_vertsP2List.Add(vertsP2);
        }
    }

    private void DetectSplineConnections()
{
    vertexGroups.Clear();

    // Step 1: Collect all endpoint positions with unique identifiers
    List<(Vector3 position, int splineIndex, int pointIndex)> allPoints = new List<(Vector3, int, int)>();

    for (int j = 0; j < m_vertsP1List.Count; j++)
    {
        List<Vector3> vertsP1 = m_vertsP1List[j];
        List<Vector3> vertsP2 = m_vertsP2List[j];

        // Add start and end points of each spline edge
        allPoints.Add((vertsP1[0], j, 0)); // Start point P1
        allPoints.Add((vertsP2[0], j, 1)); // Start point P2
        allPoints.Add((vertsP1[^1], j, 2)); // End point P1
        allPoints.Add((vertsP2[^1], j, 3)); // End point P2
    }

    // Step 2: Initialize Union-Find structure
    UnionFind unionFind = new UnionFind(allPoints.Count);

    // Step 3: Map to quickly find indices
    Dictionary<(Vector3 position, int splineIndex, int pointIndex), int> pointToIndex = new Dictionary<(Vector3, int, int), int>();
    for (int i = 0; i < allPoints.Count; i++)
    {
        pointToIndex[allPoints[i]] = i;
    }

    // Step 4: Union connected points
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

    // Step 5: Group points by root parent
    Dictionary<int, List<(Vector3 position, int splineIndex)>> groups = new Dictionary<int, List<(Vector3, int)>>();

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

    // Step 6: Create vertex groups from the connected components
    foreach (var group in groups.Values)
    {
        // Only consider groups that have points from more than one spline
        var uniqueSplines = group.Select(p => p.splineIndex).Distinct().ToList();
        if (uniqueSplines.Count > 1)
        {
            vertexGroups.Add(new VertexGroup(group));
        }
    }

    // Debug output
    Debug.Log($"Total number of connected vertex groups: {vertexGroups.Count}");
    foreach (var group in vertexGroups)
    {
        string groupInfo = "Vertex Group: ";
        foreach (var vertex in group.Vertices)
        {
            groupInfo += $"(Position: {vertex.position}, SplineIndex: {vertex.splineIndex}) ";
        }
        Debug.Log(groupInfo);
    }
}


    private void GenerateMeshes()
    {
        GenerateSplineMeshes();
        GenerateCrossingsMeshes();
    }

    private void GenerateSplineMeshes()
    {
        int splineCount = m_splineContainer.Splines.Count;

        // Ensure the list has enough GameObjects
        while (splineMeshObjects.Count < splineCount)
        {
            // Create new mesh object
            GameObject meshObj = new GameObject($"SplineMesh_{splineMeshObjects.Count}");
            meshObj.transform.parent = transform;
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localRotation = Quaternion.identity;
            meshObj.AddComponent<MeshFilter>();
            meshObj.AddComponent<MeshRenderer>();
            splineMeshObjects.Add(meshObj);
        }

        // Hide extra mesh objects
        for (int i = splineCount; i < splineMeshObjects.Count; i++)
        {
            splineMeshObjects[i].SetActive(false);
        }

        // Update or create mesh objects
        for (int j = 0; j < splineCount; j++)
        {
            GameObject meshObj = splineMeshObjects[j];
            meshObj.SetActive(true);

            MeshFilter meshFilter = meshObj.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObj.GetComponent<MeshRenderer>();

            // Assign a material if not already assigned
            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = sharedMaterial ?? new Material(Shader.Find("Standard"));
            }

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            // Generate mesh data
            GenerateSplineMeshData(j, mesh);
        }
    }


    private void GenerateSplineMeshData(int splineIndex, Mesh mesh)
    {
        List<Vector3> vertsP1 = m_vertsP1List[splineIndex];
        List<Vector3> vertsP2 = m_vertsP2List[splineIndex];

        int segmentCount = vertsP1.Count - 1;
        int vertexCount = vertsP1.Count * 2;
        int triangleCount = segmentCount * 2 * 3;

        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[triangleCount];
        Vector2[] uvs = new Vector2[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];

        // Build vertices, uvs, and normals
        for (int i = 0; i < vertsP1.Count; i++)
        {
            int idx = i * 2;
            vertices[idx] = transform.InverseTransformPoint(vertsP1[i]);
            vertices[idx + 1] = transform.InverseTransformPoint(vertsP2[i]);

            // UV mapping
            float v = (float)i / segmentCount;
            uvs[idx] = new Vector2(0, v);
            uvs[idx + 1] = new Vector2(1, v);

            normals[idx] = Vector3.up;
            normals[idx + 1] = Vector3.up;
        }

        // Build triangles
        int trisIndex = 0;
        for (int i = 0; i < segmentCount; i++)
        {
            int idx = i * 2;

            // First triangle
            triangles[trisIndex++] = idx;
            triangles[trisIndex++] = idx + 2;
            triangles[trisIndex++] = idx + 1;

            // Second triangle
            triangles[trisIndex++] = idx + 1;
            triangles[trisIndex++] = idx + 2;
            triangles[trisIndex++] = idx + 3;
        }

        // Assign mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }
    private void GenerateCrossingsMeshes()
    {
        int crossingCount = vertexGroups.Count;

        // Ensure the list has enough GameObjects
        while (crossingMeshObjects.Count < crossingCount)
        {
            // Create new mesh object
            GameObject meshObj = new GameObject($"CrossingMesh_{crossingMeshObjects.Count}");
            meshObj.transform.parent = transform;
            meshObj.transform.localPosition = Vector3.zero;
            meshObj.transform.localRotation = Quaternion.identity;
            meshObj.AddComponent<MeshFilter>();
            meshObj.AddComponent<MeshRenderer>();
            crossingMeshObjects.Add(meshObj);
        }

        // Hide extra mesh objects
        for (int i = crossingCount; i < crossingMeshObjects.Count; i++)
        {
            crossingMeshObjects[i].SetActive(false);
        }

        // Update or create mesh objects
        for (int i = 0; i < crossingCount; i++)
        {
            GameObject meshObj = crossingMeshObjects[i];
            meshObj.SetActive(true);

            MeshFilter meshFilter = meshObj.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObj.GetComponent<MeshRenderer>();

            // Assign material
            if (meshRenderer.sharedMaterial == null)
            {
                meshRenderer.sharedMaterial = sharedMaterial ?? new Material(Shader.Find("Standard"));
            }

            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null)
            {
                mesh = new Mesh();
                meshFilter.sharedMesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            // Generate mesh data
            GenerateCrossingMeshData(i, mesh);
        }
    }


   private void GenerateCrossingMeshData(int crossingIndex, Mesh mesh)
{
    VertexGroup group = vertexGroups[crossingIndex];
    List<Vector3> positions = group.Vertices.Select(v => v.position).ToList();

    if (positions.Count >= 3)
    {
        // Calculate center point
        Vector3 center = Vector3.zero;
        foreach (var pos in positions)
        {
            center += pos;
        }
        center /= positions.Count;

        // Sort vertices around the center point in reverse order
        positions = positions.OrderByDescending(v => Mathf.Atan2(v.z - center.z, v.x - center.x)).ToList();

        // Transform positions to local space
        center = transform.InverseTransformPoint(center);
        for (int j = 0; j < positions.Count; j++)
        {
            positions[j] = transform.InverseTransformPoint(positions[j]);
        }

        // Build vertices
        Vector3[] vertices = new Vector3[positions.Count + 1]; // +1 for center
        vertices[0] = center; // Center vertex
        for (int j = 0; j < positions.Count; j++)
        {
            vertices[j + 1] = positions[j];
        }

        // Build triangles with correct winding order
        int[] triangles = new int[positions.Count * 3];
        for (int j = 0; j < positions.Count; j++)
        {
            triangles[j * 3] = 0; // Center vertex index
            triangles[j * 3 + 1] = j + 1;
            triangles[j * 3 + 2] = j + 1 == positions.Count ? 1 : j + 2;
        }

        // Build UVs and normals
        Vector2[] uvs = new Vector2[vertices.Length];
        Vector3[] normals = new Vector3[vertices.Length];
        for (int j = 0; j < vertices.Length; j++)
        {
            // Simple planar mapping; adjust as needed
            uvs[j] = new Vector2(vertices[j].x, vertices[j].z);
            normals[j] = Vector3.up;
        }

        // Assign mesh data
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = normals;
        mesh.RecalculateBounds();
    }
}


    private float GetDistance(Vector3 pointA, Vector3 pointB)
    {
        return Vector3.Distance(pointA, pointB);
    }


    private void OnDrawGizmos()
    {
        if (!showGizmos || m_vertsP1List == null || m_vertsP2List == null)
            return;

        DrawSplineEdges();

        foreach (var group in vertexGroups)
        {
            List<(Vector3 position, int splineIndex)> sameSplinePoints = group.Vertices.GroupBy(v => v.splineIndex)
                .Where(g => g.Count() > 1)
                .SelectMany(g => g)
                .ToList();
            HashSet<(Vector3, Vector3)> drawnLines = new HashSet<(Vector3, Vector3)>();

            foreach (var vertex in sameSplinePoints)
            {
                var closestPoints = sameSplinePoints
                    .Where(v => v.splineIndex == vertex.splineIndex && v.position != vertex.position)
                    .OrderBy(v => GetDistance(vertex.position, v.position))
                    .Take(2) // Každý bod max dvě linky
                    .ToList();

                foreach (var closest in closestPoints)
                {
                    if (!drawnLines.Contains((closest.position, vertex.position)) &&
                        !drawnLines.Contains((vertex.position, closest.position)))
                    {
                        Debug.DrawLine(vertex.position, closest.position, Color.green);
                        drawnLines.Add((vertex.position, closest.position));
                    }
                }
            }
        }

        HashSet<(Vector3, Vector3)> drawnCrossSplineLines = new HashSet<(Vector3, Vector3)>();
        List<(Vector3 position, int splineIndex)> unconnectedPoints =
            new List<(Vector3 position, int splineIndex)>(vertexGroups.SelectMany(group => group.Vertices));

        while (unconnectedPoints.Count > 1)
        {
            var possiblePair = unconnectedPoints.SelectMany((point1, index1) =>
                    unconnectedPoints.Skip(index1 + 1)
                        .Where(point2 => point1.splineIndex != point2.splineIndex &&
                                         !drawnCrossSplineLines.Any(line =>
                                             line.Item1 == point1.position || line.Item2 == point1.position ||
                                             line.Item1 == point2.position || line.Item2 == point2.position))
                        .Select(point2 => (point1, point2, distance: GetDistance(point1.position, point2.position))))
                .OrderBy(pair => pair.distance)
                .FirstOrDefault();

            if (possiblePair != default)
            {
                Debug.DrawLine(possiblePair.point1.position, possiblePair.point2.position, Color.blue);
                drawnCrossSplineLines.Add((possiblePair.point1.position, possiblePair.point2.position));

                unconnectedPoints.Remove(possiblePair.point1);
                unconnectedPoints.Remove(possiblePair.point2);
            }
            else
            {
                break;
            }
        }
    }

    private void DrawSplineEdges()
    {
        for (int j = 0; j < m_vertsP1List.Count; j++)
        {
            List<Vector3> vertsP1 = m_vertsP1List[j];
            List<Vector3> vertsP2 = m_vertsP2List[j];

            for (int i = 0; i < vertsP1.Count - 1; i++)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(vertsP1[i], vertsP1[i + 1]);
                Gizmos.DrawLine(vertsP2[i], vertsP2[i + 1]);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(vertsP1[i], m_gizmoSize * 0.5f);
                Gizmos.DrawSphere(vertsP2[i], m_gizmoSize * 0.5f);
            }

            // Draw edge points
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(vertsP1[0], m_gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP2[0], m_gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP1[^1], m_gizmoSize * 0.5f);
            Gizmos.DrawSphere(vertsP2[^1], m_gizmoSize * 0.5f);
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
}