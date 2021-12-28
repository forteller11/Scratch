using Unity.Mathematics;

namespace Fort.Flex.Positioning
{
    public struct StylePair
    {
        public StyleValue A;
        public StyleValue B;

        public StylePair(StyleValue a, StyleValue b)
        {
            A = a;
            B = b;
        }
        public float2 GetValuesInPixelCoords(float2 pixelBounds)
        {
            return new float2(
                A.GetValueInPixelCoords(pixelBounds),
                B.GetValueInPixelCoords(pixelBounds));
        }
    }
}