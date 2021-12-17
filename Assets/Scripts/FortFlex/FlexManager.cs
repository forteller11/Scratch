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
        public Material Material;
        private const float Z_INCREMENT_BY_LAYER = 0.05f;
        public Div Root;

        private void Start()
        {
            Root = new Div(
                new float2(Screen.width/2f, Screen.height/2f), 
                new float2(500, 40), 
                new Color(0,1,0,1));
        }

        private void Update()
        {
            DrawRecurse(Root, new ScreenBound(Screen.width, Screen.height), 0);
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

                var posStartScreen = ui.LocalPosition + drawBoundsScreen.Value.xw;
                float2 posEndScreen = posStartScreen + ui.WorldScale;
                var boundsScreen = new ScreenBound(posStartScreen, posEndScreen);
                #endregion
                
                #region positioning normalized
                float2 screenMax = new float2(Screen.width, Screen.height);
                var posStartView = posStartScreen / screenMax;
                var posEndView = posEndScreen / screenMax;
                
                #endregion

                #region to cpu side mesh

                float z = layer * Z_INCREMENT_BY_LAYER;
                float3 lb = new float3(posStartView, z);
                float3 rb = new float3(posEndView.x, posStartView.y, z);

                float3 lt = new float3(posStartView.x, posEndView.y, z);
                float3 rt = new float3(posEndView, z);

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

                #region send data to gpu 
                Mesh mesh = new Mesh();
                mesh.SetVertices(vertices, 0, vertices.Length);
                mesh.SetIndices(indices, 0, indices.Length);
                
                Material.SetVector("_BackgroundColor", ui.Color);
                
                Graphics.DrawMesh(mesh, float4x4.identity, Material, 0);

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