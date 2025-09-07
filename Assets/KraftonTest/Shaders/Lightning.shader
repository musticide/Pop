Shader "Unlit/Lightning"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Dst("Source", Integer) = 10
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _Tint ("Tint", Color) = (1,1,1,1)

        _Line ("Line", Vector) = (0.5, 0.5, 1.0, 0.2)
        _DAmount ("Distortion Amount + Speed", Vector) = (0,0,0,0)
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

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Tint;
            float4 _Line;
            float4 _DAmount;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv.xy;
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.uv = o.uv + _Time.y * _DAmount.zw;
                o.rand = v.uv.x;
                o.color = v.color;
                return o;
            }

            float LineSDF( float2 p, float h, float r )
            {
                p.x -= clamp( p.x, 0.0, h );
                return length( p ) - r;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 fnl = 0;
                fnl.a = 1;

                half noise = tex2D(_MainTex, i.uv + i.rand/4);
                noise = (noise - .5h) * 2.h;

                // return i.rand;

                float2 lineUV = i.uv0;
                lineUV.y -= 0.5;
                lineUV += noise.r * _DAmount.xy;
                // lineUV += i.rand;
                half l = LineSDF(lineUV, 1.0, 0.0);

                l = step(l, _Line.x);

                fnl = l;
                fnl *= i.color;
                fnl *= _Tint;

                return fnl;
            }
            ENDCG
        }
    }
}
