using System;
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
        
        public StyleValue X;
        public StyleValue Y;
        public StylePair Position => new StylePair(X, Y);
        
        public StyleValue MarginL;
        public StyleValue MarginR;
        public StyleValue MarginT;
        public StyleValue MarginB;
        public StylePair MarginX => new StylePair(MarginL, MarginR);
        public StylePair MarginY => new StylePair(MarginB, MarginT);
        
        public Color Color = new Color(1, 0, 1, 1);

        public Div? Parent;
        public List<Div>? Children;

        public Div(float2 worldScale, StyleValue x, StyleValue y, Color color)
        {
            WorldScale = worldScale;
            X = x;
            Y = y;
            Color = color;
        }

        public Div(float2 worldScale, StyleValue x, StyleValue y, PositioningType positioningType)
        {
            WorldScale = worldScale;
            X = x;
            Y = y;
            Type = positioningType;
        }

        public void WithColor(Color color) => Color = color;

        //bound with margins, render quad
        public (ScreenBound, Quad) GetScreenBoundsBasedOffParent(ScreenBound bounds)
        {
            if (Type == PositioningType.Relative)
            {
                var resultBoundsX = GetPositionFromBounds(X, bounds.BoundsX);
                var resultBoundsY = GetPositionFromBounds(Y, bounds.BoundsY);
                return new ScreenBound(resultBoundsX, resultBoundsY);
            }
            else
            {
                throw new NotImplementedException();
            }
            //todo get positioning working absolute: pixel, percentage
            //todo get relative positioning working... wrapping
        }

        /// <summary>
        /// gets screen bounds along particular position axis
        /// </summary>
        /// <param name="styleValue"></param>
        /// <param name="bounds"> screen bounds of parent</param>
        /// <returns> screen bounds along axis</returns>
        public static float2 GetPositionFromBounds(StyleValue position, StylePair margin, float2 bounds)
        {
            float positionPixel = position.GetValueInPixelCoords(bounds);
            float2 marginsPixel = margin.GetValuesInPixelCoords(bounds);
            
            //todo if scale is scaling... dooo.....
            //todo get scale... wrap?
            
            
            //else... 
            //todo get pixel,
            //todo scale
            //todo get percentage...
        }

        public static float4 ColorToFloat4(Color c) => new float4(c.r, c.g, c.b, c.a);
    }
}