using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshCreator : MonoBehaviour
{
    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0)
        };

        int[] triangles = {
            0, 2, 1,
            2, 3, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>().sharedMesh = mesh;
    }
}

