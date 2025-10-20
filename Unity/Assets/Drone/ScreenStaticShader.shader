Shader "FPVDrone/ScreenStaticOverlay"
{
    Properties
    {
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (0,0,0,0)
        _ResX ("X Resolution", Float) = 100
        _ResY ("Y Resolution", Float) = 200
        _Intensity ("Transparency (Alpha)", Range(0,1)) = 0.5
        _MainTex ("Camera Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _ColorA;
            fixed4 _ColorB;
            float _ResX;
            float _ResY;
            float _Intensity;

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy , float2(12.345 * _Time.y, 67.890 * _Time.y))) * 12345.6789 + _Time.y);
            }

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Get the camera color
                fixed4 baseCol = tex2D(_MainTex, uv);

                // Generate static noise
                uv.x = round(uv.x * _ResX) / _ResX;
                uv.y = round(uv.y * _ResY) / _ResY;
                float noise = rand(uv);
                fixed4 stat = lerp(_ColorA, _ColorB, noise);
                stat.a = _Intensity;

                // Blend static over the camera image
                return lerp(baseCol, stat, stat.a);
            }
            ENDCG
        }
    }
}
