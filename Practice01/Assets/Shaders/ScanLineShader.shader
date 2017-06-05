Shader "Custom/ScanLineShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ScanningLineColor("Scanning Line Color", Color) = (1,1,1,1)
		_ScanningLineSpeed("Scanning Line Speed(m/s)", Range(0.05,5)) = 0.5
		_ScanningLineInterval("Scanning Line Interval(m)", Range(1,10)) = 2
		_ScanningLineWidth("Scanning Line Width(m)", Range(0, 5)) = 0.02
		_Mask("Mask", Range(0,1)) = 1
	}


	CGINCLUDE
	#include "UnityCG.cginc"

	fixed4 _Color;

	float _ScanningLineSpeed;
	float _ScanningLineInterval;
	float _ScanningLineWidth;
	fixed4 _ScanningLineColor;

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// 走査線描画
		float dist = frac((length(i.worldPos) - _Time.y * _ScanningLineSpeed) / _ScanningLineInterval);
		float fixedWidth = _ScanningLineWidth / _ScanningLineInterval;
		if (0 <= dist && dist <= fixedWidth) {
			fixed4 c = _ScanningLineColor;
			float alpha = dist / fixedWidth;

			return c * alpha + _Color * (1 - alpha);
		}

		// 背景
		return _Color;
	}

	ENDCG
	SubShader {
		Tags{
			"RenderType" = "Opaque"
			"Queue" = "Geometry-1"
		}
		LOD 200

		// Second Pas. 走査線描画用シェーダー定義
		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend SrcAlpha OneMinusSrcAlpha

			Stencil
			{
				Ref[_Mask]
				Comp NotEqual
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 5.0
			#pragma only_renderers d3d11
			ENDCG
		}
	}
	FallBack "Diffuse"
}
