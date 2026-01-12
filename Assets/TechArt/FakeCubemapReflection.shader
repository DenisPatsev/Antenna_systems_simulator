Shader "Custom/FakeCubemapReflection"
{
    Properties
    {
        _ReflectionTex ("Reflection Texture", 2D) = "black" {}
        _MainTex ("Albedo", 2D) = "white" {}
        [HDR] _Color("Main color", Color) = (1,1,1,1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _ReflectionStrength ("Reflection Strength", Range(0, 1)) = 1.0
        _Sphericity ("Sphericity", Range(0, 1)) = 0.5
        _ReflectionTiling ("Reflection Tiling", Float) = 1.0
        _ReflectionOffset ("Reflection Offset", Vector) = (0, 0, 0, 0)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _ReflectionTex;
        sampler2D _MainTex;
        float _Smoothness;
        float _Metallic;
        float _ReflectionStrength;
        float _Sphericity;
        float _ReflectionTiling;
        float4 _ReflectionOffset;
        float4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ReflectionTex;
            float3 worldPos;
            float3 worldRefl;
            float3 worldNormal;
            INTERNAL_DATA
        };

        // Преобразование мировых координат в стабильные UV
        float2 WorldToReflectionUV(float3 worldPos, float3 worldRefl, float sphericity)
        {
            // Используем мировые координаты для стабильности
            float2 uv;
            uv.x = worldPos.x * 0.1 * _ReflectionTiling + _ReflectionOffset.x;
            uv.y = worldPos.z * 0.1 * _ReflectionTiling + _ReflectionOffset.y;
            
            // Добавляем коррекцию на основе направления отражения
            float3 dir = normalize(worldRefl);
            
            if (sphericity > 0.5)
            {
                // Для высокой сферичности - больше влияния направления отражения
                uv.x += dir.x * 0.2 * sphericity;
                uv.y += dir.z * 0.2 * sphericity;
            }
            else
            {
                // Для низкой сферичности - больше влияния позиции в мире
                uv.x += dir.x * 0.05;
                uv.y += dir.z * 0.05;
            }
            
            return uv;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            float3 worldRefl = WorldReflectionVector(IN, o.Normal);
            
            // Стабильные UV на основе мировых координат
            float2 reflectionUV = WorldToReflectionUV(IN.worldPos, worldRefl, _Sphericity);
            
            // Добавляем сферический эффект
            if (_Sphericity > 0)
            {
                float2 center = float2(0.5, 0.5);
                float2 toCenter = reflectionUV - center;
                float dist = length(toCenter);
                
                // Fisheye distortion based on sphericity
                float2 distortedUV = center + toCenter * (1.0 - dist * _Sphericity * 0.3);
                reflectionUV = lerp(reflectionUV, distortedUV, _Sphericity);
            }
            
            fixed4 reflection = tex2D(_ReflectionTex, float2(reflectionUV.x * IN.uv_ReflectionTex.x, reflectionUV.y * IN.uv_ReflectionTex.y));
            
            o.Albedo = albedo.rgb;
            o.Emission = reflection.rgb * _ReflectionStrength;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            o.Alpha = albedo.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}