//This is free and unencumbered software released into the public domain.
//
//Anyone is free to copy, modify, publish, use, compile, sell, or
//distribute this software, either in source code form or as a compiled
//binary, for any purpose, commercial or non-commercial, and by any
//means.
//
//In jurisdictions that recognize copyright laws, the author or authors
//of this software dedicate any and all copyright interest in the
//software to the public domain. We make this dedication for the benefit
//of the public at large and to the detriment of our heirs and
//successors. We intend this dedication to be an overt act of
//relinquishment in perpetuity of all present and future rights to this
//software under copyright law.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
//OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
//ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.
//
//For more information, please refer to <http://unlicense.org>

Shader "SMI/PTG/Atmosphere Space"
{
	Properties
	{
		_InvWavelength			( "InvWavelength",			Vector )			= ( 5.602044746, 9.473284438, 19.64380261 )
		_OuterRadius			( "OuterRadius",			Float )				= 10.25
		_OuterRadius2			( "OuterRadius2",			Float )				= 105.06
		_InnerRadius			( "InnerRadius",			Float )				= 10
		_InnerRadius2			( "InnerRadius2",			Float )				= 100
		_KrESun					( "KrESun",					Float )				= 0.0375
		_KmESun					( "KmESun",					Float )				= 0.015
		_Kr4PI					( "Kr4PI",					Float )				= 0.0314
		_Km4PI					( "Km4PI",					Float )				= 0.01256
		_Scale					( "Scale",					Float )				= 4
		_ScaleDepth				( "ScaleDepth",				Float )				= 0.25
		_ScaleOverScaleDepth	( "ScaleOverScaleDepth",	Float )				= 16
		
		_G						( "G",						Float )				= -0.99
		_G2						( "G2",						Float )				= 0.98
		_hdrExposure			( "HdrExposure",			Float )				= 0.6
	}
	
	SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha 
			ZWrite Off
			Cull Front
			
			CGPROGRAM
			    #pragma target 3.0
			    
			    #pragma vertex vert
				#pragma fragment frag
			    
			    #include "UnityCG.cginc"
			    
				float3		_InvWavelength;
				float		_OuterRadius;
				float		_OuterRadius2;
				float		_InnerRadius;
				float		_InnerRadius2;
				float		_KrESun;
				float		_KmESun;
				float		_Kr4PI;
				float		_Km4PI;
				float		_Scale;
				float		_ScaleDepth;
				float		_ScaleOverScaleDepth;
				
				float		_G;
				float		_G2;
				float		_hdrExposure;

				struct vertOut
				{
					float4 pos			: SV_POSITION;
					float2 uv_MainTex	: TEXCOORD0;
					float3 dir			: TEXCOORD1;
					float3 c0			: COLOR0;
					float3 c1			: COLOR1;
				};

				float scale( float fCos )
				{
					float x = 1.0 - fCos;
					return _ScaleDepth * exp( -0.00287 + x * ( 0.459 + x * ( 3.83 + x * ( -6.80 + x * 5.25 ) ) ) );
				}

				float getMiePhase( float fCos, float fCos2, float g, float g2 )
				{
					return 1.5 * ( ( 1.0 - g2 ) / ( 2.0 + g2 ) ) * ( 1.0 + fCos2 ) / pow( 1.0 + g2 - 2.0 * g * fCos, 1.5 );
				}

				float getRayleighPhase( float fCos2 )
				{
					return 0.75 + 0.75 * fCos2;
				}

				float getNearIntersection( float3 v3Pos, float3 v3Ray, float fDistance2, float fRadius2 )
				{
					float B		= 2.0 * dot( v3Pos, v3Ray );
					float C		= fDistance2 - fRadius2;
					float fDet	= max( 0.0, B * B - 4.0 * C );
					return 0.5 * ( -B - sqrt( fDet ) );
				}
				
				vertOut vert( appdata_base v )
				{
					float _Samples = 2;
				
					float3 camPos			= _WorldSpaceCameraPos;
					
					float _CameraHeight		= length( camPos );
					float _CameraHeight2	= _CameraHeight * _CameraHeight;
					
					float3 Pos	= mul( unity_ObjectToWorld, v.vertex ).xyz;
					float3 Ray	= Pos - camPos;
					float Far	= length( Ray );
					Ray			/= Far;
					
					float Near			= getNearIntersection( camPos, Ray, _CameraHeight2, _OuterRadius2 );
					
					float3 Start		= camPos + Ray * Near;
					Far					-= Near;
					//float Height		= length( Start );
					float Depth			= exp( -1.0 / _ScaleDepth );//exp( _ScaleOverScaleDepth * ( _InnerRadius - _CameraHeight ) );
					float StartAngle	= dot( Ray, Start ) / _OuterRadius;//Height;
					float StartOffset	= Depth * scale( StartAngle );

					float SampleLength	= Far / _Samples;
					float ScaledLength	= SampleLength * _Scale;
					float3 SampleRay	= Ray * SampleLength;
					float3 SamplePoint	= Start + SampleRay * 0.5;
					
					float3 lightPos0	= normalize( _WorldSpaceLightPos0 ).xyz;
					
					float3 FrontColor = float3( 0, 0, 0 );
					
					for( int i = 0; i < int( _Samples ); ++i )
					{
						float Height		= length( SamplePoint );
						float Depth			= exp( _ScaleOverScaleDepth * ( _InnerRadius - Height ) );
						float LightAngle	= dot( lightPos0, SamplePoint ) / Height;
						float CameraAngle	= dot( Ray, SamplePoint ) / Height;
						float Scatter		= ( StartOffset + Depth * ( scale( LightAngle ) - scale( CameraAngle ) ) );
						float3 Attenuate	= exp( -Scatter * ( _InvWavelength * _Kr4PI + _Km4PI ) );
						FrontColor			+= clamp( Attenuate * ( Depth * ScaledLength ), 0, 20 );
						SamplePoint			+= SampleRay;
					}
					
					vertOut OUT;
	    			OUT.pos			= UnityObjectToClipPos( v.vertex );
	    			OUT.uv_MainTex	= v.texcoord.xy;
	    			
					OUT.c0			= FrontColor * ( _InvWavelength * _KrESun );
					OUT.c1			= FrontColor * _KmESun;
					OUT.dir			= normalize( camPos - Pos );
								
	    			return OUT;
				}
				
				half4 frag( vertOut IN ) : COLOR
				{
					float fCos	= dot( normalize( _WorldSpaceLightPos0 ).xyz, IN.dir ) / length( IN.dir );
					float fCos2	= fCos * fCos;
					
					float3 c	= getRayleighPhase( fCos2 ) * IN.c0 + getMiePhase( fCos, fCos2, _G, _G2 ) * IN.c1;
					c			= 1.0 - exp( c * -_hdrExposure );
					
					return float4( c, c.b );
				}
			ENDCG
		}
	}
	
	FallBack "Transparent/Diffuse"
}