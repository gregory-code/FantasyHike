Shader "Unlit/Transition_03"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Transition ("Transition", Range(0, 1)) = 0
        _Feather ("Feather Amount", Range(0, 0.2)) = 0.05
        _WarpIntensity ("Warp Intensity", Range(0, 0.2)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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
            float _Feather;
            float _WarpIntensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 col = tex2D(_MainTex, i.uv);

                // Generate procedural warp using sine and cosine waves
                float warp = sin(i.uv.y * 20.0) * _WarpIntensity + cos(i.uv.x * 15.0) * _WarpIntensity;

                // Calculate the warped wipe threshold with the edge warp
                float uvWipe = i.uv.x + warp;

                // Apply the transition threshold with feathering
                float alpha = smoothstep(_Transition - _Feather, _Transition + _Feather, uvWipe);

                // Start with black color and use the alpha for transparency
                fixed4 resultColor = fixed4(0.0, 0.0, 0.0, alpha); // Black background with transition alpha

                // Ensure the blending happens only based on alpha, not introducing any unintended white
                resultColor.rgb = lerp(resultColor.rgb, col.rgb, alpha); // Blend texture based on alpha

                return fixed4(0.0, 0.0, 0.0, alpha);
            }
            ENDCG
        }
    }
}
