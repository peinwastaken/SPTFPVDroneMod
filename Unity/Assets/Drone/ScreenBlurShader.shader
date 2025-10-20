Shader "FPVDrone/ScreenBlurShader"
{
    Properties
    {
        _MainTex ("Source Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
        _Horizontal ("Horizontal Pass", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize; // x = 1/width, y = 1/height
            float _BlurSize;
            float _Horizontal;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 ts = _MainTex_TexelSize.xy * _BlurSize;

                // Choose direction
                float2 offset = (_Horizontal > 0.5) ? float2(ts.x,0) : float2(0,ts.y);

                // Gaussian weights (standard 7-tap separable)
                float w0 = 0.227027;
                float w1 = 0.1945946;
                float w2 = 0.1216216;
                float w3 = 0.054054;

                fixed4 col = tex2D(_MainTex, i.uv) * w0;
                col += tex2D(_MainTex, i.uv + offset) * w1;
                col += tex2D(_MainTex, i.uv - offset) * w1;
                col += tex2D(_MainTex, i.uv + offset * 2.0) * w2;
                col += tex2D(_MainTex, i.uv - offset * 2.0) * w2;
                col += tex2D(_MainTex, i.uv + offset * 3.0) * w3;
                col += tex2D(_MainTex, i.uv - offset * 3.0) * w3;

                return col;
            }
            ENDCG
        }
    }
}
