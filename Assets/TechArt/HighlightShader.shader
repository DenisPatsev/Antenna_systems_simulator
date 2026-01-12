Shader "Custom/Unlit/HighlightShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor("Main color", Color) = (1,1,1,1)
        _HighlightColor("Highlight color", Color) = (1,1,1,1)
        _ColorChangeSpeed("ColorChangeSpeed", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float3 viewDir : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainColor, _HighlightColor;
            float _HighlightThickness, _HighlightTransparency, _ColorChangeSpeed;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.viewDir = UnityWorldSpaceViewDir(v.vertex);
                o.normal = v.normal;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;

                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);

                float fresnel = 1 - dot(normal, viewDir);
                _HighlightThickness += clamp(_SinTime.w, 0, 1) * _ColorChangeSpeed;
                _HighlightThickness = clamp(_HighlightThickness, 0, 1);
                fresnel = saturate(fresnel) * _HighlightThickness;
                col += fresnel * _HighlightColor;
                return col;
            }
            ENDCG
        }
    }
}