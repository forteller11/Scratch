using System;
using Unity.Mathematics;
using UnityEngine;

namespace Fort.Flex.Positioning
{
    public struct StyleValue
    {
        public float Value;
        public PositioningUnit Unit;

        public float GetValueInPixelCoords(float2 pixelBounds)
        {
            if (Unit == PositioningUnit.Pixel)
            {
                return Value;
            }

            if (Unit == PositioningUnit.Peruniage)
            {
                //could be negative, this might be a problem or a good thing?
                //perhaps abs() it in future?
               float rangeBetweenComponents = pixelBounds.y - pixelBounds.x;
               return rangeBetweenComponents * Value;
            }

            throw new NotImplementedException();
        }
    }
}