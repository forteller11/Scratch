using System.Collections.Generic;
using Unity.Mathematics;

#nullable enable
namespace Fort.Flex
{
    public class Div
    {
        public float2 WorldScale;
        public float2 LocalPosition;
        public FlexPositioningType PositioningType;

        public Div? Parent;
        public List<Div>? Children;
    }

    public enum FlexPositioningType
    {
        Relative,
        Absolute
    }
}