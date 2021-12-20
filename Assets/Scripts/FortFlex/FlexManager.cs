using System;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Fort.Flex
{
    public class FlexManager : MonoBehaviour
    {
        public Camera Camera;
        public MeshFilter DebugFilter;
        public Material Material;
        private const float Z_INCREMENT_BY_LAYER = 0.05f;
        public int InitialLayer = 0;
        public Div Root;
        public float2 Size = new float2(500, 500);
        public float2 Offset = new float2(40, 0);

        private void Start()
        {
            Root = new Div(
                Size, 
                Offset,
                new Color(0,1,0,1));
        }

        private void Update()
        {
            Root = new Div(
                Size, 
                Offset,
                new Color(0,1,0,1));
            
            DrawRecurse(Root, new ScreenBound(Screen.width, Screen.height), InitialLayer);
        }

        public void DrawRecurse(Div parent, ScreenBound screenAbsolute, int layer)
        {
            var screenBoundsOfUI = Draw(parent, screenAbsolute, layer);
            
            if (parent.Children == null)
                return;

            for (int i = 0; i < parent.Children.Count; i++)
            {
                var child = parent.Children[i];
                DrawRecurse(child, screenBoundsOfUI, layer + 1);

            }
        }

        public ScreenBound Draw(Div ui, ScreenBound drawBoundsScreen, int layer)
        {

            #region positioning screen

                var posStartScreen = ui.LocalPosition + drawBoundsScreen.Value.xz;
                float2 posEndScreen = posStartScreen + ui.WorldScale;
                var boundsScreen = new ScreenBound(posStartScreen, posEndScreen);
                #endregion
                
                #region positioning normalized
                float2 screenMax = new float2(Screen.width, Screen.height);
                float2 screenMaxHalf =screenMax / 2;
                var posStartView = (posStartScreen) / screenMaxHalf;
                var posEndView = (posEndScreen) / screenMaxHalf;
                #endregion

                #region to cpu side mesh

                float z = layer * Z_INCREMENT_BY_LAYER;
                float3 lb = new float3(posStartView, z);
                float3 rb = new float3(posEndView.x, posStartView.y, z);

                float3 lt = new float3(posStartView.x, posEndView.y, z);
                float3 rt = new float3(posEndView, z);

                NativeArray<float3> vertices = new NativeArray<float3>(4, Allocator.Temp, NativeArrayOptions.ClearMemory);
                vertices[0] = lb;
                vertices[1] = rb;
                vertices[2] = lt;
                vertices[3] = rt;

                NativeArray<int> indices = new NativeArray<int>(6, Allocator.Temp, NativeArrayOptions.ClearMemory);

                //Clock-Wise winding (why Unity, why)
                // upper left tri
                indices[0] = 0; //lb
                indices[1] = 2; //lt
                indices[2] = 3; //rt
                
                //bottom right tri 
                indices[3] = 3; //rt
                indices[4] = 1; //rb
                indices[5] = 0; //lb
                
                // // upper left tri
                // indices[0] = 2; //lb
                // indices[1] = 3; //rt
                // indices[2] = 0; //lt
                // //bottom right tri 
                // indices[3] = 3; //lb
                // indices[4] = 1; //rb
                // indices[5] = 0; //rt

                #endregion

                #region send data to gpu 
                Mesh mesh = new Mesh();
                // mesh.SetVertexBufferData();
                mesh.SetVertices(vertices, 0, vertices.Length);
                mesh.SetIndices(indices, 0, indices.Length, MeshTopology.Triangles, 0);
                mesh.RecalculateBounds();
                Material.SetVector("_BackgroundColor", ui.Color);
                
                //todo SOMETHING IS WRONG HERE..... 
                //todo Bad mesh probably
                Graphics.DrawMesh(mesh, float4x4.identity, Material, 0, Camera);


                vertices.Dispose();
                indices.Dispose();

                // VertexAttributeDescriptor s;
                // s.attribute = VertexAttribute.Position;
                // s.dimension = 3;
                // s.format = VertexAttributeFormat.Float32;
                // data.SetVertexBufferParams(4, new NativeArray<VertexAttributeDescriptor>());
                // SubMeshDescriptor descriptor;
                // data.SetSubMesh();

                #endregion

                //use parent to and Position/scalle to get size.
                //convert to normalzied coords
                //convert to mesh
                //draw...... mesh indirect

                return boundsScreen;
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

            public static implicit operator float4(ScreenBound bounds)
            {
                return bounds.Value;
            }
        }
    }
}