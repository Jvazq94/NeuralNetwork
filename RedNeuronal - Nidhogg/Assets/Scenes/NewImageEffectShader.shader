Shader "Custom/NewImageEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_intensity("intensidad",range(0,100)) = 0
		_color("color",range(0,1)) = 0
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
			float _intensity;
			float _color;
			fixed4 frag (v2f i) : SV_Target
			{
				v2f temp = i;
				
				//temp.uv.y += sin(sin( 0.5f) * 10000 *i.uv.x)*0.005f*_Time ;

				//uvtemp.x += sin(_Time * 100)*0.1f * i.uv.x % 2;
				
				fixed4 col = tex2D(_MainTex, temp.uv);
				
				

				col.y = _SinTime * _color*col.x;
			    //col = sin(_Time * 100)*0.01f * vertextemp.x % 2;
				//col = sin(_Time * 100)*0.01f * uvtemp.x % 2;

				return col;
			}
			ENDCG
		}
	}
}
