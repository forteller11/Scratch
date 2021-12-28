using System.Collections;
using System.Collections.Generic;
using Fort.Flex;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class TestMeshVisual : MonoBehaviour
{
    public Material Material;

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
        var indices = new NativeArray<int>(6, Allocator.Temp);
        
        var quad = new Quad(tl, tr, br, bl);
        quad.ApplyToArrays(
            vertices.Slice(0, 4), 
            indices. Slice(0, 6));
        
        mesh.SetVertices(vertices, 0, vertices.Length);
        mesh.SetIndices(indices, 0, indices.Length, MeshTopology.Triangles, 0);
        
        Material.SetColor("_BackgroundColor", Color.red);
        
        Graphics.DrawMesh(mesh, Matrix4x4.identity, Material, 0); 
    }
}
