Shader "Custom/URP/BloomColorUnlit"
{
   Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)

        [HDR] _EmissionColor("Emission Color", Color) = (0.3,1.0,1.0,1.0)
        _EmissionIntensity("Emission Intensity", Float) = 1.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Forward"
            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
                half4  color      : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
                half4  color       : COLOR;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                half4 _EmissionColor;
                float _EmissionIntensity;
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                // SpriteRenderer.color を受け取る
                OUT.color = IN.color * _BaseColor;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // 個体ごとのアルファがここで効く
                half4 baseCol = tex * IN.color;

                // アルファに連動した発光
                half3 emission = _EmissionColor.rgb * _EmissionIntensity * baseCol.a;

    half3 finalRGB = lerp(baseCol.rgb, _EmissionColor.rgb, baseCol.a);
finalRGB += emission;


                return half4(finalRGB, baseCol.a);
            }
            ENDHLSL
        }
    }
}
