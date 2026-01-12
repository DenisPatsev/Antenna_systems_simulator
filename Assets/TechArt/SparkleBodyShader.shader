Shader "Custom/SparkleBodyShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _SparkleTex("Sparkle texture", 2D) = "black" {}
        _sparkleBrightnessIntensity("BrightnessIntensity", Range(0, 10)) = 1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SparkleTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SparkleTex;
            float3 worldPos;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        float _sparkleBrightnessIntensity;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            float3 viewDir = normalize(UnityWorldSpaceViewDir(IN.worldPos));
            float brightness = 1 - saturate(dot(IN.worldNormal, viewDir));
            float sparkleColor = tex2D(_SparkleTex, IN.uv_SparkleTex) * brightness;
            sparkleColor = lerp(c, sparkleColor, brightness);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission = sparkleColor * _sparkleBrightnessIntensity;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
