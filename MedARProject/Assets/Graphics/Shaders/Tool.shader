//Stella

//Renders tool on top of spine and adjusts alpha according to depth
Shader "Custom/Tool"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _AlphaOutside("_AlphaOutside", Range(0,1)) = 0.3
        _AlphaInside("_AlphaInside", Range(0,1)) = 0.5
    }
        SubShader
        {
            Tags  {"Queue" = "Transparent" "RenderType" = "Transparent" "disablebatching" = "True" "RenderInTwoD"="True"}
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

            half _AlphaOutside;
            half _AlphaInside;

            float _ZEntryPoint;

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

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // Albedo comes from a texture tinted by color
                fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;

                // Metallic and smoothness come from slider variables
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;

                //Tool is rendered less transparent inside the spine, 
                //as in this case the physical tool is no longer visible
                o.Alpha = _AlphaOutside;
                if (IN.localPos.z > _ZEntryPoint)
                    o.Alpha = _AlphaInside;
            }
            ENDCG
        }
            FallBack "Diffuse"
}
