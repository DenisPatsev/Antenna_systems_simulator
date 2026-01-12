Shader "Custom/Glass"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _GrabTexture ("Grab Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.96
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpStrength ("Normal Strength", Range(0,1)) = 0.3
        _Refraction ("Refraction", Range(0,100)) = 0.02
        _FresnelPower ("Fresnel Power", Range(0,10)) = 5.0
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
    }
    
    SubShader
    {
        Tags { 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent" 
            "IgnoreProjector" = "True"
        }
        LOD 200
        
        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0
        
        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _GrabTexture;
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;
            float4 screenPos;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _BumpStrength;
        float _Refraction;
        float _FresnelPower;
        fixed4 _FresnelColor;
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            
            float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            normal.xy *= _BumpStrength;
            normal = normalize(normal);
            o.Normal = normal;
            
            float fresnel = pow(1.0 - saturate(dot(normal, normalize(IN.viewDir))), _FresnelPower);
            
            float2 screenUV = (IN.screenPos.xy / IN.screenPos.w);
            screenUV += normal.xy * _Refraction * IN.viewDir;
            fixed4 refraction = tex2D(_GrabTexture, screenUV);
            
            // Комбинирование цветов
            o.Albedo = lerp(refraction.rgb * c.rgb, _FresnelColor.rgb, fresnel * 0.5);
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a * (0.3 + fresnel * 0.7); // Прозрачность зависит от Френеля
        }
        ENDCG
    }
    
    FallBack "Transparent/Diffuse"
}