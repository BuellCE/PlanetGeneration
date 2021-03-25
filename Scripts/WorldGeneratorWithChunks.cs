using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[CustomEditor(typeof(WorldGeneratorWithChunks))]
public class WorldGeneratorWithChunks : MonoBehaviour {

    public int subdivideIterations = 4;
    public float radius = 25f;
    public TerrainNoise3D[] noiseComponents;

    public Gradient heightGradient;

    List<WorldChunk> chunks;

    [ContextMenu("Spawn Planet")]
    public void Create() {

        for (int i = 0; i < transform.childCount; i++) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        chunks = new List<WorldChunk>();
        List<Vector3> vertList = new List<Vector3>();
        List<Triangle> faces = new List<Triangle>();

        //create the initial 20-sided icosahedron
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
        vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
        vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

        vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
        vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
        vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

        vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
        vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);

        faces.Add(new Triangle(0, 1, 7));
        faces.Add(new Triangle(0, 5, 1));
        faces.Add(new Triangle(0, 7, 10));
        faces.Add(new Triangle(0, 10, 11));
        faces.Add(new Triangle(0, 11, 5));
        faces.Add(new Triangle(1, 5, 9));
        faces.Add(new Triangle(3, 9, 4));
        faces.Add(new Triangle(3, 4, 2));
        faces.Add(new Triangle(3, 2, 6));
        faces.Add(new Triangle(3, 6, 8));
        faces.Add(new Triangle(3, 8, 9));
        faces.Add(new Triangle(5, 11, 4));
        faces.Add(new Triangle(11, 10, 2));
        faces.Add(new Triangle(10, 7, 6));
        faces.Add(new Triangle(7, 1, 8));
        faces.Add(new Triangle(4, 9, 5));
        faces.Add(new Triangle(2, 4, 11));
        faces.Add(new Triangle(6, 2, 10));
        faces.Add(new Triangle(8, 6, 7));
        faces.Add(new Triangle(9, 8, 1));

        foreach (Triangle triangle in faces) {
            GameObject chunk = new GameObject("Chunk");
            chunk.tag = "Planet";
            chunk.transform.SetParent(this.transform);
            chunk.transform.localPosition = new Vector3(0,0,0);
            WorldChunk worldChunk = chunk.AddComponent<WorldChunk>();
            worldChunk.SetComponents(this.GetComponent<MeshRenderer>().sharedMaterial, radius);

            Vector3[] initialVertices = new Vector3[] { 
                vertList[triangle.p1], 
                vertList[triangle.p2], 
                vertList[triangle.p3] 
            };

            worldChunk.Create(initialVertices, subdivideIterations);

            float height = 0;
            foreach (TerrainNoise3D noise in noiseComponents) {
                height += noise.heightMultiplier;
            }

            worldChunk.AddNoise(this.transform.position, noiseComponents, heightGradient, 0, height);
            worldChunk.UpdateMesh();

            chunks.Add(worldChunk);
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