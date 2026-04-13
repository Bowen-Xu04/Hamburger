Shader "Custom/URP_StencilOutline"
{
    Properties
    {
        // 物体本体的主纹理和颜色
        _BaseMap ("Base Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)

        // 描边的颜色和宽度
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.01
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        // --------------------------------------------------
        // Pass 1: 渲染物体本体，并写入模板值
        // --------------------------------------------------
        Pass
        {
            Name "BasePass"
            Tags { "LightMode" = "UniversalForward" }

            Stencil
            {
                Ref 1                // 设定参考值为 1
                Comp Always          // 总是通过模板测试
                Pass Replace         // 通过时，将像素的模板值替换为 1
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST;
            float4 _BaseColor;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                return baseMap * _BaseColor;
            }
            ENDHLSL
        }

        // --------------------------------------------------
        // Pass 2: 渲染描边，并进行模板测试裁剪
        // --------------------------------------------------
        Pass
        {
            Name "OutlinePass"
            Tags { "LightMode" = "SRPDefaultUnlit" }

            // 剔除正面，只渲染背面（通常是描边可见的部分）
            Cull Front

            Stencil
            {
                Ref 1                // 同样参考值为 1
                Comp NotEqual        // 只在模板值不为1的地方通过
                Pass Keep            // 通过时，保持模板值不变（或不写）
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert(Attributes input)
            {
                Varyings output;

                // 1. 将顶点沿法线方向外扩
                float3 expandedPosOS = input.positionOS.xyz + input.normalOS * _OutlineWidth;

                // 2. 将外扩后的顶点转换到裁剪空间
                output.positionHCS = TransformObjectToHClip(expandedPosOS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}