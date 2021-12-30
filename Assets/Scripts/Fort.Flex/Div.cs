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

        public StyleValue ScaleWidth;
        public StyleValue ScaleHeight;
        public StylePair Scale => new StylePair(ScaleWidth, ScaleHeight);
        
        // public StyleValue X;
        // public StyleValue Y;
        // public StylePair Position => new StylePair(X, Y);
        
        public StyleValue MarginLeft;
        public StyleValue MarginRight;
        public StyleValue MarginTop;
        public StyleValue MarginBottom;
        public StylePair MarginWidth => new StylePair(MarginLeft, MarginRight);
        public StylePair MarginHeight => new StylePair(MarginBottom, MarginTop);
        
        public Color Color = new Color(1, 0, 1, 1);

        public Div? Parent;
        public List<Div>? Children;

        public Div(float2 worldScale, StyleValue x, StyleValue y, Color color)
        {
            WorldScale = worldScale;
            Color = color;
        }

        public Div(float2 worldScale, StyleValue x, StyleValue y, PositioningType positioningType)
        {
            WorldScale = worldScale;
            Type = positioningType;
        }

        public void WithColor(Color color) => Color = color;

        //bound with margins, render quad
        public (ScreenBound, Quad) GetScreenBoundsBasedOffParent(ScreenBound bounds, ref WrappingInfo info)
        {
            if (Type == PositioningType.Relative)
            {
                var resultBoundsX = GetPositionFromBounds(ScaleWidth, MarginWidth, bounds.BoundsX, ref info);
                var resultBoundsY = GetPositionFromBounds(ScaleHeight, MarginHeight, bounds.BoundsY, ref info);
                
                //todo can fit within wrap?
                //if not............ 
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
        /// returns in screen pixel bounds
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="margin"></param>
        /// <param name="bounds"></param>
        /// <param name="wrappingInfo"></param>
        /// <returns> screne bounds + margin, screen visual</returns>
        public static (float2, float2) GetPositionFromBounds(StyleValue scale, StylePair margin, float2 bounds, ref WrappingInfo wrappingInfo)
        {
            float scalePixel = scale.GetValueInPixelCoords(bounds);
            float2 marginsPixel = margin.GetValuesInPixelCoords(bounds);

            float scaleTotal = marginsPixel.x + scalePixel + marginsPixel.y;
            float leftBoundsMargin = scalePixel - marginsPixel.x;
            float rightBoundsMargin = scalePixel - marginsPixel.y;
            

            //todo REL Position WITHOUT POSITION
            //todo WRAPPING DIR / Behavior when cut off?
            
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