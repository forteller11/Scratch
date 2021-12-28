using Unity.Mathematics;

namespace Fort.Flex.Positioning
{
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