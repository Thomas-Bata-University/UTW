using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class RecalculateNormalsOutwards : MonoBehaviour
{
    private Mesh mesh;
    private MeshFilter meshFilter;

    void Start()
    {
        Debug.Log("RecalculateNormalsOutwards Start called");
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found!");
            return;
        }

        mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            Debug.LogError("Mesh not found!");
            return;
        }
        RecalculateMeshNormals();
    }

    // Pokud chcete, aby se skript spouštěl při každé změně v editoru
    void OnValidate()
    {
        RecalculateMeshNormals();
    }

    // Pokud chcete, aby se skript spouštěl každým snímkem
    void Update()
    {
        // RecalculateMeshNormals();
    }

    void RecalculateMeshNormals()
    {
        Mesh meshCopy = Instantiate(mesh);
        RecalculateNormals(meshCopy);
        meshFilter.sharedMesh = meshCopy;
    }

    void RecalculateNormals(Mesh mesh)
    {
        int[] triangles = mesh.triangles;
        Debug.Log("Triangles before: " + mesh.triangles.Length);

        Vector3[] vertices = mesh.vertices;

        // Vytvoření edge mapy
        Dictionary<Edge, List<int>> edgeMap = BuildEdgeMap(triangles);

        // Iniciace návštěvnosti trojúhelníků
        bool[] visited = new bool[triangles.Length / 3];
        Queue<int> queue = new Queue<int>();

        // Začínáme s prvním trojúhelníkem
        queue.Enqueue(0);
        visited[0] = true;

        while (queue.Count > 0)
        {
            int currentFaceIndex = queue.Dequeue();
            int triIndex = currentFaceIndex * 3;

            int[] currentFace = new int[]
            {
                triangles[triIndex],
                triangles[triIndex + 1],
                triangles[triIndex + 2]
            };

            for (int i = 0; i < 3; i++)
            {
                // Hrana aktuálního trojúhelníku
                Edge edge = new Edge(currentFace[i], currentFace[(i + 1) % 3]);

                if (edgeMap.TryGetValue(edge, out List<int> adjacentFaces))
                {
                    foreach (int adjTriIndex in adjacentFaces)
                    {
                        int adjFaceIndex = adjTriIndex / 3;
                        if (!visited[adjFaceIndex])
                        {
                            // Získání sousedního trojúhelníku
                            int adjTriStart = adjFaceIndex * 3;
                            int[] adjFace = new int[]
                            {
                                triangles[adjTriStart],
                                triangles[adjTriStart + 1],
                                triangles[adjTriStart + 2]
                            };

                            // Zkontrolujte orientaci a případně invertujte
                            if (!IsConsistent(currentFace, adjFace))
                            {
                                // Invertujte pořadí vrcholů
                                int temp = triangles[adjTriStart];
                                triangles[adjTriStart] = triangles[adjTriStart + 1];
                                triangles[adjTriStart + 1] = temp;
                            }

                            visited[adjFaceIndex] = true;
                            queue.Enqueue(adjFaceIndex);
                        }
                    }
                }
            }
        }

        // Nastavení aktualizovaných trojúhelníků
        mesh.triangles = triangles;

        // Určení směru normál
        if (CalculateMeshVolume(mesh) < 0)
        {
            // Invertujte všechny trojúhelníky
            for (int i = 0; i < triangles.Length; i += 3)
            {
                int temp = triangles[i];
                triangles[i] = triangles[i + 1];
                triangles[i + 1] = temp;
            }
            mesh.triangles = triangles;
        }
        Debug.Log("Triangles after: " + mesh.triangles.Length);

        // Aktualizace normál
        mesh.RecalculateNormals();
    }

    Dictionary<Edge, List<int>> BuildEdgeMap(int[] triangles)
    {
        Dictionary<Edge, List<int>> edgeMap = new Dictionary<Edge, List<int>>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int[] face = new int[]
            {
                triangles[i],
                triangles[i + 1],
                triangles[i + 2]
            };

            for (int j = 0; j < 3; j++)
            {
                Edge edge = new Edge(face[j], face[(j + 1) % 3]);
                Edge reverseEdge = new Edge(face[(j + 1) % 3], face[j]);

                if (!edgeMap.ContainsKey(reverseEdge))
                {
                    // Přidáme hranu do edge mapy
                    if (!edgeMap.ContainsKey(edge))
                    {
                        edgeMap[edge] = new List<int>();
                    }
                    edgeMap[edge].Add(i);
                }
            }
        }
        return edgeMap;
    }

    bool IsConsistent(int[] face1, int[] face2)
    {
        // Najdeme sdílené vrcholy
        int sharedVertices = 0;
        foreach (int v1 in face1)
        {
            foreach (int v2 in face2)
            {
                if (v1 == v2)
                {
                    sharedVertices++;
                }
            }
        }

        // Pokud mají společné 2 vrcholy (sdílejí hranu)
        return sharedVertices != 2;
    }

    float CalculateMeshVolume(Mesh mesh)
    {
        float volume = 0;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = vertices[triangles[i]];
            Vector3 p2 = vertices[triangles[i + 1]];
            Vector3 p3 = vertices[triangles[i + 2]];

            volume += SignedVolumeOfTriangle(p1, p2, p3);
        }
        return volume;
    }

    float SignedVolumeOfTriangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Vector3.Dot(Vector3.Cross(p1, p2), p3) / 6.0f;
    }

    struct Edge
    {
        public int v1;
        public int v2;

        public Edge(int vertex1, int vertex2)
        {
            v1 = Mathf.Min(vertex1, vertex2);
            v2 = Mathf.Max(vertex1, vertex2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Edge))
                return false;

            Edge other = (Edge)obj;
            return v1 == other.v1 && v2 == other.v2;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + v1.GetHashCode();
                hash = hash * 23 + v2.GetHashCode();
                return hash;
            }
        }
    }
}
