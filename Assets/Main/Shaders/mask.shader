Shader "UI/Mask" {
    Properties {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MaskTex ("Mask Texture", 2D) = "white" {}
    }

    SubShader {
        Tags { "RenderPipeline"="UniversalPipeline" }

        Pass {
            Stencil {
                Ref 1
                Comp always
                Pass replace
                Fail keep
                ZFail keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Sample the mask texture
                fixed4 mask = tex2D(_MaskTex, i.uv);

                // Multiply the main texture by the mask alpha
                col.a *= mask.a;

                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}
