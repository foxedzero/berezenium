Shader "Custom/SnowDrawer"
{
    Properties
    {
        _DrawBrush("Brush", 2D) = "white" {}
        _DrawPosition("Draw position", Vector) = (-1, -1, 0, 0)
        _Mastab("Mastab", Range(0, 500)) = 1
    }

    SubShader
    {
        Lighting Off
        Blend One Zero
        
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0
            
            sampler2D _DrawBrush;
            float4 _DrawPosition;
            float _DrawAngle;
            float _Mastab;
            
            float4 frag(v2f_customrendertexture IN) : COLOR
            {
               float4 prevColor = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
               
               float2 pos = IN.localTexcoord.xy - _DrawPosition;
               float2x2 rot = float2x2(cos(_DrawAngle), -sin(_DrawAngle), sin(_DrawAngle), cos(_DrawAngle));
               pos = mul(rot, pos);
               pos *= _Mastab; 
               pos += float2(0.5, 0.5);

               float4 drawColor = tex2D(_DrawBrush, pos);
                
                return min(drawColor, prevColor);
            }
            ENDCG
        }
    }
}