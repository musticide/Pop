Shader "Hidden/musticide/UI/RichImageShader"
{
    Properties
    {
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData]_SecTex ("Sprite Texture 2", 2D) = "black" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // _TexBlendMode ("Texture Blend", Float) = 0
        [KeywordEnum(Alpha, Add, Multiply, Overlay)] _TexBlendMode ("Blend Mode Test", Float) = 0

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
            #pragma shader_feature _CLAMP_SECTEX

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

            Varyings vert(Attributes input)
            {
                Varyings o;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.positionWS = input.positionOS;
                o.positionCS = UnityObjectToClipPos(o.positionWS);

                o.uv = input.uv;
                o.uv1 = TRANSFORM_TEX(input.uv, _MainTex);
                // o.uv2 = TRANSFORM_TEX(input.uv, _SecTex);
                o.uv2 = input.uv;// * _SecTex_ST.xy + _SecTex_ST.zw;

                o.uv2 *= _SecTex_UserST.xy;
                o.uv2 += _SecTex_UserST.zw;

                // o.uv2 *= _SecTex_ST.xy;
                // o.uv2 += _SecTex_ST.zw;

                // o.uv2.x *= _SecTex_ST.x;
                // o.uv2 *= _SecTex_TexelSize.zw;
                // o.uv2 *= _MainTex_TexelSize.zw;

                o.color = input.color * _Color;
                return o;
            }

            fixed4 frag(Varyings input) : SV_Target
            {
                half4 fnl = half4(0,0,0,1);
                float2 uv = input.uv;

                float2 mTexUV = input.uv1;
                half4 mainTex = tex2D(_MainTex, mTexUV);

                // float2 sTexUV = input.uv2 * _SecTex_UserST.xy + _SecTex_UserST.zw;
                // float2 sTexUV = input.uv2 * _SecTex_ST.xy + _SecTex_ST.zw;
                float2 sTexUV = frac(input.uv2);// * _SecTex_UserST.xy + _SecTex_UserST.zw;
    
                sTexUV *= _SecTex_ST.xy;
                sTexUV += _SecTex_ST.zw;

                // float2 maxUV = _SecTex_ST.xy + _SecTex_ST.zw;
                // float2 minUV = _SecTex_ST.zw;
                // sTexUV.x = MOD(sTexUV.x, maxUV.x);
                // sTexUV.y = MOD(sTexUV.y, maxUV.y);

                // sTexUV = RemapRange(frac(sTexUV), 0,1, minUV, maxUV);

                half4 secTex = tex2D(_SecTex, sTexUV);

                half4 mixTex = lerp(mainTex, secTex, secTex.a);//default

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
