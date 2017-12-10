Shader "Transparent/BlobbyShaderTransparentLit"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_EmissiveColor("Emissive", Color) = (0,0,0,0)
		_MainTex("Texture", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_DisplaceTex("Displacement Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 0.1
		_a1("a1", Range(0,50)) = 1
		_a2("a2", Range(0,50)) = 1
	}
		

	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		Pass{
			ColorMask 0
		}
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert alpha:blend vertex:vert
#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _DisplaceTex;
		fixed4 _Color;
		fixed4 _EmissiveColor;
		float _Magnitude;
		float _a1;
		float _a2;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float4 color : COLOR;
		};

		void vert(inout appdata_full v)
		{
			//v.vertex.x += sign(v.vertex.x) * sin(_Time.w *  0.5) / _a1;
			//v.vertex.y += sign(v.vertex.y) * cos(_Time.w *  0.5) / _a2;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Albedo *= IN.color.rgb;
			o.Emission = _EmissiveColor;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}

		ENDCG
	}
		Fallback "VertexLit"
}
