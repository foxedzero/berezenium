Shader "Custom/GridXShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("MainTex", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _GridTex("Grid", 2D) = "white" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}

        Cull Off
        ZWrite Off

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct mesh
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct pixel
            {
                float2 uv : TEXCOORD0;
                float2 worldPosition : TEXCOORD1;
                float4 vertex : SV_POSITION;
                fixed4 color : TEXCOORD2;
            };

            sampler2D _GridTex;
            half4 _Color;

            pixel vert (mesh v)
            {
                pixel o;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPosition = mul(unity_ObjectToWorld, v.vertex).xz;

                o.uv = v.uv;
                return o;
            }

            fixed4 frag (pixel i) : SV_Target
            {
                fixed4 col = tex2D(_GridTex, i.worldPosition);
                col.a *= i.color.a;

                return col * _Color;
            }
            ENDCG
        }
    }
}
