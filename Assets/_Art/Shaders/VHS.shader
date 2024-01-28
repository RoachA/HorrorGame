Shader "PostProcess/VHS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Texture", 2D) = "white" {}
        _Resolution ("Resolution", Vector) = (1,1,0,0)
    }

    SubShader
    {
        Tags { "Queue" = "Overlay" }
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
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv2 = v.uv * 10;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            SamplerState sampler_point_clamp;
            float2 _Resolution;
            uniform float _DelayAmount;
            uniform float _DelayOffset;
            uniform float _ChromaOffset;
            uniform float2 _ChromaVector;
            uniform float _Sharpness;
            
            uniform float _DotCrawlSpeed;
            uniform float _DotCrawlAmount;
            uniform float2 _DotCrawlVector;

            uniform float _RingingAmount;

            uniform float _NoiseAmount;
            uniform float _NoiseSpeed;
            uniform float2 _NoiseDistribution;

            //https://github.com/JargeZ/ntscqt
            fixed4 frag(v2f i) : SV_Target
            {                 
                float2 uv = i.uv;
                float2 uv2 = i.uv2;
                float2 delayUV = uv + float2(uv.y * _ChromaVector.x * _DelayOffset, uv.x * _ChromaVector.y * _DelayOffset) * _DelayAmount;
                delayUV += float2(cos(uv.y * _DotCrawlVector.x  + _Time.y * _DotCrawlSpeed) * _DotCrawlAmount, cos(uv.x * _DotCrawlVector.y + _Time.y * _DotCrawlSpeed) * _DotCrawlAmount);

                // Sample the RGB values from the current pixel and the delayed pixels
                fixed3 originalRGB = tex2D(_MainTex, uv).rgb;
                fixed3 delayedRGB = tex2D(_MainTex, delayUV).rgb;
               
                // Apply chroma offset to delayed RGB values
                fixed3 finalRGB = fixed3(originalRGB.r + _ChromaOffset, delayedRGB.g, originalRGB.b + _ChromaOffset);

                finalRGB += _RingingAmount * (finalRGB - originalRGB);

                 float time = _Time.y * 1;

               
                float flickerNoise = frac(sin(dot(i.uv2 + time, float2(12.9898, 78.233))) * 41);
                float flickerValue = _NoiseAmount - flickerNoise;

        
                 float grayscale = dot(originalRGB.rgb, fixed3(0.299, 0.587, 0.114));
                grayscale = smoothstep(_NoiseDistribution.x, _NoiseDistribution.y, grayscale);
                fixed3 noiseSample = tex2D(_NoiseTex, uv2 + _Time.y * _NoiseSpeed).rgb;
                
                finalRGB = lerp(finalRGB, noiseSample + finalRGB,  grayscale * flickerValue);

                finalRGB = originalRGB + (_Sharpness * (finalRGB - originalRGB));


                // Output the final color
                return fixed4(finalRGB, 1.0);        
            }
  
            ENDCG
        }
    }

    FallBack "Diffuse"
}
