using Fort.Flex.Positioning;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Fort.Flex
{
    public partial class FlexManager : MonoBehaviour
    {
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

        ScreenBound CalculateScreenBounds(Div div, ScreenBound parentScreenBounds)
        {
            div
        }
        public ScreenBound Draw(Div ui, ScreenBound parentDrawBounds, int layer)
        {
            #region positioning screen

            ui.GetScreenBoundsBasedOffParent(parentDrawBounds);
            
            var posStartScreen = ui.LocalPosition + parentDrawBounds.Value.xz;
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
            float2 lb = new float2(posStartView);
            float2 rb = new float2(posEndView.x, posStartView.y);
            float2 lt = new float2(posStartView.x, posEndView.y);
            float2 rt = new float2(posEndView);

            NativeArray<float3> vertices = new NativeArray<float3>(4, Allocator.Temp, NativeArrayOptions.ClearMemory);
            NativeArray<int> indices = new NativeArray<int>(6, Allocator.Temp, NativeArrayOptions.ClearMemory);

            Quad quad = Quad.Create(lt, rt, rb, lb, z);
       
            quad.ApplyToArrays(
                vertices.Slice(0, 4), 
                indices. Slice(0, 6));
            #endregion

            #region send data to gpu 
            Mesh mesh = new Mesh();
            mesh.SetVertices(vertices, 0, vertices.Length);
            mesh.SetIndices(indices, 0, indices.Length, MeshTopology.Triangles, 0);
            Material.SetColor("_BackgroundColor", FloatToColor(ui.Color));
            
            Graphics.DrawMesh(mesh, float4x4.identity, Material, 0, Camera.main);

            vertices.Dispose();
            indices.Dispose();
            #endregion

            return boundsScreen;
        }

        public static Color FloatToColor(float4 f)
        {
            return new Color(f.x, f.y, f.x, f.w);
        }
    }
}