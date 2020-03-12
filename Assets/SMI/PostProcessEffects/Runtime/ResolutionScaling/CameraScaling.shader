Shader "Hidden/CameraScaling"
{
	Properties
	{
		_MainTex	( "MainTex",	2D )	= ""		{}
	}

	Subshader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off Blend Off
			Fog{ Mode off }      

			CGPROGRAM
				#pragma target 3.0
				
				#pragma vertex vert
				#pragma fragment frag
				
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma exclude_renderers d3d11_9x   
				
				#include "UnityCG.cginc"
				
				sampler2D	_MainTex;
		
				struct vertOut 
				{
					float4 pos			: SV_POSITION;
					float2 uv_MainTex	: TEXCOORD0;
				};
				
				vertOut vert( appdata_img v ) 
				{
					vertOut o;
					o.pos			= UnityObjectToClipPos( v.vertex );
					o.uv_MainTex	= v.texcoord.xy;
					return o;
				}

				half4 frag( vertOut IN ) : COLOR
				{
					return tex2D( _MainTex, IN.uv_MainTex );
				}
			ENDCG
		}
	}

	Fallback off
}