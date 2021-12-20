using System;
using Unity.Mathematics;
using UnityEngine;

namespace Fort.Flex
{
    public class TestMesh : MonoBehaviour
    {
        public Mesh Mesh;
        public Material Material;
        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {

            Graphics.DrawMesh(Mesh, float4x4.identity, Material, 0);
        }
    }
}