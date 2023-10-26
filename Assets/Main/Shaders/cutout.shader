Shader "Custom/SimpleMaskShader"
 {
     Properties
     {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        _MaskTex ("Mask", 2D) = "white" {}
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.2
     }
 
     SubShader
     {
         Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
         LOD 300
         
         CGPROGRAM
         #pragma surface surf Lambert alphatest:_Cutoff
 
         sampler2D _MainTex;
         sampler2D _MaskTex;
 
         struct Input
         {
             float2 uv_MainTex;
             float2 uv_MaskTex;
         };
 
         void surf (Input IN, inout SurfaceOutput o)
         {
             o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
             o.Alpha = tex2D(_MaskTex, IN.uv_MaskTex).a;
         }
         ENDCG
     }
 
     FallBack "Transparent/Cutout/Diffuse"
 }