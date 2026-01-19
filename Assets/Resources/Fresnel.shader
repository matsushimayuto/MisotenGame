Shader "Custom/LitWithFresnel"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _Color ("Base Color", Color) = (1,1,1,1)

        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(1,10)) = 5
        _FresnelIntensity ("Fresnel Intensity", Range(0,5)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            fixed4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                float fresnel = pow(1.0 - saturate(dot(i.worldNormal, i.viewDir)), _FresnelPower);
                fresnel = max(fresnel, 0.3);

                float t = _Time.y * 3.0f;
                float blink = sin(t) * 0.5f + 0.5f;

                baseCol.rgb += _FresnelColor.rgb * fresnel * blink *  _FresnelIntensity;
                return baseCol;
            }
            ENDCG
        }
    }
}
