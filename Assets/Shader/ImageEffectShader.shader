Shader "Hidden/ImageEffectShader"
{
    Properties
    {
		_MainTex("Base (RGB)", 2D) = "white" {}
		_GlowStrength("Glow Strength", Float) = 0.5
		_GlowSize("Glow Size", Float) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _MainTex_TexelSize;
			fixed _GlowStrength;
			fixed _GlowSize;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                return o;
            }

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				if (c.r != 0 || c.g != 0 || c.b != 0) {
					return c;
				}
				/*const float gaussian[25] = {
					0.0030, 0.0133, 0.0219, 0.0133, 0.0030,
					0.0133, 0.0596, 0.0983, 0.0596, 0.0133,
					0.0219, 0.0983, 0.1621, 0.0983, 0.0219,
					0.0133, 0.0596, 0.0983, 0.0596, 0.0133,
					0.0030, 0.0133, 0.0219, 0.0133, 0.0030
				};*/
				const float gaussian[25] = {
					0.04, 0.04, 0.04, 0.04, 0.04,
					0.04, 0.04, 0.04, 0.04, 0.04,
					0.04, 0.04, 0.04, 0.04, 0.04,
					0.04, 0.04, 0.04, 0.04, 0.04,
					0.04, 0.04, 0.04, 0.04, 0.04
				};
				c = fixed4(0,0,0,0);
				float a = 0;
				_GlowStrength /= 50.0;
				for (int x = -5; x <= 5; x++) {
					for (int y = -5; y <= 5; y++) {
						fixed4 color = tex2D(_MainTex, i.uv + fixed2(x * _GlowSize * _MainTex_TexelSize.x, y * _GlowSize * _MainTex_TexelSize.y));
						c += color;
						if (color.r != 0 && color.g != 0 && color.b != 0) {
							a += color.a / 121 * _GlowStrength;
						}
					}
				}
				
				return fixed4(lerp(fixed3(0, 0, 0), c.rgb, a), 1);
			}
            ENDCG
        }
    }
}
