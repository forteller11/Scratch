using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fort.Flex
{
    public class FlexManager : MonoBehaviour
    {
        private const float Z_INCREMENT_BY_LAYER = 0.05f;
        public Div Root;

        private void Start()
        {
            Draw(Root, new ScreenBound(Screen.width, Screen.height), 0);
        }

        public void Draw(Div parent, ScreenBound screenAbsolute, int layer)
        {
            if (parent.Children == null)
                return;
            
            for (int i = 0; i < parent.Children.Count; i++)
            {
                var child = parent.Children[i];
                Mesh.MeshData data;
                NativeArray<float3> meshVertices = new NativeArray<float3>(4, Allocator.Temp);

                #region positioning
                var childOriginWorld = child.LocalPosition + screenAbsolute.Value.xw;
                float2 childEndWorld = childOriginWorld + child.WorldScale;
                var childBoundWorld = new ScreenBound(childOriginWorld, childEndWorld);

                #endregion
                
                #region to cpu side mesh
                float z = layer * Z_INCREMENT_BY_LAYER;
                float3 lb = new float3(childOriginWorld, z);
                float3 rb = new float3(childEndWorld.x, childOriginWorld.y, z);

                float3 lt = new float3(childOriginWorld.x, childEndWorld.y, z);
                float3 rt = new float3(childEndWorld, z);

                NativeArray<float3> vertices = new NativeArray<float3>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                vertices[0] = lb;
                vertices[1] = rb;
                vertices[2] = lt;
                vertices[3] = rt;
                
                NativeArray<int> indices = new NativeArray<int>(6, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                
                // upper left tri
                indices[0] = 0; //lb
                indices[1] = 3; //rt
                indices[2] = 2; //lt
                //bottom right tri 
                indices[3] = 0; //lb
                indices[4] = 1; //rb
                indices[5] = 3; //rt
                #endregion
                
                #region send cpu to side mesh to gpu

                Mesh.MeshData data;
                VertexAttributeDescriptor s;
                s.attribute = VertexAttribute.Position;
                s.dimension = 3;
                s.format = VertexAttributeFormat.Float32;
                Mesh mesh;
                mesh.
                data.SetVertexBufferParams(4, new NativeArray<VertexAttributeDescriptor>());
                SubMeshDescriptor descriptor;
                data.SetSubMesh();
                #endregion
                //use parent to and Position/scalle to get size.
                //convert to normalzied coords
                //convert to mesh
                //draw...... mesh indirect
                meshVertices.Dispose();
                
                
                #region recurse
                Draw(child, childBoundWorld, layer + 1);
                #endregion
            }
        }
        

        public struct ScreenBound
        {
            public float4 Value;
            public float2 BoundsX => Value.xy;
            public float2 BoundsY => Value.zw;

            public ScreenBound(float2 boundsX, float2 boundsY)
            {
                Value = new float4(boundsX, boundsY);
            }
            
            public ScreenBound(float boundsX, float boundsY) 
                : this(new float2(0, boundsX), new float2(0, boundsY))
            {
            }

            public static implicit operator float4 (ScreenBound bounds)
            {
                return bounds.Value;
        }
    }
}