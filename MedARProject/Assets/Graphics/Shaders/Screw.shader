Shader "Custom/Screw"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _YAlphaMin("AlphaMin",  Range(0,1)) = 0.4
        _YAlphaMax("AlphaMax",  Range(0,1)) = 0.8
        _YDiffMax("MaxYDiff",  Range(-20,20)) = 0.0
    }
    SubShader
    {
        Tags  {"Queue" = "Transparent" "RenderType" = "Transparent" "disablebatching" = "True"}
        LOD 200
        ZTest Always

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha:fade vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _UseEntryPoint;
        float _YEntryPoint;

        half _YAlphaMin;
        half _YAlphaMax;
        half _YDiffMax;


        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o) 
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            if (_UseEntryPoint > 0 && IN.localPos.y < _YEntryPoint)
                o.Alpha = _YAlphaMax - ((_YEntryPoint - IN.localPos.y) / _YDiffMax) * (_YAlphaMax - _YAlphaMin);

        }
        ENDCG
    }
    FallBack "Diffuse"
}
