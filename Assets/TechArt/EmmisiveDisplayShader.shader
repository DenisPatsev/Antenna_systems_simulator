Shader "Custom/EmmisiveDisplayshader"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        [HDR]_EmissionColor("EmissionColor", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"
            "Queqe"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color, _EmissionColor;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float4 mask = tex2D(_MaskTex, IN.uv_MaskTex) * _EmissionColor;
            c += mask;
            o.Albedo = c;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = _EmissionColor * mask.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
