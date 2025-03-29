Shader "Custom/WavedTexture"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _Texture("Texture", 2D) = "white" {}
        _Power("Power", Range(-2, 2)) = 0
        _Color("Color", Color) = (1,1,1,1)

    }
    SubShader
    {
        // No culling or depth
        ZWrite Off ZTest LEqual

        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define TAU 6.28318530718

            struct mesh
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct pixel
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            pixel vert (mesh v)
            {
                pixel o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _Texture;
            fixed4 _Color;
            fixed _Power;

            fixed Waves(float2 uv)
            {
                fixed offset = sin(uv.x * 2.16 * TAU) * 0.04;
                fixed waves = cos((uv.y + offset) * 5 * TAU) * 0.5 + 0.5;
                return waves;
            }

            fixed4 frag (pixel i) : SV_Target
            { 
                fixed4 color = tex2D(_Texture, i.uv + Waves(i.uv + _Time) * _Power * sin(_Time)) * _Color;
                color *= i.vertex.y * 0.01;
                return color;
            }
            ENDCG
        }
    }
}
