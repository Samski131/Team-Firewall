Shader "Costum/Transformation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	_EffectTexture("Effect Texture", 2D) = "white" {}
	_EffectStrength("Effect Strength", Range(0,0.1))=1
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _EffectTexture;
			float _EffectStrength;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 newUvCordinates = float2(i.uv.x + _Time.x * 2, i.uv.y + _Time.x * 2);
				// save the values of the texture for each pixel 
				float2 effect = tex2D(_EffectTexture, newUvCordinates).xy;
				// set the texture in the place of the screen 
				effect = ((effect * 2) -1) * _EffectStrength;
				// use the values of the saved texture above to displace the main texture of the camera.
				float4 col = tex2D(_MainTex, i.uv + effect);

				return col;
			}
			ENDCG
		}
	}
}
// Created by: Panagiotis Katsiadramis 18/02/18