// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Transparent/BlobbyShaderTransparentLit"
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_DisplaceTex("Displacement Texture", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,0.1)) = 0.1
			_a1("a1", Range(0,50)) = 1
			_a2("a2", Range(0,50)) = 1
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _DisplaceTex;
		fixed4 _Color;
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
			//float phase = _Time * 20.0;
			v.vertex.x += sign(v.vertex.x) * sin(_Time.w *  0.5) / _a1;
			v.vertex.y += sign(v.vertex.y) * cos(_Time.w *  0.5) / _a2;
			//float offset = (v.vertex.x + (v.vertex.z * _a1)) * _a2;
			//v.vertex.z = sin(phase + offset) * _a3;
		}

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Albedo *= IN.color.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
		}

		ENDCG
		//Pass
		//{
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	// make fog work
		//	#pragma multi_compile_fog
		//	
		//	#include "UnityCG.cginc"

		//	struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//	};

		//	struct v2f
		//	{
		//		float2 uv : TEXCOORD0;
		//		UNITY_FOG_COORDS(1)
		//		float4 vertex : SV_POSITION;
		//	};

		//	sampler2D _MainTex;
		//	float4 _MainTex_ST;
		//	sampler2D _DisplaceTex;
		//	float _Magnitude;
		//	float _a1;

		//	float _a2;

		//	
		//	v2f vert (appdata v)
		//	{
		//		v2f o;
		//		//float phase = _Time * 20.0;
		//		v.vertex.x += sign(v.vertex.x) * sin(_Time.w *  0.5) / _a1;
		//		v.vertex.y += sign(v.vertex.y) * cos(_Time.w *  0.5) / _a2;
		//		//float offset = (v.vertex.x + (v.vertex.z * _a1)) * _a2;
		//		//v.vertex.z = sin(phase + offset) * _a3;

		//		o.vertex = UnityObjectToClipPos(v.vertex);
		//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//		UNITY_TRANSFER_FOG(o,o.vertex);
		//		return o;
		//	}
		//	
		//	fixed4 frag (v2f i) : SV_Target
		//	{
		//		//float2 distuv = float2(i.uv.x + _Time.x * 2, i.uv.y + _Time.x * 2);

		//		//float2 disp = tex2D(_DisplaceTex, distuv).xy;
		//		//disp = ((disp * 2) - 1) * _Magnitude;

		//		//// sample the texture
		//		//fixed4 col = tex2D(_MainTex, i.uv + disp);
		//		fixed4 col = tex2D(_MainTex, i.uv);
		//		// apply fog
		//		UNITY_APPLY_FOG(i.fogCoord, col);
		//		return col;
		//	}
		//	ENDCG
		//}
	}
	Fallback "VertexLit"
}
