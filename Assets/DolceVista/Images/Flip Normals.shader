Shader "Custom/Flip Normals" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		SubShader{
		Lighting Off

		Tags{ "RenderType" = "Opaque" }

		Cull Front

		CGPROGRAM

#pragma surface surf Unlit 
#include "UnityCG.cginc" 

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
		half4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}
		sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float4 color : COLOR;
	};


	void vert(inout appdata_full v)
	{
		v.normal.xyz = v.normal * -1;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		fixed3 result = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = result.rgb;
		o.Alpha = 1;
	}

	ENDCG

	}

		Fallback "Diffuse"
}