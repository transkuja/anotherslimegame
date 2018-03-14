
Shader "Hidden/cloudsTest"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color1("_Color1", COLOR) = (1,1,1,1)
		_Tiling("tiling",float) = 0.001
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "ClassicNoise3D.hlsl"
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos :NORMAL;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			float _Tiling;
			float4 _Color1;
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors

				float alpha = cnoise(i.worldPos.xyz*_Tiling);
				return float4(cnoise(i.worldPos.xyz*_Tiling)*_Color1.xyz, alpha);
				//return float4(i.worldPos.xyz,1);
				//return float4(1, 0, 0, 0);
				//return col;
			}
			ENDCG
		}
	}
}
