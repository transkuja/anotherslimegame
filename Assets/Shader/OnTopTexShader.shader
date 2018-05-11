Shader "Custom/OnTopTexShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader{
			Tags { "RenderType" = "Transparent" }
			LOD 200
			ZWrite Off
			ZTest On

			CGPROGRAM
			#pragma surface surf Standard fullforwardshadows
			float4 _Color;
			sampler2D _MainTex;

			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutputStandard o) {
				float4 main = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = main.rgb;
				o.Alpha = main.a;
			}
		ENDCG
	}
}
