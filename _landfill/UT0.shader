// https://forum.unity.com/threads/unlit-with-adjustable-alpha.115455/

Shader "Custom/UT0" {

    Properties {
        _Color ("Main Color (A=Opacity)", Color) = (1,1,1,1)
        _MainTex ("Base (A=Opacity)", 2D) = ""
    }
    
    Category {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True"} // <= IgnorePro = True 가 원래.
        ZWrite On // <- Off였는데 윗쪽 창문들이 안 보이는 문제 때문에 내가 바꿨음
        Blend SrcAlpha OneMinusSrcAlpha
    
        SubShader {
            Pass {
                GLSLPROGRAM
                varying mediump vec2 uv;
            
                #ifdef VERTEX
                uniform mediump vec4 _MainTex_ST;
                void main() {
                    gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                    uv = gl_MultiTexCoord0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                }
                #endif
            
                #ifdef FRAGMENT
                uniform lowp sampler2D _MainTex;
                uniform lowp vec4 _Color;
                void main() {
                    gl_FragColor = texture2D(_MainTex, uv) * _Color;
                }
                #endif     
                ENDGLSL
            }
        }
    
        SubShader {
            Pass {
                SetTexture[_MainTex] {Combine texture * constant ConstantColor[_Color]}
            }
        }
    }
 
}