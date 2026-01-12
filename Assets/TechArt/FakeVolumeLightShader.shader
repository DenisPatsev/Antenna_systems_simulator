Shader "Custom/LightGlow"
{
    Properties
    {
        _MainTex ("Glow Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowIntensity ("Glow Intensity", Range(1, 10)) = 2
        _FadeStart ("Fade Start", Float) = 0
        _FadeEnd ("Fade End", Float) = 1
        _FadeSoftness ("Fade Softness", Range(0, 2)) = 0.2
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "IgnoreProjector"="True"
        }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
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
                float3 worldPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 normal : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowIntensity;
            float _FadeStart;
            float _FadeEnd;
            float _FadeSoftness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(o.worldPos));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 tex = tex2D(_MainTex, i.uv);
                
                float fresnel = 1.0 - abs(dot(i.normal, i.viewDir));
                fresnel = pow(fresnel, 2.0);
                
                fixed4 glow = tex * _GlowColor * _GlowIntensity;
                glow.rgb += fresnel * _GlowColor.rgb * 0.5;
                
                float yPos = i.worldPos.y;
                float fadeRange = _FadeEnd - _FadeStart;
                float fadePos = (yPos - _FadeStart) / fadeRange;
                
                float alpha = smoothstep(0.0, _FadeSoftness, fadePos) * 
                             smoothstep(1.0, 1.0 - _FadeSoftness, fadePos);
                
                glow.a *= alpha;
                
                return glow;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}