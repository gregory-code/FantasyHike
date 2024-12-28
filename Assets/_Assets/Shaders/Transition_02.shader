Shader "Unlit/Transition_02"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "black" {}
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

                // Generate procedural warp using sine waves
                float warp = sin(i.uv.y * 20.0) * _WarpIntensity + cos(i.uv.x * 15.0) * _WarpIntensity;

                // Calculate the warped wipe threshold
                float uvWipe = i.uv.x + warp;
                float alpha = smoothstep(_Transition - _Feather, _Transition + _Feather, uvWipe);

                // Ensure the transition starts as black
                fixed4 resultColor = fixed4(0.0, 0.0, 0.0, alpha); // Set color to black with alpha

                // Blend the black background to the texture based on alpha
                resultColor.rgb = lerp(resultColor.rgb, col.rgb, alpha); // Blend black to the texture color

                return resultColor;
            }
            ENDCG
        }
    }
}
