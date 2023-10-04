Shader "Custom/WavingFlagWithRandomness"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Speed", Range(0, 10)) = 1
        _Amplitude ("Amplitude", Range(0, 1)) = 0.1
        _Frequency ("Frequency", Range(0, 10)) = 1
        _Gravity ("Gravity", Range(0, 10)) = 1
        _Randomness ("Randomness", Range(0, 1)) = 0.5
    }
 
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
        #pragma target 3.0
 
        sampler2D _MainTex;
        float _Speed;
        float _Amplitude;
        float _Frequency;
        float _Gravity;
        float _Randomness;
 
        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };
 
        void vert(inout appdata_full v)
        {
            float angle = sin((_Time.y + v.vertex.x * _Randomness) * _Speed + v.vertex.y * _Frequency) * _Amplitude;
            v.vertex.y += angle;
            
            float gravity = _Gravity * (v.vertex.x * v.vertex.x);
            v.vertex.x -= gravity;
            v.vertex.y -= gravity;
        }
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
