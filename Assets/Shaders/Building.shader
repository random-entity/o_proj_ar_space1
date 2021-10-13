Shader "Unlit/Building"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _AngWid ("AngWid", Range(0, 6.28318530718)) = 1
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define AngWidToHeight 80
            #define Radius 50
            #define CenterArg 1.57079632679

            struct meshInput
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct interpolators
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _AngWid;

            interpolators vert (meshInput v)
            {
                interpolators o;

                float arg = CenterArg + (v.uv.x - 0.5) * _AngWid;
                
                float x = Radius * cos(arg);
                float z = Radius * sin(arg);
                float y = _AngWid * AngWidToHeight * v.uv.y;

                o.vertex = UnityObjectToClipPos(float4(x, y, z, 0));
                o.uv = v.uv;
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