

Shader "Unlit/Cutout" {
	Properties{
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB) Alpha (A)", 2D) = "Green" {}
	_Cutoff("Base Alpha cutoff", Range(0, 1.01)) = 1.01
		_Multiplier("Color Multiplier",Range(1,10)) = 1.0
	}
		SubShader{
		Tags{ "Queue" = "AlphaTest" "RenderType" = "TransparentCutout" }
		LOD 200
		Cull Off Lighting On

		CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff

		sampler2D _MainTex;
	fixed4 _Color;
	float _Multiplier;

	struct Input {
		float2 uv_MainTex;
	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex)  * _Color * _Multiplier;
		o.Albedo = c.rgb;
		o.Alpha = c.a / _Multiplier;
	}

	ENDCG
	}

		Fallback "Transparent/Cutout/VertexLit"
}