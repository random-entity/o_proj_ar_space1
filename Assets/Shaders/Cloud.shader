Shader "Unlit/Cloud"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Transparent"
        }
        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;

            struct meshInput
            {
                float4 vertex : POSITION;
            };

            struct interpolators
            {
                float4 vertex : SV_POSITION;
            };

            interpolators vert (meshInput v)
            {
                interpolators o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (interpolators i) : SV_Target
            {
                return _Color;
            }
            ENDCG
        }
    }
}