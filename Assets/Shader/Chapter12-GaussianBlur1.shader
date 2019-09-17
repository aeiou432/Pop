Shader "Unity Shaders Book/Chapter 12/Gaussian Blur1" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
		_BlurStrength("Blur Strength", Float) = 1.0
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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			fixed4 _MainTex_TexelSize;
			float _BlurSize;
			float _BlurStrength;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed3 color = tex2D(_MainTex, i.uv);
				fixed3 sum = color;
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-2 * _BlurSize * _MainTex_TexelSize.x, -2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-2 * _BlurSize * _MainTex_TexelSize.x, -1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-2 * _BlurSize * _MainTex_TexelSize.x, 0 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-2 * _BlurSize * _MainTex_TexelSize.x, 1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-2 * _BlurSize * _MainTex_TexelSize.x, 2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-1 * _BlurSize * _MainTex_TexelSize.x, -2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-1 * _BlurSize * _MainTex_TexelSize.x, -1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-1 * _BlurSize * _MainTex_TexelSize.x, 0 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-1 * _BlurSize * _MainTex_TexelSize.x, 1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(-1 * _BlurSize * _MainTex_TexelSize.x, 2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(0 * _BlurSize * _MainTex_TexelSize.x, -2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(0 * _BlurSize * _MainTex_TexelSize.x, -1 * _BlurSize * _MainTex_TexelSize.y));

				sum = sum + tex2D(_MainTex, i.uv + fixed2(0 * _BlurSize * _MainTex_TexelSize.x, 1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(0 * _BlurSize * _MainTex_TexelSize.x, 2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(1 * _BlurSize * _MainTex_TexelSize.x, -2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(1 * _BlurSize * _MainTex_TexelSize.x, -1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(1 * _BlurSize * _MainTex_TexelSize.x, 0 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(1 * _BlurSize * _MainTex_TexelSize.x, 1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(1 * _BlurSize * _MainTex_TexelSize.x, 2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(2 * _BlurSize * _MainTex_TexelSize.x, -2 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(2 * _BlurSize * _MainTex_TexelSize.x, -1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(2 * _BlurSize * _MainTex_TexelSize.x, 0 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(2 * _BlurSize * _MainTex_TexelSize.x, 1 * _BlurSize * _MainTex_TexelSize.y));
				sum = sum + tex2D(_MainTex, i.uv + fixed2(2 * _BlurSize * _MainTex_TexelSize.x, 2 * _BlurSize * _MainTex_TexelSize.y));
				
				/*for (int x = -2; x <= 2; x++) {
					for (int y = -2; y <= 2; y++) {
						fixed3 c = tex2D(_MainTex, i.uv + fixed2(x * _BlurSize * _MainTex_TexelSize.x, y * _BlurSize * _MainTex_TexelSize.y));
						sum = sum + c;
					}
				}*/
				sum = sum / 25 * _BlurStrength;
				return fixed4(sum + color , 1);
			}
			ENDCG
		}
	}
}
