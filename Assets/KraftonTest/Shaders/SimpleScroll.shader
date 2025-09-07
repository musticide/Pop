Shader "Unlit/SimpleScroll"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Dst("Source", Integer) = 10
        _MainTex ("Texture", 2D) = "white" {}
        _Scroll ("Scroll", Vector) = (0,0,0,0)
        _Range ("Mask", Vector) = (0,1,0,1)


        _NoiseTex ("Noise", 2D) = "grey" {}
        _DisAmount ("Distortion amount", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags {
        "RenderType"="Transparent"
        "Queue"="Transparent"
        }
        Blend [_Src][_Dst]
        Cull Off
        ZWrite Off
        ZTest LEqual
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                half4 color : VAR_COLOR;
                float rand : VAR_RAND;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _DisAmount;
            float2 _Scroll;
            half4 _Range;
            CBUFFER_END

            half Remap(half value, half from1, half to1, half from2, half to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv.xy;
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.rand = v.uv.z;
                o.uv += _Scroll.xy * _Time.y;
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 fnl = 0;
                fnl.a = 1;

                float2 noiseUV = i.uv0 * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
                noiseUV = noiseUV + _DisAmount.zw * _Time.y;
                half noise = tex2D(_NoiseTex, noiseUV).r;
                noise = (noise - .5h) * 2.h;
                // return noise;

                float2 offset = noise * _DisAmount.xy + i.rand;

                half4 col = tex2D(_MainTex, i.uv + offset);

                // float2 maskUV = i.uv0;

                half mask = i.uv0.y + offset.y;
                mask = 1 - abs ((mask - .5h) * 2.h);
                mask = Remap(mask, _Range.x, _Range.y, _Range.z, _Range.w);
                // return mask;

                fnl = col * mask;
                fnl *= i.color;

                return fnl;
            }
            ENDCG
        }
    }
}
