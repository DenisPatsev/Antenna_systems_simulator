Shader "Custom/FakeLitShader_Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor("Color", Color) = (1,1,1,1)
        _LightDirection("Light Direction", Vector) = (1,1,1,1)
        _LightIntensity("LightIntensity", Range(0,5)) = 1
        _AmbientIntensity("AmbientIntensity", Range(0,1)) = 1
        _LightColor("LightColor", Color) = (1,1,1,1)
        _ShadowStrength("ShadowStrength", Range(0,1)) = 0.5
        _RimPower("RimPower", Int) = 1
        _RimIntensity("RimIntensity", Range(0,10)) = 1
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(1, 1.1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue" = "Geometry"
        }
        ZTest On
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LightDirection, _LightColor, _MainColor;
            float _LightIntensity, _AmbientIntensity, _ShadowStrength, _RimPower, _RimIntensity;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 lightDir = normalize(_LightDirection.xyz);
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;

                float diffuse = max(_AmbientIntensity, dot(normal, lightDir));
                diffuse = lerp(_AmbientIntensity, diffuse, _ShadowStrength);

                float3 lighting = diffuse * _LightColor * _LightIntensity;

                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float rim = 1 - saturate(dot(viewDir, normal));
                rim = pow(rim, _RimPower) * _RimIntensity;

                float3 finalColor = (col.rgb * lighting.rgb + rim * _LightColor.rgb);

                return float4(finalColor, col.a);
            }
            ENDCG
        }

        Pass
        {
            Name "OUTLINE"

            Cull Front
            ZWrite On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;

                v.vertex.xyz *= _OutlineWidth;

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}