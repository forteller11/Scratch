using Unity.Mathematics;

namespace Fort.Flex.Positioning
{
    public struct WrappingInfo
    {
        public FlexDirection Direction;
        public float2 CurrentOrigin;
    }
    public enum FlexDirection
    {
        Row,
        RowReverse,
        Column,
        ColumnReverse
    }

    public static class FlexTypeHelpers
    {
        // public enum int2 direction{}
    }
}