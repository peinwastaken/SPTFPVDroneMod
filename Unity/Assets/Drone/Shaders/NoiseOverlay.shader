Shader "FPV/NoiseOverlay"
{
    Properties
    {
        _MainTex ("BaseTex", 2D) = "white" {}
        _ResX ("Noise Res X", Range(1,512)) = 100
        _ResY ("Noise Res Y", Range(1,512)) = 200
        _Intensity ("Intensity", Range(0,1)) = 0.35
        _TimeScale ("Time Scale", Float) = 1.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.25
        _BlurAmount ("Blur Amount", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" "IgnoreProjector"="True" }
        ZWrite Off Cull Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _ResX, _ResY, _Intensity, _TimeScale, _Smoothness, _BlurAmount;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float hash21(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.x + p3.y) * p3.z);
            }

            float randT(float2 co, float t)
            {
                return hash21(co + t * 0.37);
            }

            float smoothNoise(float2 uv, float t)
            {
                float2 g = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                float a = randT(g, t);
                float b = randT(g + float2(1,0), t);
                float c = randT(g + float2(0,1), t);
                float d = randT(g + float2(1,1), t);

                float ab = lerp(a,b,f.x);
                float cd = lerp(c,d,f.x);
                return lerp(ab,cd,f.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _TimeScale;

                fixed4 baseCol = tex2D(_MainTex, uv);

                // grid sampling
                float2 gUV = uv;
                gUV.x = round(gUV.x * _ResX) / _ResX;
                gUV.y = round(gUV.y * _ResY) / _ResY;

                float nBlock = randT(gUV * float2(_ResX,_ResY), t);
                float nSmooth = smoothNoise(gUV * float2(_ResX,_ResY), t);
                float n = lerp(nBlock, nSmooth, _Smoothness);

                float3 noiseCol = float3(n,n,n);

                // simple 3x3 blur around texel
                float2 texelSize = float2(1.0/_ResX, 1.0/_ResY);
                float3 blurCol = float3(0,0,0);
                int count = 0;
                for(int x=-1;x<=1;x++)
                    for(int y=-1;y<=1;y++)
                    {
                        float2 offsetUV = gUV + float2(x,y)*texelSize;
                        float nb = randT(offsetUV * float2(_ResX,_ResY), t);
                        float ns = smoothNoise(offsetUV * float2(_ResX,_ResY), t);
                        float nv = lerp(nb, ns, _Smoothness);
                        blurCol += float3(nv,nv,nv);
                        count++;
                    }
                blurCol /= count;

                // blend with original texel based on _BlurAmount
                noiseCol = lerp(noiseCol, blurCol, _BlurAmount);

                fixed4 noiseOut = fixed4(noiseCol, _Intensity);

                fixed4 outCol;
                outCol.rgb = lerp(baseCol.rgb, noiseOut.rgb, noiseOut.a);
                outCol.a = 1.0;

                return outCol;
            }
            ENDHLSL
        }
    }
}
