Shader "SMI/PostProcessEffects/Heatwave"
{
	Properties
	{
		_NormalMap( "Normal Map", 2D ) = "bump" {}
	}

	Category
	{
		//render after everything else
		Tags{ "Queue" = "Transparent+100" "RenderType" = "Opaque" "LightMode" = "Always" }

		SubShader
		{
			//copies contents of framebuffer into texture called _FrameBuffer
			//should only happen once per frame if this shader is used
			//uses RenderTexture
			GrabPass { "_FrameBuffer" }
	 		
			Pass
			{
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					
					#pragma fragmentoption ARB_precision_hint_fastest
					#pragma fragmentoption ARB_fog_exp2
					
					#include "UnityCG.cginc"

					sampler2D	_FrameBuffer			: register( s0 );
					sampler2D	_NormalMap				: register( s1 );
					
					float4		_FrameBuffer_TexelSize;
					float		_Distortion;

					float4		_NormalMap_ST;
					
					struct vertOut
					{
						float4 vertex			: POSITION;
						float4 uv_FrameBuffer	: TEXCOORD0;
						float2 uv_NormalMap		: TEXCOORD1;
					};

					vertOut vert( appdata_base v )
					{
#if UNITY_UV_STARTS_AT_TOP
						float scale = -1;
#else
						float scale = 1;
#endif
					
						vertOut OUT;
						OUT.vertex				= UnityObjectToClipPos( v.vertex );
						OUT.uv_FrameBuffer.xy	= ( float2( OUT.vertex.x, OUT.vertex.y * scale ) + OUT.vertex.w ) * 0.5;
						OUT.uv_FrameBuffer.zw	= OUT.vertex.zw;
						OUT.uv_NormalMap		= TRANSFORM_TEX( v.texcoord, _NormalMap );
						return OUT;
					}
					
					half4 frag( vertOut IN ) : COLOR
					{
						half2 bump				= UnpackNormal( tex2D( _NormalMap, IN.uv_NormalMap ) ).xy;
						float2 offset			= bump * _Distortion * _FrameBuffer_TexelSize.xy;
						IN.uv_FrameBuffer.xy	= offset * IN.uv_FrameBuffer.w + IN.uv_FrameBuffer.xy;
						
						return tex2Dproj( _FrameBuffer, IN.uv_FrameBuffer );
					}
				ENDCG
			}
		}
	}
}