using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using System.Linq;
using System;
using System.Collections;

[ExecuteInEditMode()]
public class SplineRoads : MonoBehaviour
{
    private struct SplineConnection
    {
        public int splineIndexA;
        public int splineIndexB;
        public bool isStartA;
        public bool isStartB;
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3[] vertsA;
        public Vector3[] vertsB;
    }

    private struct SplineEndpoint
    {
        public int splineIndex;
        public bool isStart;
        public Vector3 position;
        public Vector3 vert1;
        public Vector3 vert2;
    }

    public class VertexGroup
    {
        public List<(Vector3 position, int splineIndex)> Vertices { get; private set; }

        public VertexGroup(List<(Vector3 position, int splineIndex)> vertices)
        {
            Vertices = new List<(Vector3 position, int splineIndex)>(vertices);
        }

        // Můžeš přidat další metody nebo vlastnosti dle potřeby, např.:
        public void AddVertex(Vector3 position, int splineIndex)
        {
            Vertices.Add((position, splineIndex));
        }

        public bool ContainsSpline(int splineIndex)
        {
            return Vertices.Any(v => v.splineIndex == splineIndex);
        }
    }


    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] int resolution = 10;
    [SerializeField] private float m_gizmoSize = 0.5f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float connectionRadius = 1.0f;

    private List<SplineConnection> splineConnections = new List<SplineConnection>();
    private List<SplineEndpoint> endpoints = new List<SplineEndpoint>();
    private List<List<SplineEndpoint>> connectionGroups = new List<List<SplineEndpoint>>();
    private List<VertexGroup> vertexGroups = new();


    private List<List<Vector3>> m_vertsP1List;
    private List<List<Vector3>> m_vertsP2List;
    private SplineSampler splineSampler;

    private Vector3 p1, p2;

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

        GetVerts();
        DetectSplineConnections();
        GenerateMeshes();
    }

    private void Update()
    {
        GetVerts();
        DetectSplineConnections();
    }
    
    private void OnValidate()
    {
        GenerateMeshes();
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

    private void CollectEndpoints()
    {
        endpoints = new List<SplineEndpoint>();

        for (int i = 0; i < m_splineContainer.Splines.Count; i++)
        {
            // Get start point
            splineSampler.SampleSplineWidth(i, 0f, out Vector3 startP1, out Vector3 startP2);
            // Don't transform the points - keep them in local space like the vertices
            endpoints.Add(new SplineEndpoint
            {
                splineIndex = i,
                isStart = true,
                position = (startP1 + startP2) / 2f,
                vert1 = startP1,
                vert2 = startP2
            });

            // Get end point
            splineSampler.SampleSplineWidth(i, 1f, out Vector3 endP1, out Vector3 endP2);
            endpoints.Add(new SplineEndpoint
            {
                splineIndex = i,
                isStart = false,
                position = (endP1 + endP2) / 2f,
                vert1 = endP1,
                vert2 = endP2
            });
        }
    }

    private void DetectSplineConnections()
    {
        vertexGroups.Clear();

        // 1. Shromáždit všechny V-body
        List<(Vector3 position, int splineIndex)> allVPoints1 = new();
        List<(Vector3 position, int splineIndex)> allVPoints2 = new();

        List<List<(Vector3 position, int splineIndex)>> allVConnection = new();

        for (int j = 0; j < m_vertsP1List.Count; j++)
        {
            List<Vector3> vertsP1 = m_vertsP1List[j];
            List<Vector3> vertsP2 = m_vertsP2List[j];

            // Přidání krajových V-bodů
            allVPoints1.Add((vertsP1[0], j)); // Start point P1
            allVPoints2.Add((vertsP2[0], j)); // Start point P2
            allVPoints1.Add((vertsP1[^1], j)); // End point P1
            allVPoints2.Add((vertsP2[^1], j)); // End point P2
        }

        List<int> indexes1 = new List<int>();

        for (int j = 0; j < allVPoints1.Count; j++)
        {
            var point1 = allVPoints1[j].position;
            var point2 = allVPoints2[j].position;
            for (int k = 0; k < allVPoints1.Count; k++)
            {
                if (j != k)
                {
                    var inDistance = false;

                    var comparePoint1 = allVPoints1[k].position;
                    var comparePoint2 = allVPoints2[k].position;

                    inDistance |= GetDistance(point1, comparePoint1) <= connectionRadius;
                    inDistance |= GetDistance(point1, comparePoint2) <= connectionRadius;
                    inDistance |= GetDistance(point2, comparePoint1) <= connectionRadius;
                    inDistance |= GetDistance(point2, comparePoint2) <= connectionRadius;

                    if (inDistance)
                    {
                        if (!indexes1.Contains(k))
                        {
                            if (indexes1.Contains(j))
                            {
                                allVConnection[^1].Add((comparePoint1, k));
                                allVConnection[^1].Add((comparePoint2, k));
                            }
                            else
                            {
                                List<(Vector3 position, int splineIndex)> newConnection = new(allVConnection.Count > 0 ? allVConnection[^1] : new List<(Vector3 position, int splineIndex)>());
                                newConnection.Add((point1, j));
                                newConnection.Add((point2, j));
                                newConnection.Add((comparePoint1, k));
                                newConnection.Add((comparePoint2, k));
                                allVConnection.Add(newConnection);
                            }
                            indexes1.Add(k);
                            indexes1.Add(j);
                        }
                    }
                }
            }
        }

        // 2. Zpracování všech nalezených spojení do skupin vrcholů (vertexGroups)
        foreach (var connection in allVConnection)
        {
            HashSet<int> uniqueSplines = new HashSet<int>();
            foreach (var (position, splineIndex) in connection)
            {
                uniqueSplines.Add(splineIndex);
            }

            if (uniqueSplines.Count > 1) // Pokud skupina obsahuje body z více spline, jedná se o propojení
            {
                vertexGroups.Add(new SplineRoads.VertexGroup(connection));
            }
        }

        // Debug: Vypsat informace o nalezených propojeních
        Debug.Log($"Celkový počet nalezených skupin propojených vrcholů: {vertexGroups.Count}");
        foreach (var group in vertexGroups)
        {
            string groupInfo = "Skupina vrcholů: ";
            foreach (var vertex in group.Vertices)
            {
                groupInfo += $"(Pozice: {vertex.position}, SplineIndex: {vertex.splineIndex}) ";
            }

            Debug.Log(groupInfo);
        }
        
}

private void GenerateMeshes()
{
    // Vymazání předchozích mesh objektů
    for (int i = transform.childCount - 1; i >= 0; i--)
    {
        DestroyImmediate(transform.GetChild(i).gameObject);
    }

    // Mesh pro každou spline
    for (int j = 0; j < m_vertsP1List.Count; j++)
    {
        List<Vector3> vertsP1 = m_vertsP1List[j];
        List<Vector3> vertsP2 = m_vertsP2List[j];

        Mesh splineMesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Generuj vertexy a trojúhelníky pro spline
        for (int i = 0; i < vertsP1.Count; i++)
        {
            vertices.Add(vertsP1[i]);
            vertices.Add(vertsP2[i]);
        }
        for (int i = 0; i < vertsP1.Count - 1; i++)
        {
            int vertIndex = i * 2;
            triangles.Add(vertIndex);
            triangles.Add(vertIndex + 2);
            triangles.Add(vertIndex + 1);

            triangles.Add(vertIndex + 1);
            triangles.Add(vertIndex + 2);
            triangles.Add(vertIndex + 3);
        }

        splineMesh.vertices = vertices.ToArray();
        splineMesh.triangles = triangles.ToArray();
        splineMesh.RecalculateNormals();

        // Přidání mesh do scény jako potomek aktuálního objektu
        GameObject splineObject = new GameObject($"SplineMesh_{j}");
        splineObject.transform.parent = transform;
        MeshFilter meshFilter = splineObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = splineObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = splineMesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }

    // Mesh pro každou křižovatku
    foreach (var group in vertexGroups)
    {
        Mesh intersectionMesh = new Mesh();
        List<Vector3> intersectionVertices = group.Vertices.Select(v => v.position).ToList();
        List<int> intersectionTriangles = new List<int>();

        // Jednoduché triangulování křižovatky (předpokládáme, že body leží v rovině)
        for (int i = 1; i < intersectionVertices.Count - 1; i++)
        {
            intersectionTriangles.Add(0);
            intersectionTriangles.Add(i + 1);
            intersectionTriangles.Add(i);
        }

        intersectionMesh.vertices = intersectionVertices.ToArray();
        intersectionMesh.triangles = intersectionTriangles.ToArray();
        intersectionMesh.RecalculateNormals();

        // Přidání mesh do scény jako potomek aktuálního objektu
        GameObject intersectionObject = new GameObject($"IntersectionMesh_{group.Vertices.First().splineIndex}");
        intersectionObject.transform.parent = transform;
        MeshFilter meshFilter = intersectionObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = intersectionObject.AddComponent<MeshRenderer>();
        meshFilter.mesh = intersectionMesh;
        meshRenderer.material = new Material(Shader.Find("Standard"));
    }
}

private IEnumerator GenerateMeshesCoroutine()
{
    yield return null; // Wait for the end of frame to avoid errors during physics or animation events
    StartCoroutine(GenerateMeshesCoroutine());
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
                var closestPoints = sameSplinePoints.Where(v => v.splineIndex == vertex.splineIndex && v.position != vertex.position)
                    .OrderBy(v => GetDistance(vertex.position, v.position))
                    .Take(2) // Každý bod max dvě linky
                    .ToList();

                foreach (var closest in closestPoints)
                {
                    if (!drawnLines.Contains((closest.position, vertex.position)) && !drawnLines.Contains((vertex.position, closest.position)))
                    {
                        Debug.DrawLine(vertex.position, closest.position, Color.green);
                        drawnLines.Add((vertex.position, closest.position));
                    }
                }
            }
        }
        
        HashSet<(Vector3, Vector3)> drawnCrossSplineLines = new HashSet<(Vector3, Vector3)>();
        List<(Vector3 position, int splineIndex)> unconnectedPoints = new List<(Vector3 position, int splineIndex)>(vertexGroups.SelectMany(group => group.Vertices));

        while (unconnectedPoints.Count > 1)
        {
            // Najdi nejkratší možné propojení mezi body s různými spline indexy a zajisti, že nebudou propojeny body již v jiné skupině
            var possiblePair = unconnectedPoints.SelectMany((point1, index1) =>
                    unconnectedPoints.Skip(index1 + 1)
                        .Where(point2 => point1.splineIndex != point2.splineIndex &&
                                         !drawnCrossSplineLines.Any(line => line.Item1 == point1.position || line.Item2 == point1.position ||
                                                                            line.Item1 == point2.position || line.Item2 == point2.position))
                        .Select(point2 => (point1, point2, distance: GetDistance(point1.position, point2.position))))
                .OrderBy(pair => pair.distance)
                .FirstOrDefault();

            if (possiblePair != default)
            {
                // Vykresli linku mezi body a smaž je z kopie struktury
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
        // Calculate points in local space relative to the world center
        int segments = 32;
        float angleStep = 2f * Mathf.PI / segments;

        // Calculate the first point in world space
        Vector3 firstOffset = new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);
        // Transform the offset to match the object's rotation
        firstOffset = transform.TransformDirection(firstOffset);
        Vector3 previousPoint = worldCenter + firstOffset;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            // Calculate offset in local space
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            // Transform the offset direction to match object's rotation
            offset = transform.TransformDirection(offset);
            // Add the transformed offset to the world center
            Vector3 nextPoint = worldCenter + offset;

            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }
    }
}