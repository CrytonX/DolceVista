Shader "AVProVideo/Lit/Diffuse (texture+color+fog+stereo support)" 
{
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}

		[KeywordEnum(None, Top_Bottom, Left_Right)] Stereo("Stereo Mode", Float) = 0
		[Toggle(STEREO_DEBUG)] _StereoDebug("Stereo Debug Tinting", Float) = 0
	}

	SubShader
	{
		Tags { "Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Geometry" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:VertexFunction
		#pragma multi_compile MONOSCOPIC STEREO_TOP_BOTTOM STEREO_LEFT_RIGHT
		#pragma multi_compile __ STEREO_DEBUG

		#include "AVProVideo.cginc"

		uniform sampler2D _MainTex;
		uniform fixed4 _Color;
		uniform float3 _cameraPosition;

		struct Input 
		{
			float2 uv_MainTex;
#if STEREO_DEBUG
			float4 color;
#endif
		};

		void VertexFunction(inout appdata_full v)
		{
#if STEREO_TOP_BOTTOM | STEREO_LEFT_RIGHT
			float4 scaleOffset = GetStereoScaleOffset(IsStereoEyeLeft(_cameraPosition, UNITY_MATRIX_V[0].xyz));
			v.texcoord.xy *= scaleOffset.xy;
			v.texcoord.xy += scaleOffset.zw;
#endif
#if STEREO_DEBUG
			v.color = GetStereoDebugTint(IsStereoEyeLeft(_cameraPosition, UNITY_MATRIX_V[0].xyz));
#endif
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

#if STEREO_DEBUG
			c *= IN.color;
#endif
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/VertexLit"
}
