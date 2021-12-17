Shader "Unlit/DivShader"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                return o;
            }

            float4 _BackgroundColor;
            fixed4 frag (v2f i) : SV_Target
            {
                // fixed4 col = _BackgroundColor;
                fixed4 col = float4(1,0,0,1);
                return col;
            }
            ENDCG
        }
    }
}
