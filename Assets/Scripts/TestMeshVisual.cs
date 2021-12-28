using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class TestMeshVisual : MonoBehaviour
{
    public Material Material;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Mesh mesh = new Mesh();
        
        float p = .5f;
        float z = 0;
        
        float3 tl = new float3(0, p, z);
        float3 tr = new float3(p, p, z);
        float3 br = new float3(p, 0, z);
        float3 bl = new float3(0, 0, z);

        var vertices = new NativeArray<float3>(4, Allocator.Temp);
        vertices[0] = tl;
        vertices[1] = tr;
        vertices[2] = br;
        vertices[3] = bl;
        
        //todo int3
        var indices = new NativeArray<int>(6, Allocator.Temp);
        indices[0] = 0;
        indices[1] = 1;
        indices[2] = 3;
        
        indices[3] = 1;
        indices[4] = 2;
        indices[5] = 3; 
        
        mesh.SetVertices(vertices, 0, vertices.Length);
        mesh.SetIndices(indices, 0, indices.Length, MeshTopology.Triangles, 0);
        
        Graphics.DrawMesh(mesh, Matrix4x4.identity, Material, 0); 
       
       //todo make own quad mesh now....
       
       //do screen space
    }
}
