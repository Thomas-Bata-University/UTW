using UnityEngine;
using UnityEngine.ProBuilder;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(ProBuilderMesh))]
public class Solidify : MonoBehaviour
{
    [SerializeField]
    private float height = 5f;

    public float Height
    {
        get => height;
        set => height = value;
    }

    public void OnMeshUpdated()
    {
        ProBuilderMesh pb = GetComponent<ProBuilderMesh>();
        if (pb == null) return;

        pb.ToMesh();
        pb.Refresh();

        var originalPositions = pb.positions.ToArray();
        var originalFaces = pb.faces.ToArray();
        var normals = pb.GetNormals();
        
        int count = originalPositions.Length;

        // Vytvoříme novou sadu vrcholů - posunutou o height
        List<Vector3> newPositions = new List<Vector3>(pb.positions);
        for (int i = 0; i < count; i++)
        {
            newPositions.Add(originalPositions[i] + normals[i] * height);
        }

        // Vytvoříme spodní plochy (reverzní plochy z posunutých vrcholů)
        List<Face> newFaces = new List<Face>(originalFaces);
        foreach (var face in originalFaces)
        {
            var inds = face.indexes.ToArray();
            System.Array.Reverse(inds);
            // Přičteme count, protože posunuté vrcholy začínají za původními
            for (int i = 0; i < inds.Length; i++)
            {
                inds[i] += count;
            }

            var backFace = new Face(inds);
            backFace.smoothingGroup = face.smoothingGroup;
            newFaces.Add(backFace);
        }

        // Určíme, které hrany jsou okrajové
        var edgeCount = new Dictionary<Edge, int>();
        foreach (var face in originalFaces)
        {
            var inds = face.indexes;
            for (int i = 0; i < inds.Count; i++)
            {
                int i0 = inds[i];
                int i1 = inds[(i + 1) % inds.Count];
                var e = new Edge(Mathf.Min(i0, i1), Mathf.Max(i0, i1));
                if (!edgeCount.ContainsKey(e))
                    edgeCount[e] = 0;
                edgeCount[e]++;
            }
        }

        // Boundary edges = hrany vyskytující se jen jednou
        var boundaryEdges = edgeCount.Where(kv => kv.Value == 1).Select(kv => kv.Key);

        // Pro každou boundary hranu vytvoříme boční stěny
        foreach (var e in boundaryEdges)
        {
            int v0 = e.a;
            int v1 = e.b;
            int v2 = e.a + count;
            int v3 = e.b + count;

            // Dva trojúhelníky tvořící čtyřúhelníkovou boční stěnu
            // Místo původních (v0, v2, v1) a (v1, v2, v3) použijeme:
            var side1 = new Face(new int[] { v0, v1, v2 });
            var side2 = new Face(new int[] { v1, v3, v2 });


            side1.smoothingGroup = 2;
            side2.smoothingGroup = 2;

            newFaces.Add(side1);
            newFaces.Add(side2);
        }

        pb.Clear();
        pb.positions = newPositions;
        pb.faces = newFaces;
        pb.ToMesh();
        pb.Refresh();
    }
}
