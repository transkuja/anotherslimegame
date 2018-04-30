Shader "Custom/PlayerBodyShaderOnTop" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_ColorFade("Use Color Fade ? (0 = no, 1 or more = yes)", Int) = 0
		_MainTex ("Main Texture", 2D) = "white" {}
		_Emissive("Emissive", 2D) = "white" {}
		_EmissiveColor("Emissive Color", Color) = (0, 0, 0, 1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector" = "True" "RenderQueue" = "Overlay" }
		LOD 200
		ZWrite On
		ZTest On

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _Emissive;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
		};

		float _FaceType;
		float _FaceEmotion;
		float _ColorFade;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed4 _EmissiveColor;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_CBUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_CBUFFER_END

		float3 HUEtoRGB(in float H)
		{
			float R = abs(H * 6 - 3) - 1;
			float G = 2 - abs(H * 6 - 2);
			float B = 2 - abs(H * 6 - 4);
			return saturate(float3(R, G, B));
		}

		float Epsilon = 1e-10;

		float3 RGBtoHCV(in float3 RGB)
		{
			// Based on work by Sam Hocevar and Emil Persson
			float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
			float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
			float C = Q.x - min(Q.w, Q.y);
			float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
			return float3(H, C, Q.x);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			float4 col = _Color;
			if (_ColorFade >= 1)
			{
				col.rgb = float3(1, 0, 0);
				col.rgb = RGBtoHCV(col.rgb);
				if(_ColorFade < 2.0f)
					col.r += _Time.x*2.0;
				else
					col.r += _Time.y*0.75f - localPos.y + sin(localPos.x/4.0f * _Time.x);
				while (col.r > 1.0)
					col.r -= 1.0;
				col.rgb = HUEtoRGB(col.r);
			}

			o.Albedo = tex2D(_MainTex, IN.uv_MainTex) * col;

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = col.a;
			o.Emission = tex2D(_Emissive, IN.uv_MainTex).rgb * _EmissiveColor;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
