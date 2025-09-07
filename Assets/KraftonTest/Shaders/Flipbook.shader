Shader "Unlit/Flipbook"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Dst("Source", Integer) = 10
        [NoScaleOffset]_MainTex ("Texture", 2D) = "white" {}
        [HDR] _Tint("Tint", Color) = (1,1,1,1)

        _Speed("Speed", Float) = 1.0
        _Rows("Row", Integer) = 1
        _Columns("Column", Integer) = 1
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
            float _Speed;
            float _Rows;
            float _Columns;
            half4 _Tint;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv.xy;
                o.uv = v.uv.xy;

                int totalFrames = _Rows * _Columns;

                float currentFrame = fmod(_Time.y * _Speed + v.uv.z, totalFrames);
                int frameIndex = floor(currentFrame);

                int row = frameIndex / _Columns;
                int col = frameIndex % _Columns;

                float2 frameSize = float2(1.0 / _Columns, 1.0 / _Rows);

                float2 frameOffset = float2(col * frameSize.x, ((_Rows - 1) - row) * frameSize.y);
                o.uv = v.uv.xy * frameSize + frameOffset;

                o.rand = v.uv.z;

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
                fnl *= _Tint;

                return fnl;
            }
            ENDCG
        }
    }
}
