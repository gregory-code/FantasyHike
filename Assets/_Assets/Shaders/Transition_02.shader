Shader "Unlit/Transition_02"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Transition ("Transition", Range(0, 1)) = 0
        _Feather ("Feather Amount", Range(0, 0.2)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
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

                // Calculate distance from the center of the screen
                float dist = distance(i.uv, float2(0.5, 0.5)); // Calculate distance to the center

                // Create a radial wipe effect (a growing circle)
                float alpha = smoothstep(_Transition - _Feather, _Transition + _Feather, dist);

                // Ensure black background and alpha-controlled blending
                fixed4 resultColor = fixed4(0.0, 0.0, 0.0, alpha); // Black background with transition alpha

                // Blend the texture based on the transition alpha (ensure no white values appear)
                resultColor.rgb = lerp(resultColor.rgb, col.rgb, alpha); // Blend texture with black background

                return fixed4(0.0, 0.0, 0.0, alpha);
            }
            ENDCG
        }
    }
}
