Shader "Unlit/LandPath"
{
    Properties
    {
        _AngWidB2S ("AngWid of BuildingToSky building", Range(0, 6.28318530718)) = 0.4
        _AngWidD2B ("AngWid of DoorToBuilding building", Range(0, 6.28318530718)) = 0.2
        _Color0 ("Color of the path", Color) = (1, 0, 0, 1)
        _Color1 ("Color outside of the path", Color) = (0, 1, 0, 1)
        _Color2 ("Color outside of the outside of the path", Color) = (0, 0, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float _AngWidB2S;
            float _AngWidD2B;
            float4 _Color0;
            float4 _Color1;
            float4 _Color2;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = abs(i.uv.x - 0.5);

                float4 color = 0;

                if(_AngWidB2S < 3.14159265359)
                {
                    if(t < 0.5 * sin(_AngWidB2S * 0.5))
                    {
                        color =  _Color1;
                    }
                    else 
                    {
                        color = _Color2;
                    }
                }
                else 
                {
                    color = _Color1;
                }

                if(t < 0.5 * sin(_AngWidD2B * 0.5))
                {
                    color = color * (1 - _Color0.a) + _Color0 * _Color0.a;
                }

                return color;
            }
            ENDCG
        }
    }
}
