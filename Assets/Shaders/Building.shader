Shader "Unlit/Building"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _WindowColor ("Window Color", Color) = (0,1,0,1)
        _AngWid ("AngWid", Range(0, 6.28318530718)) = 1
        _WindowMargin ("Window Margin Left Right Bottom Top", Vector) = (.2, .2, .2, .1)
        _NumWinX ("Number of columns of windows", Int) = 8
        _NumWinY ("Number of rows of windows", Int) = 8
    }
    SubShader
    {
        Tags 
        {
            "RenderType"="Opaque"
        }
        Pass
        {
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            #define AngWidToHeight 80
            #define Radius 50
            #define CenterArg 1.57079632679 // 90 degrees

            float4 _Color;
            float4 _WindowColor;
            float _AngWid;
            float4 _WindowMargin;
            int _NumWinX;
            int _NumWinY;

            float _WindowAlphas[100];

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

            float inverseLerp(float start, float end, float spot) {
                return (spot - start) / (end - start);
            }

            interpolators vert (meshInput v)
            {
                interpolators o;

                float arg = CenterArg + (v.uv.x - 0.5) * _AngWid;
                float r = Radius * pow(1.02944, _AngWid); // 1.2^(1/(2pi)) = 1.02944
            
                float x = r * cos(arg);
                float z = r * sin(arg);
                float y = _AngWid * AngWidToHeight * v.uv.y;

                o.vertex = UnityObjectToClipPos(float4(x, y, z, 0));
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (interpolators i) : SV_Target
            {
                if (_WindowMargin.x < i.uv.x && i.uv.x < 1 - _WindowMargin.y && _WindowMargin.z < i.uv.y && i.uv.y < 1 - _WindowMargin.w)
                {
                    float x = inverseLerp(_WindowMargin.x, 1 - _WindowMargin.y, i.uv.x);
                    float y = inverseLerp(_WindowMargin.z, 1 - _WindowMargin.w, i.uv.y);
                    
                    int winResX = _NumWinX * 2 - 1;
                    float winCheckX = floor(x * winResX);

                    int winResY = _NumWinY * 2 - 1;
                    float winCheckY = floor(y * winResY);

                    if (winCheckX % 2 == 0 && winCheckY % 2 == 0) 
                    {
                        int alphaIndexX = winCheckX / 2;
                        int alphaIndexY = winCheckY / 2;
                        float winAlpha = _WindowAlphas[(alphaIndexX + _NumWinX * alphaIndexY) % _WindowAlphas.Length];

                        return _Color * (1 - winAlpha) + _WindowColor * winAlpha;
                    }
                }

                return _Color;
            }
            ENDCG
        }
    }
}