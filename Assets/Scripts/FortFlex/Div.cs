using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

#nullable enable
namespace Fort.Flex
{
    public class Div
    {
        public float2 WorldScale;
        public float2 LocalPosition;
        public FlexPositioningType PositioningType;
        public float4 Color = new float4(1, 0, 1, 1);

        public Div? Parent;
        public List<Div>? Children;

        public Div(float2 worldScale, float2 localPosition, Color color)
        {
            WorldScale = worldScale;
            LocalPosition = localPosition;
            Color = ColorToFloat4(color);
        }

        public static float4 ColorToFloat4(Color c) => new float4(c.r, c.g, c.b, c.a);
    }

    public enum FlexPositioningType
    {
        Relative,
        Absolute
    }
}