Shader "Custom/PlanarReflection"
{
    Properties
    {
        _MainTex ("ReflectionMask", 2D) = "white" {}
        _ReflectionTex ("Reflection Texture", 2D) = "white" {}
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 1.0
        _Transparency("Transparency", Range(0,1)) = 0
        _Cutoff("Cutoff", Range(0,1)) = 0
        _Tiling("Tiling", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha : fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ReflectionTex;
        float _Smoothness;
        float _Metallic;
        float _ReflectionStrength;
        float _Transparency, _Cutoff;
        float3 _ReflectionCameraPos;
        float4 _Tiling;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ReflectionTex;
            float3 worldPos;
            float4 screenPos;
            float3 worldNormal;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 mask = tex2D(_MainTex, IN.uv_MainTex);

            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            float2 totalUV = float2(screenUV.x * _Tiling.x, screenUV.y * _Tiling.y) + float2(_Tiling.z, _Tiling.w);

            float4 reflection = tex2D(_ReflectionTex, totalUV);
clip(mask.a - _Cutoff);
            lerp(mask.rgb, float4(0,0,0,1), mask.a*_Transparency);
            o.Albedo = mask * reflection * _ReflectionStrength;
            // o.Emission = reflection.rgb * _ReflectionStrength;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness * mask.a * _Transparency;
            o.Alpha = mask.a * _Transparency;
        }
        ENDCG
    }
    FallBack "Diffuse"
}