Shader "Pop/BubbleRM"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (0,0,0,0)
        _F0("Reflectance at Normal Incidence (F0)", Range(0, 1)) = 0.04
        _FresnelBlend("Fresnel Blend", Range(0, 10)) = 4
        _Radius("Radius", Float) = 1.0


        _MaxSteps("Max Steps", Float) = 100.0
        _SurfDist("Min Dist From Surface", Float) = 0.001
        _MaxDist("Max Distance to Travel", Float) = 100.0
    }
    SubShader
    {

    LOD 100
    Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #pragma multi_compile_instancing

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
                float2 uv : TEXCOORD2;
                float3 posWS :TEXCOORD3;
            };

            float4 _BaseColor;
            float4 _RimColor;
            float _F0;
            float _FresnelBlend;
            float _Radius;
            half _MaxSteps;
            half _SurfDist;
            half _MaxDist;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.posWS = v.vertex;
                o.normalWS = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.viewDirWS = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.uv = v.uv;
                return o;
            }

            // #define SDF_FUNCTION(x) SDFFunction(x)

            half SDFFunction(half3 p)
            {
                half d = length(p) - _Radius;
                return d;
            }

            #include "Assets/Graphics/Shaders/Includes/FunctionLibrary.hlsl"

            float4 frag(v2f i) : SV_Target
            {
                float4 finalCol = 0;
                finalCol.a =1;
                float3 normalWS = normalize(i.normalWS);
                float3 viewDirWS = normalize(i.viewDirWS);

                float cosTheta = saturate(dot(viewDirWS, normalWS));
                float fresnel = _F0 + (1.0 - _F0) * pow(1.0 - cosTheta, _FresnelBlend);

                float3 color = _BaseColor.rgb + _RimColor.rgb * fresnel;

                half2 uv = i.uv - .5;
                half3 ro = _WorldSpaceCameraPos;
                half3 rd = normalize(i.posWS - ro);

                // finalCol.rg = uv;
                // return finalCol;

                half d = Raymarch(ro, rd, _MaxSteps, _SurfDist, _MaxDist);

                if(d < 100){
                    half3 p = ro + d * rd;
                    finalCol.rgb = GetNormal(p);
                }
                return finalCol;

                return float4(color,saturate(_BaseColor.a + fresnel)); 
            }
            ENDHLSL
        }

    }
}
