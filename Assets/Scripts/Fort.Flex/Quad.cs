using Unity.Collections;
using Unity.Mathematics;

namespace Fort.Flex
{
    public struct Quad
    {
        float3 lt;
        float3 rt;
        float3 rb;
        float3 lb;

        public Quad(float3 lt, float3 rt, float3 rb, float3 lb)
        {
            this.lt = lt;
            this.rt = rt;
            this.rb = rb;
            this.lb = lb;
        }

        /// <summary>
        /// from four corners
        /// </summary>
        public static Quad Create(float2 tl, float2 tr, float2 br, float2 bl, float depth)
        {
            return new Quad(
                new float3(tl, depth), 
                new float3(tr, depth), 
                new float3(br, depth), 
                new float3(bl, depth));
        }

        public void ApplyToArrays(NativeSlice<float3> vertices, NativeSlice<int> indices)
        {
            vertices[0] = lt; //top left
            vertices[1] = rt; //top right
            vertices[2] = rb; //bottom right
            vertices[3] = lb; //bottom left
            
            //Clock-Wise winding (why Unity, why)
            //tri upper left half
            indices[0] = 0;
            indices[1] = 1;
            indices[2] = 3;
        
            //tri bottom right half
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;
        }
    }
}