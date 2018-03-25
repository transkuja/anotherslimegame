Shader "Custom/OnTopShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Transparent" }
			LOD 200
			ZWrite Off
			ZTest On

			CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
			float4 _Color;
		struct Input {
		float2 uv_MainTex;
	};

		void surf(Input IN, inout SurfaceOutputStandard o) {
		o.Albedo = _Color.rgb;
		o.Alpha = _Color.a;
	}
	ENDCG
	}
}
