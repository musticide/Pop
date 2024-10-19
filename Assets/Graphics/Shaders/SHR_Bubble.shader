Shader "Pop/Bubble Shader"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (0,0,0,0)
        _F0("Reflectance at Normal Incidence (F0)", Range(0, 1)) = 0.04
        _FresnelBlend("Fresnel Blend", Range(0, 10)) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent"}
        // Blend SrcAlpha OneMinusSrcAlpha
        Blend SrcAlpha One 
        ZWrite Off
        ZTest LEqual
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
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            float4 _BaseColor;
            float4 _RimColor;
            float _F0;
            float _FresnelBlend;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalWS = normalize(mul(v.normal, (float3x3)unity_WorldToObject));
                o.viewDirWS = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex).xyz);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 finalCol = 0;
                finalCol.a = 1;
                float3 normalWS = normalize(i.normalWS);
                // finalCol.rgb = normalWS;
                // return finalCol;
                float3 viewDirWS = normalize(i.viewDirWS);

                // Calculate the Fresnel effect using Schlick's approximation
                float cosTheta = saturate(dot(viewDirWS, normalWS));
                float fresnel = _F0 + (1.0 - _F0) * pow(1.0 - cosTheta, _FresnelBlend);

                // Apply the Fresnel effect to the base color
                float3 color = _BaseColor.rgb + _RimColor.rgb * fresnel;

                return float4(color,saturate(_BaseColor.a + fresnel)); 
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
