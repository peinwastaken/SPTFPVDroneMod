Shader "FPV/ScreenBlur"
{
    Properties
    {
        _MainTex ("BaseTex", 2D) = "white" {}
        _BlurSize ("Blur Size", Range(0,4)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                if (_BlurSize <= 0.001) return tex2D(_MainTex, uv);

                float2 t = _MainTex_TexelSize.xy * _BlurSize;
                float3 sum = float3(0,0,0);

                sum += tex2D(_MainTex, uv + t * float2(-1,-1)).rgb;
                sum += tex2D(_MainTex, uv + t * float2( 0,-1)).rgb;
                sum += tex2D(_MainTex, uv + t * float2( 1,-1)).rgb;
                sum += tex2D(_MainTex, uv + t * float2(-1, 0)).rgb;
                sum += tex2D(_MainTex, uv).rgb;
                sum += tex2D(_MainTex, uv + t * float2( 1, 0)).rgb;
                sum += tex2D(_MainTex, uv + t * float2(-1, 1)).rgb;
                sum += tex2D(_MainTex, uv + t * float2( 0, 1)).rgb;
                sum += tex2D(_MainTex, uv + t * float2( 1, 1)).rgb;

                sum /= 9.0;
                return float4(sum, 1.0);
            }
            ENDHLSL
        }
    }
}
