Shader "Custom/AverageBlur" {
    Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurSize ("Blur Size", Float) = 1.0
		_BlurStrength("Blur Strength", Float) = 1.0
    }
		SubShader{
			CGINCLUDE

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_TexelSize;
			float _BlurSize;
			float _BlurStrength;

			struct v2f {
				float4 pos : SV_POSITION;
				half2 uv[5]: TEXCOORD0;
			};

			v2f vertBlurVertical(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				half2 uv = v.texcoord;

				o.uv[0] = uv;
				o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
				o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
				o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
				o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;

				return o;
			}

			v2f vertBlurHorizontal(appdata_img v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				half2 uv = v.texcoord;

				o.uv[0] = uv;
				o.uv[1] = uv + float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
				o.uv[2] = uv - float2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
				o.uv[3] = uv + float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
				o.uv[4] = uv - float2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;

				return o;
			}

			fixed4 fragBlur(v2f i) : SV_Target {
				float weight[3] = {0.2, 0.2, 0.2};

				fixed3 color = tex2D(_MainTex, i.uv[0]);
				fixed sum = color.g * weight[0];

				for (int it = 1; it < 3; it++) {
					sum += tex2D(_MainTex, i.uv[it * 2 - 1]).g * weight[it];
					sum += tex2D(_MainTex, i.uv[it * 2]).g * weight[it];
				}
				return fixed4(color.r, sum * _BlurStrength + color.g, color.b, 1);
			}
			ENDCG

			ZTest Always Cull Off ZWrite Off

			Pass {
				NAME "GAUSSIAN_BLUR_VERTICAL"

				CGPROGRAM

				#pragma vertex vertBlurVertical
				#pragma fragment fragBlur

				ENDCG
			}

			Pass {
				NAME "GAUSSIAN_BLUR_HORIZONTAL"

				CGPROGRAM

				#pragma vertex vertBlurHorizontal
				#pragma fragment fragBlur

				ENDCG
			}
		}
		FallBack "Diffuse"
}
