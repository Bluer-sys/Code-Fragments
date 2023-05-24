Shader "Game/Diffuse Blink"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D)             = "white" {}
        _Color ("Color ", color)                = (1,1,1,1)
        _Frequency ("Frequency ", Range(0,1))   = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 150

        CGPROGRAM
        #pragma surface surf Lambert noforwardadd

        sampler2D   _MainTex;
        fixed4      _Color;
        float       _Frequency;

        struct Input
        {
            float2 uv_MainTex;
        };

        float time()
        {
            return sin( _Time.w * _Frequency ) / 2 + 0.5f; // [0;1]
        }
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 blink    = lerp(fixed4(0,0,0,0), _Color, time());
            fixed4 c        = tex2D(_MainTex, IN.uv_MainTex) + blink;
            
            o.Albedo        = c.rgb;
            o.Alpha         = c.a;
        }
        ENDCG
    }

    Fallback "Mobile/VertexLit"
}