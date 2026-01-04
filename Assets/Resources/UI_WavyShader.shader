Shader "Custom/UI_Wavy"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _Strength ("Strength", Range(0,0.1)) = 0.04
        _Margin ("UV Margin", Range(0,0.1)) = 0.05
        _Speed ("Speed", Range(0,5)) = 1
        _TimeOffset ("Time Offset", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Strength;
            float _Margin;
            float _Speed;
            float _TimeOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UVÇà¿ëSóÃàÊÇ…èkÇﬂÇÈ
                float2 baseUV = lerp(_Margin, 1.0 - _Margin, i.uv);

                // ÉmÉCÉYópUVÅiéûä‘â¡éZÅj
                float2 noiseUV = baseUV + _TimeOffset * _Speed;

                float2 noise = tex2D(_NoiseTex, noiseUV).rg - 0.5;

                // å©ÇΩñ⁄ópÇÃóhÇÍ
                float2 uv = baseUV + noise * _Strength;

                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
