Shader "Costum/FoxVision"
{
	Properties
	{
		_MainTex (" Main Texture", 2D) = "white" {}
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

			v2f vert (appdata v) // runs for every vertex on the associated object
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float4 boxBlur(sampler2D texel, float2 uv, float4 size) 
			{
				float4 result = tex2D(texel, uv + float2(-size.x, size.y)) + tex2D(texel, uv + float2(0, size.y)) + tex2D(texel, uv + float2(size.x, size.y)) +
					tex2D(texel, uv + float2(-size.x, 0)) + tex2D(texel, uv + float2(0, 0)) + tex2D(texel, uv + float2(size.x, 0)) +
					tex2D(texel, uv + float2(-size.x, -size.y)) + tex2D(texel, uv + float2(0, -size.y)) + tex2D(texel, uv + float2(size.x, -size.y));

				return result / 9;
			}


			fixed4 frag (v2f i) : SV_Target // runs for every pixel on the associated object, no phusical changes.
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				


			// dogs cant see red, the same aplies for foxes. put red to zero and make green and blue more intense
			col = boxBlur(_MainTex, i.uv, _MainTex_TexelSize);
			col.r =col.r-1.0f;
			col.g = col.g;
			col.b = col.b;

				return col;
			}
			ENDCG
		}
	}
}

// modified by: Panagiotis Katsiadramis 13/02/18
// modified by: Panagiotis Katsiadramis 07/03/18
