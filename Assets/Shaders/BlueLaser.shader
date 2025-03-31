Shader "Unlit/BlueLaser"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Laser Color", Color) = (0.2, 0.5, 1, 1) // Blue
        _GlowIntensity ("Glow Intensity", Float) = 3.0
        _Speed ("Scroll Speed", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend One One // Additive blending for a glowing effect
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _GlowIntensity;
            float _Speed;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv.x += _Time.y * _Speed; // Horizontal scrolling effect
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    float2 centeredUV = abs(i.uv - 0.5) * 2.0; // Makes it symmetrical
    float gradient = exp(-4.0 * centeredUV.y); // Exponential falloff for a beam shape

    // Force the laser to be a strong blue color
    fixed4 finalColor = fixed4(0.0, 0.5, 1.0, 1.0) * gradient * _GlowIntensity;
    finalColor.a = gradient; // Fade at edges

    return finalColor;
}
            ENDCG
        }
    }
}
