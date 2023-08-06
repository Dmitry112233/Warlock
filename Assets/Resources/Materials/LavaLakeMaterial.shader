// LavaLakeShader.shader

Shader "Custom/LavaLakeShader" {
    Properties {
        _MainTex ("Lava Texture", 2D) = "white" {}
        _DistortionStrength ("Distortion Strength", Range(0.0, 1.0)) = 0.1
        _LavaColor ("Lava Color", Color) = (1, 0.4, 0, 1)
        _Speed ("Speed", Range(0.0000001, 10.0)) = 1.0
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float _DistortionStrength;
        fixed4 _LavaColor;
        float _Speed;

        // Function to apply circular motion to UV coordinates
        float2 CircularMotion(float2 uv, float time, float speed) {
            float2 center = 0.5;
            float radius = 0.3;
            uv -= center;
            float angle = time * speed;
            float s = sin(angle);
            float c = cos(angle);
            uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
            uv += center;
            uv *= radius;
            uv += center;
            return uv;
        }


        void surf (Input IN, inout SurfaceOutput o) {
            float2 offset = CircularMotion(IN.uv_MainTex, _Time.y, _Speed) * _DistortionStrength;
            fixed4 lavaColor = tex2D(_MainTex, offset);
            o.Albedo = lavaColor.rgb * _LavaColor.rgb;
            o.Alpha = lavaColor.a;
        }

        // Vertex function to pass UV coordinates to the surface function
        void vert(inout appdata_full v) {
            v.texcoord = v.texcoord * _DistortionStrength;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
