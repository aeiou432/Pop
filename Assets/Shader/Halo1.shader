﻿Shader "Custom/SpriteGlow1"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _GlowStrength ("Glow Strength", Float) = 0.5
		_GlowLowThreshold ("Glow Low Threshold", Float) = 0.3
        _GlowSize ("Glow Size", Float) = 1.0
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags {"Queue"="Transparent"}
		// No culling or depth
		Cull Off ZWrite Off ZTest Always Blend SrcAlpha OneMinusSrcAlpha

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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            float _GlowStrength;
			float _GlowLowThreshold;
            float _GlowSize;
            fixed4 _GlowColor;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv);
                if (c.r > 0.99 && c.g > 0.99 && c.b > 0.99) {
                    return c;
                }

                _GlowStrength /= 50.0;
				_GlowLowThreshold /= 50.0;
				float minV = 3;
				for (int x = -2; x <= 2; x++) 
					for (int y = -2; y <= 2; y++) {
						fixed4 color = tex2D(_MainTex, i.uv + fixed2(x * _GlowSize * _MainTex_TexelSize.x, y * _GlowSize * _MainTex_TexelSize.y));
						if (color.r < 0.99 || color.g < 0.99 || color.b < 0.99)
							continue;
						else {
							uint tmp = max(abs(x), abs(y));
							minV = min(minV, tmp);
						}
					}
				
				float a = 0;
				if (minV != 3) {
					a = (3.0 - minV) / 3.0 * _GlowStrength + _GlowLowThreshold;
				}
				/*float a = 0;
                for (int x=-15; x<=15; x++)
                    for (int y=-15; y<=15; y++){
						if (abs(x) + abs(y) > 15) {
							continue;
						}
                        fixed4 color = tex2D(_MainTex, i.uv + fixed2(x * _GlowSize * _MainTex_TexelSize.x, y * _GlowSize * _MainTex_TexelSize.y));
                        if (color.r < 0.99 || color.g < 0.99 || color.b < 0.99)
                            continue;
                        else
                            a += color.a * _GlowStrength;
                        }
                            */
                a = min(a, 1);
                return fixed4(_GlowColor.rgb, a);
            }
			ENDCG
		}
	}
}
