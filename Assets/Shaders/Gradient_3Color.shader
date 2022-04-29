// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient_3Color" {
    Properties {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _ColorTop ("Top Color", Color) = (1,1,1,1)
        _ColorMid ("Mid Color", Color) = (1,1,1,1)
        _ColorBot ("Bot Color", Color) = (1,1,1,1)
        _Middle ("Middle", Range(0.001, 0.999)) = 1
    }
    
    SubShader {
        Tags {"Queue"="Background" "IgnoreProjector"="True"}
        LOD 100
        
        ZWrite On
        
        Pass {
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"
            
            fixed4 _ColorTop;
            fixed4 _ColorMid;
            fixed4 _ColorBot;
            float  _Middle;
            
            struct v2f {
                float4 pos : SV_POSITION;
                float4 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            
            v2f vert (appdata_full v) {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                float2x2 rotationMatrix = float2x2( 0, 1, 0, 0);
                v.texcoord.xy = mul ( v.texcoord.xy, rotationMatrix );
                o.texcoord = v.texcoord;
                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }
            
            fixed4 frag (v2f i) : COLOR {
                fixed4 c = lerp(_ColorBot, _ColorMid, i.texcoord.y / _Middle) * step(i.texcoord.y, _Middle);
                c += lerp(_ColorMid, _ColorTop, (i.texcoord.y - _Middle) / (1 - _Middle)) * step(_Middle, i.texcoord.y);
                UNITY_APPLY_FOG(i.fogCoord, c);
                c.a = 1;
                return c;
            }
            ENDCG
        }
    }
}