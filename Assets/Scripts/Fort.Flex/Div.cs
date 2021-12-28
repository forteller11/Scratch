using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Fort.Flex.Positioning;

#nullable enable
namespace Fort.Flex
{
    public class Div
    {
        public float2 WorldScale;
        public PositioningType Type;
        public Position Position;
        public Color Color = new Color(1, 0, 1, 1);

        public Div? Parent;
        public List<Div>? Children;

        public Div(float2 worldScale, Position localPosition, Color color)
        {
            WorldScale = worldScale;
            Position = localPosition;
            Color = color;
        }

        public Div(float2 worldScale, Position localPosition, PositioningType positioningType)
        {
            WorldScale = worldScale;
            Position = localPosition;
            Type = positioningType;
        }

        public void WithColor(Color color) => Color = color;

        public ScreenBound GetScreenBoundsBasedOffParent(ScreenBound bounds)
        {
            
        }

        /// <summary>
        /// gets screen bounds along particular position axis
        /// </summary>
        /// <param name="position"></param>
        /// <param name="bounds"> screen bounds of parent</param>
        /// <returns> screen bounds along axis</returns>
        public static float2 GetPositionFromBounds(Position position, float2 bounds)
        {
            
        }

        public static float4 ColorToFloat4(Color c) => new float4(c.r, c.g, c.b, c.a);
    }
}