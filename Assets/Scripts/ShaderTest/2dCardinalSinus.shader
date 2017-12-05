//http://docs.unity3d.com/Manual/SL-SurfaceShaders.html
//http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html
//http://docs.unity3d.com/Manual/SL-SurfaceShaderLightingExamples.html
//http://docs.unity3d.com/Manual/SL-VertexProgramInputs.html

Shader "Custom/2dCardinalSinus" {
	Properties {
		_MainTex ("Diffuse RGBA", 2D) = "white" {}
		_BumpMap ("Normal map", 2D) = "bump" {} //couleur "bump" par défaut


	}
	
	Category{
	
	SubShader {

		CGPROGRAM
		//ici, on veut utiliser le modèle d'éclairage SimpleSpecular associé à la "fonction LightingSimpleSpecular", 
		//la fonction "surf" en fonction de surface et la fonction "vertexFunction" en vertex shader
		#pragma surface surf SimpleSpecular vertex:vertexFunction 
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;
		float3 _ImpactPos[5];
		float _ImpactMag[5];
		float _ImpactTimer[5];
		

		
		 //Structure d'entrée/sortie du vertex shader, alimentée par la carte graphique.
		struct Prog2Vertex {
	        float4 vertex : POSITION;
	        float4 tangent : TANGENT;
	        float3 normal : NORMAL;
	        float4 texcoord : TEXCOORD0;
	        float4 texcoord1 : TEXCOORD1;
	        fixed4 color : COLOR;
	    };
	    
	    //Structure d'entrée/sortie du surface shader.
		struct Input {
			float3 viewDir; //will contain view direction, for computing Parallax effects, rim lighting etc.
			float2 uv_MainTex:TEXCOORD0; //Premier niveau d'UV. Doit etre de la forme uv_<nom d'une property existante>. Incompréhensible.
			float2 uv2_MainTex:TEXCOORD1;//Second niveau d'UV (généralement utilisé pour la lightmap). Doit etre de la forme uv2_<nom d'une property>
			float3 worldPos; //Position dans le monde
			float3 worldRefl; //Vecteur de reflection dans le monde
			float3 worldNormal; //Normale dans le monde. INTERNAL_DATA permet de modifier ce paramètre dans la fonction surf.
			INTERNAL_DATA		//Obligatoire pour utiliser une normal map. Ne pas oublier d'écrire une valeur par défaut sinon!
		};
			
		//Fonction d'éclairage custom, qui sera lancée pour chaque lumière et gère le spéculaire.
		half4 LightingSimpleSpecular (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 h = normalize (normalize(lightDir) +viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			float nh = max (0, dot (s.Normal, h));
			float spec = s.Gloss*pow (nh, s.Specular);
			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff +  _LightColor0.rgb*spec) * (atten * 2);
			c.a = s.Alpha;
	
			return c;
		}
			
			
		//Fonction de vertex shader. Optionnelle car le surface shader embarque sa propre fonction interne
		//On peut cependant modifier les données de la structure Prog2Vertex 
		//(mais elles ont déja une valeur par défaut)
		void vertexFunction (inout Prog2Vertex v) 
		{
			float Pi = 3.14159265359;
			for (int i = 0; i < 5; i++)
			{
				if (_ImpactTimer[i] != 0)
				{
					float slimeRay = 1;
					float distance = length(v.vertex.xyz - _ImpactPos[i]);
					float VertexforceMag = slimeRay - distance;
					float3 direction = v.vertex.xyz / length(v.vertex);
					float x = (_ImpactTimer[i] * 10 - Pi);

					v.vertex.xyz += -direction * sin(x) / x * VertexforceMag;
				}
			}
		
			//v.vertex.xyz += v.vertex.xyz * sin(_ImpactTimer + Pi) ;
		}
		
		//Fonction principale du surface shader
		//Il faut remplir les paramètres albedo, normal, specular, gloss, emission
		void surf (Input i, inout SurfaceOutput o) 
		{
			o.Albedo = tex2D(_MainTex, i.uv_MainTex);
			o.Normal = UnpackNormal(tex2D(_BumpMap, i.uv_MainTex));
			//o.Albedo=float4(0,1,0,1);
		}
		
		ENDCG
	}

Fallback "VertexLit"
}
}
