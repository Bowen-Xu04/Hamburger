Shader "Custom/Lit Outline"
{
    Properties
    {
        [Header(Outline)]
        _StrokeColor ("Stroke Color", Color) = (0,0,0,1)
        _Outline ("Outline Width", Range(0,1)) = 0.1
        _MaxOutlineZOffset ("Max Outline Z Offset", Range(0,1)) = 0.5

        [Header(StencilData)]
        //_StencilComp ("Stencil Comparison", Float) = 3   //比较的操作：Equal
        _Stencil ("Stencil ID", Int) = 1                //比较参考值
        //_StencilOp ("Stencil Operation", Float) = 2        //模板操作：Replace
    }

    Subshader
    {
        Tags
        {
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Interior"

            Stencil
            {
                Ref [_Stencil]
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SpaceTransforms.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            int _Stencil;

            struct a2v
            {
                float4 vertex: POSITION;
                float4 normal: NORMAL;
                float2 uv: TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex: SV_POSITION;
                float2 uv: TEXCOORD0;
            };

            v2f Vert(a2v v)
            {
                v2f o;
                o.vertex = GetVertexPositionInputs(v.vertex.xyz).positionCS;
                o.uv=v.uv;
                return o;
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            }

            float4 Frag(v2f i) : SV_TARGET
            {
                if(_Stencil==1){
                    return float4(0,0,1,1);
                }
                else{
                    return float4(0,1,0,1);
                }
            }

            ENDHLSL
        }

        Pass
        {
            Name "Outline"

            // Stencil
            // {
                //     Ref 1
                //     Comp Equal
                //     Pass Keep
                // }

                //Cull Front

                HLSLPROGRAM

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

                #pragma vertex Vert
                #pragma fragment Frag

                float4 _StrokeColor;
                sampler2D _OutlineNoise;
                float _Outline;
                float _OutsideNoiseWidth;
                float _MaxOutlineZOffset;

                int _Stencil;

                struct a2v
                {
                    float4 vertex: POSITION;
                    float4 normal: NORMAL;
                    float2 uv: TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex: SV_POSITION;
                    float2 uv: TEXCOORD0;
                };

                v2f Vert(a2v v)
                {
                    v2f o;
                    // 将顶点和法线从模型空间变换到世界空间
                    o.vertex = GetVertexPositionInputs(v.vertex.xyz).positionCS;

                    float3 worldNormal = normalize(TransformObjectToWorld(v.normal.xyz));
                    // 沿法线方向外扩顶点
                    o.vertex.xyz += worldNormal * 0.05;//_OutlineWidth;
                    // 将结果变换到裁剪空间
                    //o.vertex = mul(UNITY_MATRIX_VP, worldPos);
                    return o;
                }

                float4 Frag(v2f i) : SV_TARGET
                {
                    return float4(1,1,1,1);
                    //return _StrokeColor;
                }

                ENDHLSL
            }


        }
    }
