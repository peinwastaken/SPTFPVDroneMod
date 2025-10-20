Shader "FPV/AnalogColor"
{
    Properties
    {
        _MainTex ("BaseTex", 2D) = "white" {}
        _Chromatic ("Chromatic Aberration", Range(0,0.02)) = 0.003
        _Desaturation ("Desaturation", Range(0,1)) = 0.25
        _PosterizeLevels ("Posterize Levels", Range(1,64)) = 12
        _SepiaStrength ("Sepia Strength", Range(0,1)) = 1.0
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
            float _Chromatic;
            float _Desaturation;
            float _PosterizeLevels;
            float _SepiaStrength;

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
                float ca = _Chromatic;

                // Chromatic aberration
                float r = tex2D(_MainTex, uv + float2(ca, 0)).r;
                float g = tex2D(_MainTex, uv).g;
                float b = tex2D(_MainTex, uv - float2(ca, 0)).b;
                float3 col = float3(r,g,b);

                // Desaturation
                float gray = dot(col, float3(0.3,0.59,0.11));
                col = lerp(float3(gray,gray,gray), col, 1.0 - _Desaturation);

                // Posterize
                col = floor(col * _PosterizeLevels) / max(1.0, _PosterizeLevels);

                // Sepia tone
                float3 sepia;
                sepia.r = dot(col, float3(0.393, 0.769, 0.189));
                sepia.g = dot(col, float3(0.349, 0.686, 0.168));
                sepia.b = dot(col, float3(0.272, 0.534, 0.131));

                // Blend between original and sepia
                col = lerp(col, sepia, _SepiaStrength);

                return float4(col, 1.0);
            }
            ENDHLSL
        }
    }
}
