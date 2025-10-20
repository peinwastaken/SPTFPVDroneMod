Shader "FPV/Scanlines"
{
    Properties
    {
        _MainTex ("BaseTex", 2D) = "white" {}
        _ScanIntensity ("Scanline Intensity", Range(0,1)) = 0.15
        _LineDensity ("Line Density", Range(100,2000)) = 600
        _LineBrightness ("Line Brightness", Range(0,2)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZWrite Off Cull Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ScanIntensity;
            float _LineDensity;
            float _LineBrightness;

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
                fixed4 baseCol = tex2D(_MainTex, uv);

                // horizontal line pattern
                float linePattern = sin(uv.y * _LineDensity * 3.14159);
                float brightness = lerp(1.0 - _ScanIntensity, 1.0, abs(linePattern));
                baseCol.rgb *= brightness * _LineBrightness;
                baseCol.a = 1.0;

                return baseCol;
            }
            ENDHLSL
        }
    }
}
