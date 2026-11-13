Shader "Custom/XRayOcclusion"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _XRayColor ("X-Ray Color", Color) = (0, 1, 1, 0.5)
        _XRayPower ("X-Ray Fresnel Power", Range(0.5, 5.0)) = 2.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        // Pass 1：當物件被牆遮擋時（ZTest Greater）渲染半透明 X-Ray 發光
        Pass
        {
            Name "XRayPass"
            ZTest Greater
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                float3 viewDir : TEXCOORD0;
                float3 normal : TEXCOORD1;
            };

            fixed4 _XRayColor;
            float _XRayPower;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(WorldSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 計算 Fresnel 菲涅耳邊緣發光效果
                float rim = 1.0 - saturate(dot(normalize(i.viewDir), normalize(i.normal)));
                rim = pow(rim, _XRayPower);
                
                fixed4 col = _XRayColor;
                col.a *= rim; // 中央半透明，邊緣較亮
                return col;
            }
            ENDCG
        }

        // Pass 2：當物件未被遮擋時（ZTest LEqual）正常繪製
        Pass
        {
            Name "NormalPass"
            ZTest LEqual
            ZWrite On
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _BaseColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _BaseColor;
                return col;
            }
            ENDCG
        }
    }
}