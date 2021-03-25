using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//add levels of detail using the non-sub divided meshes

public class WorldChunk : MonoBehaviour {

    private MeshCollider collider;
    private MeshFilter filter;
    private MeshRenderer renderer;

    private Mesh mesh;

    private List<Vector3> vertList;
    private List<int> triList;
    private Color[] colours;

    Dictionary<long, int> vertexIndices;

    private float radius;

    public void SetComponents(Material material, float radius) {
        this.radius = radius;

        vertexIndices = new Dictionary<long, int>();
        renderer = gameObject.AddComponent<MeshRenderer>();
        filter = gameObject.AddComponent<MeshFilter>();
        collider = gameObject.AddComponent<MeshCollider>();

        renderer.sharedMaterial = material;

        mesh = new Mesh();
        mesh.Clear();
        filter.mesh = mesh;
    }

    public void UpdateMesh() {
        mesh.vertices = vertList.ToArray();
        mesh.triangles = triList.ToArray();
        mesh.RecalculateNormals();
        collider.sharedMesh = mesh;
        mesh.colors = colours;
    }

    public void Create(Vector3[] initialVertices, int subdivideIterations) {

        triList = new List<int>();
        vertList = new List<Vector3>();

        vertList.Add(initialVertices[0]);
        vertList.Add(initialVertices[1]);
        vertList.Add(initialVertices[2]);

        List<Triangle> faces = new List<Triangle>();
        faces.Add(new Triangle(0, 1, 2));

        for (int i = 0; i < subdivideIterations; i++) {
            SubdivideMesh(ref faces);
        }

        for (int i = 0; i < faces.Count; i++) {
            triList.Add(faces[i].p1);
            triList.Add(faces[i].p2);
            triList.Add(faces[i].p3);
        }

    }

    void SubdivideMesh(ref List<Triangle> faces) {

        List<Triangle> newFaces = new List<Triangle>();

        foreach (Triangle tri in faces) {
            int a = MakeNewVertexBetweenTwoPoints(tri.p1, tri.p2, ref vertList, radius);
            int b = MakeNewVertexBetweenTwoPoints(tri.p2, tri.p3, ref vertList, radius);
            int c = MakeNewVertexBetweenTwoPoints(tri.p3, tri.p1, ref vertList, radius);

            newFaces.Add(new Triangle(tri.p1, a, c));
            newFaces.Add(new Triangle(tri.p2, b, a));
            newFaces.Add(new Triangle(tri.p3, c, b));
            newFaces.Add(new Triangle(a, b, c));
        }
        faces = newFaces;
    }

    int MakeNewVertexBetweenTwoPoints(int point1Index, int point2Index, ref List<Vector3> vertices, float radius) {
        bool isFirstPointSmaller = point1Index < point2Index;
        long smallerPoint = 0;
        long largerPoint = 0;

        if (isFirstPointSmaller) {
            smallerPoint = point1Index;
            largerPoint = point2Index;
        } else {
            smallerPoint = point2Index;
            largerPoint = point1Index;
        }

        long existingKey = (smallerPoint << 32) + largerPoint;

        int toReturn;
        if (vertexIndices.TryGetValue(existingKey, out toReturn)) {
            return toReturn;
        }

        Vector3 v1 = vertices[point1Index];
        Vector3 v2 = vertices[point2Index];
        Vector3 middle = new Vector3((v1.x + v2.x) / 2f, (v1.y + v2.y) / 2f, (v1.z + v2.z) / 2f);

        toReturn = vertices.Count;

        vertices.Add(middle.normalized * radius);

        vertexIndices.Add(existingKey, toReturn);

        return toReturn;
    }

    public void AddNoise(Vector3 pos, TerrainNoise3D[] noiseComponents, Gradient heightGradient, float minHeight, float maxHeight) {

        colours = new Color[vertList.Count];

        for (int i = 0; i < vertList.Count; i++) {
            Vector3 vertex = vertList[i];

            float multiplier = 0;

            foreach (TerrainNoise3D noise in noiseComponents) {
                float height = noise.GetHeight(pos.x + vertex.x, pos.y + vertex.y, pos.z + vertex.z);
                multiplier += height;
            }

            colours[i] = heightGradient.Evaluate(Mathf.InverseLerp(minHeight, maxHeight, multiplier));

            vertex *= 1 + multiplier;

            vertList[i] = vertex;
        }

    }

    private struct Triangle {
        public int p1;
        public int p2;
        public int p3;

        public Triangle(int p1, int p2, int p3) {
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
        }
    }

}
