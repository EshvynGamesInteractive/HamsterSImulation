Shader "Custom/URP/BathtubWater"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.05,0.4,0.6,1)
        _Opacity ("Opacity", Range(0,1)) = 0.8

        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalTiling ("Normal Tiling", Vector) = (2,2,0,0)
        _NormalScale ("Normal Strength", Range(0,2)) = 1.0
        _NormalSpeed ("Normal Scroll Speed", Vector) = (0.3, 0.2,0,0)

        _SpecColor ("Specular Color", Color) = (1,1,1,1)
        _SpecPower ("Specular Power", Range(1,128)) = 32
        _SpecIntensity ("Specular Intensity", Range(0,5)) = 1.2

        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0.1,8)) = 2.0
        _FresnelIntensity ("Fresnel Intensity", Range(0,2)) = 0.7

        _FoamTex ("Foam Mask", 2D) = "white" {}
        _FoamTiling ("Foam Tiling", Vector) = (1,1,0,0)
        _FoamStrength ("Foam Strength", Range(0,2)) = 1.0

        _WaterHeight ("Water Level Y (world)", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionH : SV_POSITION;
                float2 uv        : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
                float3 viewDir   : TEXCOORD2;
                float3 normalWS  : TEXCOORD3;
                float3 tangentWS : TEXCOORD4;
                float3 bitangentWS : TEXCOORD5;
            };

            // Properties (mapped to C#)
            float4 _BaseColor;
            float _Opacity;

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            float4 _NormalTiling;
            float _NormalScale;
            float4 _NormalSpeed;

            float4 _SpecColor;
            float _SpecPower;
            float _SpecIntensity;

            float4 _FresnelColor;
            float _FresnelPower;
            float _FresnelIntensity;

            TEXTURE2D(_FoamTex);
            SAMPLER(sampler_FoamTex);
            float4 _FoamTiling;
            float _FoamStrength;

            float _WaterHeight;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 worldPos4 = TransformObjectToWorld(IN.positionOS);
                OUT.worldPos = worldPos4.xyz;
                OUT.positionH = TransformWorldToHClip(OUT.worldPos);

                OUT.uv = IN.uv;

                OUT.normalWS = normalize(TransformObjectToWorldNormal(IN.normalOS));
                float3 tangent = IN.tangentOS.xyz;
                OUT.tangentWS = normalize(TransformObjectToWorldDir(tangent));
                OUT.bitangentWS = cross(OUT.normalWS, OUT.tangentWS) * IN.tangentOS.w;

                OUT.viewDir = normalize(_WorldSpaceCameraPos - OUT.worldPos);
                return OUT;
            }

            // unpack normal map from texture
            float3 UnpackNormalMap(float4 n)
            {
                // standard normal map unpack: [0,1] -> [-1,1]
                return normalize(n.xyz * 2.0 - 1.0);
            }

            float4 frag(Varyings IN) : SV_Target
            {
                // animate normal UVs
                float2 uv1 = IN.uv * _NormalTiling.xy;
                uv1 += _NormalSpeed.xy * _Time.y;
                float4 nSample = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv1);
                float3 nTS = UnpackNormalMap(nSample) * _NormalScale;

                // build TBN
                float3x3 TBN = float3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                float3 bumpedNormal = normalize(mul(TBN, nTS));

                // lighting (fake single directional light)
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = saturate(dot(bumpedNormal, lightDir));
                float3 diffuse = _BaseColor.rgb * ndotl;

                // specular (Blinn-Phong)
                float3 V = normalize(IN.viewDir);
                float3 H = normalize(lightDir + V);
                float spec = pow(saturate(dot(bumpedNormal, H)), _SpecPower) * _SpecIntensity;
                float3 specular = _SpecColor.rgb * spec;

                // fresnel rim
                float fresnel = pow(1.0 - saturate(dot(V, bumpedNormal)), _FresnelPower) * _FresnelIntensity;
                float3 fresnelCol = _FresnelColor.rgb * fresnel;

                // foam based on world Y relative to water height (edges)
                float foamMask = 0.0;
                float waterDiff = saturate((_WaterHeight - IN.worldPos.y) * 5.0); // steeper near edges
                float2 foamUV = IN.worldPos.xz * _FoamTiling.xy;
                float foamSample = SAMPLE_TEXTURE2D(_FoamTex, sampler_FoamTex, foamUV).r;
                foamMask = foamSample * waterDiff * _FoamStrength;

                // final color composition
                float3 col = _BaseColor.rgb * (0.2 + 0.8 * ndotl) + specular + fresnelCol;
                col = lerp(col, float3(1,1,1), foamMask * 0.6); // foam brightening

                float alpha = _Opacity;

                return float4(col, alpha);
            }
            ENDHLSL
        } // Pass
    } // SubShader

    FallBack "Unlit/Transparent"
}
