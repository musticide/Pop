Shader "Unlit/Particle"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Dst("Source", Integer) = 10
        _MainTex ("Texture", 2D) = "white" {}
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
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                half4 color : VAR_COLOR;
            };

            sampler2D _MainTex;

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 fnl = 0;
                fnl.a = 1;
                half4 col = tex2D(_MainTex, i.uv);

                fnl = col;
                fnl *= i.color;

                return fnl;
            }
            ENDCG
        }
    }
}
