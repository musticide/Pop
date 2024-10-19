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
        _Pos("Position", Vector) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent"}
        Blend SrcAlpha One 
        ZWrite Off
        ZTest LEqual
        Cull Off
        LOD 100

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
                float3 vertex :TEXCOORD4;
            };

            float4 _BaseColor;
            float4 _RimColor;
            float _F0;
            float _FresnelBlend;
            float _Radius;
            half _MaxSteps;
            half _SurfDist;
            half _MaxDist;

            half4 _Pos;

            half4 _BTrans[25];


            float SphereSDF(float3 s, float r){
                float d = length(s.xyz) - (r * 0.5f);
                return d;
            }

            float SDFFunction(float3 p)
            {
                float sdf = SphereSDF(p - _BTrans[0].xyz, _BTrans[0].w);

                for(int i = 1; i < 25; i++){
                    sdf = min(sdf, SphereSDF(p - _BTrans[i].xyz, _BTrans[i].w));
                }
                // float sdf = SphereSDF(p - _Pos.xyz, _Pos.w);

                return sdf;
            }

            // #include "Assets/Graphics/Shaders/Includes/FunctionLibrary.hlsl"

            half Raymarch(half3 ro, half3 rd, int maxSteps, half surfaceDist, half maxDist){
                half dO = 0;
                half dS;
                for(int i = 0; i < maxSteps; i++){
                    half3 p = ro + dO * rd;

                    dS = SDFFunction(p);
                    dO += dS;

                    if (dS < surfaceDist || dO > maxDist)break;
                }
                return dO;
            }

            float3 GetNormals(half3 p){
                half2 o = half2(.01, 0);
                half3 n = SDFFunction(p) - (half3(
                            SDFFunction(p - o.xyy),
                            SDFFunction(p - o.yxy),
                            SDFFunction(p - o.yyx)
                ));
                return normalize(n);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.posWS = mul(v.vertex, UNITY_MATRIX_MV);
                o.normalWS = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.viewDirWS = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                o.vertex = v.vertex.xyz;
                o.uv = v.uv;
                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float4 finalCol = 0;
                finalCol.a = 1;
                float3 normalWS = normalize(i.normalWS);
                float3 viewDirWS = normalize(i.viewDirWS);

                half2 uv = i.uv - .5;
                half3 ro = _WorldSpaceCameraPos;
                half3 rd = normalize(i.posWS - ro);

                float d = Raymarch(ro, rd, _MaxSteps, _SurfDist, _MaxDist);

                if(d < _MaxDist){
                    half3 p = ro + (rd * d);
                    normalWS = GetNormals(p);
                    viewDirWS = normalize(_WorldSpaceCameraPos -  p);
                }
                else{
                    discard;
                }

                // return finalCol;


                float cosTheta = saturate(dot(viewDirWS, normalWS));
                float fresnel = _F0 + (1.0 - _F0) * pow(1.0 - cosTheta, _FresnelBlend);

                float3 color = _BaseColor.rgb + _RimColor.rgb * fresnel;

                finalCol.rgb = color;
                // finalCol.rgb = normalWS;
                finalCol.a = saturate(_BaseColor.a + fresnel);
                // finalCol.a = 1;

                return finalCol;
            }
            ENDHLSL
        }

    }
}
