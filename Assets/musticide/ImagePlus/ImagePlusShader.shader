Shader "Hidden/musticide/UI/ImagePlus Shader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData]_SecTex ("Sprite Texture 2", 2D) = "black" {}
        [HideInInspector]_Color ("Tint", Color) = (1,1,1,1)

        [HideInInspector]_TexBlendMode ("Texture Blend", Float) = 0

        // _Gleam("Gleam Width Angle Speed Space", Vector) = (0.2, 0.0, 0.5, 2.0)
        // _GleamWidth ("Gleam Width", Float) = 0.5
        // _GleamAngle ("Gleam Angle", Float) = 0.0
        // _GleamSpeed ("Gleam Speed", Float) = 0.0
        // _GleamSpace ("Gleam Space", Float) = 1.0

        [HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector]_Stencil ("Stencil ID", Float) = 0
        [HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector]_ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma shader_feature_fragment _TEXBLENDMODE_ALPHA _TEXBLENDMODE_ADD _TEXBLENDMODE_MULTIPLY _TEXBLENDMODE_OVERLAY
            #pragma shader_feature _TILE_SECTEX
            #pragma shader_feature _CLIP_SECTEX_TO_BASE

            #pragma multi_compile_fragment _GLEAM

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv  : TEXCOORD0;
                float2 uv1 : VAR_MAINTEX_UV;
                float2 uv2 : VAR_SECTEX_UV;
                float4 positionWS : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            sampler2D _SecTex;
            float4 _SecTex_ST;
            float4 _SecTex_TexelSize;
            float4 _SecTex_UserST;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;

            float4 _Gleam;

            half3 Luminance(float4 color)
            {
                return dot(color.rgb, half3(0.299, 0.587, 0.114));
            }
            half3 Overlay(half3 a, half3 b)
            {
            return Luminance(a) > 0.5?
                1 - (1-a) * (1-b):
                2 * a * b;
            }

            float MOD(float x, float y)
            {
                return x - y * floor(x / y);
            }

            float2 RemapRange(float2 val, float2 inMin, float2 inMax,float2 outMin,float2 outMax)
            {
                return (val - inMin) / (inMax - inMin) * (outMax - outMin) + outMin;
            }

            float2 CustomRotator(float2 inUV, float2 center, float angle)
            {
                    if(angle <= 0 ) return inUV;
                    inUV = (center * -1) * inUV;
                    float2 outUV = 0;
                    float sinAngle = sin(angle);
                    float cosAngle = cos(angle);
                    outUV.x = dot(inUV, float2(cosAngle, -sinAngle));
                    outUV.y = dot(inUV, float2(sinAngle, cosAngle));
                    return outUV;
            }

            Varyings vert(Attributes input)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionWS = input.positionOS;
                o.positionCS = UnityObjectToClipPos(o.positionWS);

                o.uv = input.uv;
                o.uv1 = TRANSFORM_TEX(input.uv, _MainTex);

                o.uv2 = input.uv;

                o.uv2 -= 0.5f;
                o.uv2 *= _SecTex_UserST.xy;
                o.uv2 += _SecTex_UserST.zw;
                o.uv2 += 0.5f;

                // #ifdef _PRESERVE_SECTEX_ASPECT
                // o.uv2 -= 0.5f;
                // if(_SecTex_TexelSize.x > _SecTex_TexelSize.y)
                //     o.uv2.y *= _SecTex_TexelSize.x / _SecTex_TexelSize.y;
                // else
                //     o.uv2.x *= _SecTex_TexelSize.y / _SecTex_TexelSize.x;
                // o.uv2 += 0.5f;
                // #endif

                o.color = input.color * _Color;
                return o;
            }

            fixed4 frag(Varyings input) : SV_Target
            {
                half4 fnl = half4(0,0,0,1);
                float2 uv = input.uv;

                float2 mTexUV = input.uv1;
                half4 mainTex = tex2D(_MainTex, mTexUV);

                float2 sTexUV = input.uv2;// * _SecTex_UserST.xy + _SecTex_UserST.zw;

                #ifdef _TILE_SECTEX
                    sTexUV = frac(sTexUV);
                #else
                    sTexUV = clamp(sTexUV, 0, 1);
                #endif
    
                sTexUV *= _SecTex_ST.xy;
                sTexUV += _SecTex_ST.zw;

                half4 secTex = tex2D(_SecTex, sTexUV);

                half4 mixTex = lerp(mainTex, secTex, secTex.a);//default

                #ifdef _CLIP_SECTEX_TO_BASE
                mixTex.a = mainTex.a;
                #else
                mixTex.a = max(mainTex.a, secTex.a);
                #endif

                #ifdef _TEXBLENDMODE_ALPHA
                mixTex = mixTex;
                #elif _TEXBLENDMODE_ADD
                mixTex.rgb = lerp(mixTex.rgb, mainTex.rgb + secTex.rgb, mainTex.a * secTex.a);
                #elif _TEXBLENDMODE_MULTIPLY
                mixTex.rgb = lerp(mixTex.rgb, mainTex.rgb * secTex.rgb, mainTex.a * secTex.a);
                #elif _TEXBLENDMODE_OVERLAY
                mixTex.rgb = lerp(mixTex, Overlay(mainTex.rgb, secTex.rgb), mainTex.a * secTex.a);//using the lerped alpha here
                #endif

                fnl = mixTex;
                fnl*= input.color;

                //Gleam
                // fnl.rgb = 0;

                #ifdef _GLEAM
                float2 gleamUV = input.uv;
                gleamUV = CustomRotator(gleamUV, 0.5f, _Gleam.y);
                gleamUV.x += _Time.y * _Gleam.z;
                gleamUV.x = abs(gleamUV.x);
                gleamUV.x = fmod(gleamUV.x, _Gleam.w);
                float gleam = step(gleamUV.x, _Gleam.x);
                fnl.rgb = lerp(fnl.rgb, Overlay(fnl.rgb, gleam), gleam);
                #endif


                #ifdef UNITY_UI_CLIP_RECT
                fnl.a *= UnityGet2DClipping(input.positionWS.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (fnl.a - 0.001);
                #endif

                return fnl;
            }
        ENDCG
        }
    }
}
