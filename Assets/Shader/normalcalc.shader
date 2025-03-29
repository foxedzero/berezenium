Shader "Hidden/normalcalc"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HeightMap("HeightMap", 2D) = "white" {}
        _HeightAmount ("HeightAmount", Range(0,5)) = 0.5

    }
    SubShader
    {
        // No culling or depth
        Cull Off
        ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
        sampler2D _HeightMap;
        float _HeightAmount;

            sampler2D _MainTex;

          half4 calculate(half4 a, half4 b)
            {
                half4 c = normalize(half4(a.y * b.z - a.z*b.y, a.z*b.x-a.x*b.z, a.x*b.y-a.y*b.x, 0));
                c.y *= -1;
                return c;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 a = fixed4(0, tex2Dlod(_HeightMap, float4(i.uv.x, i.uv.y, 0, 0)).r* _HeightAmount, 0, 0);
                fixed4 b = fixed4(0.01, tex2Dlod(_HeightMap, float4(i.uv.x+0.01, i.uv.y, 0, 0)).r * _HeightAmount, 0, 0);
                fixed4 c = fixed4(0, tex2Dlod(_HeightMap, float4(i.uv.x, i.uv.y+0.01, 0, 0)).r* _HeightAmount, 0.01, 0);

                fixed4 d = calculate(b - a, c - a);
                 
                // just invert the colors
                return d;
            }
            ENDCG
        }
    }
}
