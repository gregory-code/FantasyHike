Shader "Unlit/Transition_01"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Transition ("Transition", Range(0, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Transition;

            float smoothNoise(float2 uv)
            {
                // Generate smoother, continuous noise using sine and UV warping
                float n = sin(uv.x * 10.0) * cos(uv.y * 10.0);
                n += sin(uv.x * 5.0 + uv.y * 7.0) * 0.5;
                n += sin(uv.x * 15.0 - uv.y * 8.0) * 0.25;
                return n * 0.5 + 0.5; // Normalize to [0, 1]
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Scale UVs for larger puddle-like shapes
                float2 scaledUV = i.uv * 4.0;

                // Generate smooth blotchy noise
                float blotchiness = smoothNoise(scaledUV);

                // Map blotchiness to a threshold based on transition
                float blotch = smoothstep(_Transition - 0.1, _Transition + 0.1, blotchiness);

                // Blend alpha: fully opaque at 0, fully transparent at 1
                float alpha = 1.0 - _Transition;

                // Apply blotchy mask to the alpha
                alpha *= blotch;

                // Final output: black with alpha blending
                return fixed4(0.0, 0.0, 0.0, alpha);
            }
            ENDCG
        }
    }
}
