Shader "Unlit/Fresnel"
{
    Properties
    {
        [Enum(UnityEngine.Rendering.BlendMode)] _Src("Source", Integer) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _Dst("Source", Integer) = 10
        [HDR]_Tint ("Tint", Color) = (1,1,1,1)
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
                float3 normal : NORMAL;
                half4 color : COLOR;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float3 positionWS : VAR_POSITION_WS;
                float3 normalWS : VAR_NORMAL_WS;
                float4 positionCS : SV_POSITION;
                half4 color : VAR_COLOR;
            };


            CBUFFER_START(UnityPerMaterial)
            half4 _Tint;
            CBUFFER_END

           float Fresnel(float3 viewDir, float3 normal)
           {
                return 1.0 - saturate(dot(viewDir, normal));
           }

            v2f vert (appdata v)
            {
                v2f o;
                o.positionCS = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normalWS = UnityObjectToWorldNormal(v.normal);

                o.uv0 = v.uv;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 fnl = 0;
                fnl.a = 1;

                float fresnel = Fresnel(normalize(_WorldSpaceCameraPos - i.positionWS.xyz), i.normalWS);
                fnl = fresnel * _Tint;
                fnl *= i.color;

                return fnl;
            }
            ENDCG
        }
    }
}
