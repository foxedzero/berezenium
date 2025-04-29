Shader "Custom/MenuPaused"
{
    Properties
    {
       [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        _AlphaPalette("AlphaPalette", 2D) = "white" {}
        _Direction("Direction", Vector) = (0, -1, 0, 0)
        _ScaleOffset("ScaleOffset", Float) = 1
        _UnscaledTime("UnscaledTime", Float) = 0
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

            Tags {"Queue" = "Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct mesh
                {
                    fixed4 vertex : POSITION;
                    fixed2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct pixel
                {
                    fixed2 uv : TEXCOORD0;
                    fixed4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                };

                pixel vert(mesh v)
                {
                    pixel o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    o.color = v.color;
                    return o;
                }

                sampler2D _MainTex;
                sampler2D _AlphaPalette;
                fixed2 _Direction;
                fixed _UnscaledTime;
                fixed _ScaleOffset;

                fixed4 frag(pixel i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv * _ScaleOffset - _Direction * _UnscaledTime) * i.color;
                col.a *= tex2D(_AlphaPalette, i.uv).a;

                    return col;
                }
                ENDCG
            }
        }
}
