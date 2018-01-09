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

Shader "SMI/PTG/Ground Space Screen"
{
	Properties
	{
		_GroundMap				( "Ground Map",				2D )					= "white" {}
		_SpecularColour			( "SpecularColour",			Color )					= ( 1, 1, 1, 1 )
		_Shininess				( "Shininess",				Range( 1, 1000 ) )		= 10

		_NormalMap				( "Normal Map",				2D )					= "bump" {}
		_Parallax				( "Height",					Range( 0.005, 0.08 ) )	= 0.02

		_IllumMap				( "Illumination Map",		2D )					= "black" {}
		_CityEmissionStrength	( "City Emission Strength",	Range( 0, 1 ) )			= 0.5

		_CloudMap				( "Cloud Map",				2D )					= "black" {}
		_CloudNormalMap			( "Cloud Normal Map",		2D )					= "bump" {}
		_CloudAlpha				( "Cloud Alpha",			Range( 0, 1.0 ) )		= 1
		
		_InvWavelength			( "InvWavelength",			Vector )				= ( 5.602044746, 9.473284438, 19.64380261 )
		_OuterRadius			( "OuterRadius",			Float )					= 10.25
		_OuterRadius2			( "OuterRadius2",			Float )					= 105.06
		_InnerRadius			( "InnerRadius",			Float )					= 10
		_InnerRadius2			( "InnerRadius2",			Float )					= 100
		_KrESun					( "KrESun",					Float )					= 0.0375
		_KmESun					( "KmESun",					Float )					= 0.015
		_Kr4PI					( "Kr4PI",					Float )					= 0.0314
		_Km4PI					( "Km4PI",					Float )					= 0.01256
		_Scale					( "Scale",					Float )					= 4
		_ScaleDepth				( "ScaleDepth",				Float )					= 0.25
		_ScaleOverScaleDepth	( "ScaleOverScaleDepth",	Float )					= 16
		
		_G						( "G",						Float )					= -0.99
		_G2						( "G2",						Float )					= 0.98
		_hdrExposure			( "HdrExposure",			Float )					= 0.6
	}
	
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		
		Pass
		{
			CGPROGRAM
			    #pragma target 3.0
			    
			    #pragma vertex vert
				#pragma fragment frag
			    
			    #include "UnityCG.cginc"
			    
				sampler2D	_GroundMap;
				fixed4		_SpecularColour;
				float		_Shininess;

				sampler2D	_NormalMap;
				float		_Parallax;

				sampler2D	_IllumMap;
				float		_CityEmissionStrength;

				sampler2D	_CloudMap;
				sampler2D	_CloudNormalMap;
				float		_CloudAlpha;
				
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
					float2 uv_GroundMap	: TEXCOORD0;
					float3 norm			: TEXCOORD1;
					float4 lightDir		: TEXCOORD2;
					float3 eyeDir		: TEXCOORD3;
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
				
				vertOut vert( appdata_tan v )
				{
					float _Samples = 2;
				
					float4 lightDir			= normalize( _WorldSpaceLightPos0 );
				
					float3 camPos			= _WorldSpaceCameraPos;
					
					float _CameraHeight		= length( camPos );
					float _CameraHeight2	= _CameraHeight * _CameraHeight;
					
					float3 Pos				= mul( unity_ObjectToWorld, v.vertex ).xyz;
					float3 worldPos			= Pos;
					float3 Ray				= Pos - camPos;
					Pos						= normalize( Pos );
					float Far				= length( Ray );
					Ray						/= Far;
					
					float Near				= getNearIntersection( camPos, Ray, _CameraHeight2, _OuterRadius2 );
					
					float3 Start			= camPos + Ray * Near;
					Far						-= Near;
					
					float Depth				= exp( ( _InnerRadius - _OuterRadius ) / _ScaleDepth );
					float CameraAngle		= dot( -Ray, Pos );
					float CameraScale		= scale( CameraAngle );
					float CameraOffset		= Depth * CameraScale;
					float LightAngle		= dot( lightDir.xyz, Pos );
					float LightScale		= scale( LightAngle );
					float Temp				= ( LightScale + CameraScale );

					float SampleLength		= Far / _Samples;
					float ScaledLength		= SampleLength * _Scale;
					float3 SampleRay		= Ray * SampleLength;
					float3 SamplePoint		= Start + SampleRay * 0.5;
					
					float3 FrontColor		= float3( 0, 0, 0 );
					float3 Attenuate		= float3( 0, 0, 0 );
					
					for( int i = 0; i < 2; ++i )
					{
						float Height		= length( SamplePoint );
						float Depth			= exp( _ScaleOverScaleDepth * ( _InnerRadius - Height ) );
						float Scatter		= Depth * Temp - CameraOffset;
						Attenuate			= exp( -Scatter * ( _InvWavelength * _Kr4PI + _Km4PI ) );
						FrontColor			+= clamp( Attenuate * ( Depth * ScaledLength ), 0, 1 );
						SamplePoint			+= SampleRay;
					}
					
					//constructs tbn matrix
					TANGENT_SPACE_ROTATION;
					
					vertOut OUT;
	    			OUT.pos					= UnityObjectToClipPos( v.vertex );
	    			OUT.uv_GroundMap		= v.texcoord.xy;
	    			OUT.norm				= normalize( mul( unity_ObjectToWorld, float4( v.normal, 1 ) ) ).xyz;
	    			OUT.lightDir			= normalize( float4( mul( rotation, mul( unity_WorldToObject, lightDir ).xyz ), 0 ) );
	    			OUT.eyeDir				= normalize( mul( rotation, ObjSpaceViewDir( v.vertex ) ) );
					OUT.c0					= FrontColor * ( _InvWavelength * _KrESun + _KmESun );
					OUT.c1					= Attenuate;
					
	    			return OUT;
				}
				
				half4 frag( vertOut IN ) : COLOR
				{
					float3 c			= IN.c0 + 0.25 * IN.c1;
					c					= 1.0 - exp( c * -_hdrExposure );
					

					//do clouds before parallax
					half4 atmos			= tex2D( _CloudMap, IN.uv_GroundMap );
					atmos				*= _CloudAlpha;

					half4 illum			= tex2D( _IllumMap, IN.uv_GroundMap );
					
					float2 offset		= ParallaxOffset( illum.a, _Parallax, IN.eyeDir );
					IN.uv_GroundMap		+= offset;
					
					//redo illumination after getting height - use seperate height map instead?
					illum				= tex2D( _IllumMap, IN.uv_GroundMap );


					//diffuse
					half4 ground		= tex2D( _GroundMap, IN.uv_GroundMap );
					float specAlpha		= ground.a;
					
					float3 n			= UnpackNormal( tex2D( _NormalMap, IN.uv_GroundMap ) );
					float3 l			= IN.lightDir.xyz;
					half diffGround		= max( dot( n, l ), 0.15 );

					ground				*= diffGround;// * max( c.b, 0.15 );
					float3 colour		= ground.rgb + c;

					
					//specular
					float3 h			= normalize( IN.lightDir.xyz + IN.eyeDir );

					float specularlight	= pow( max( dot( n, h ), 0 ), _Shininess ) * specAlpha;
					float3 spec			= _SpecularColour.rgb * specularlight * _SpecularColour.a;

					colour				+= spec;


					//cloud
					n					= UnpackNormal( tex2D( _CloudNormalMap, IN.uv_GroundMap ) );
					l					= IN.lightDir.xyz;
					half diffAtmos		= max( dot( n, l ), 0.2 );

					colour				= 1 - ( ( 1 - colour ) * ( 1 - atmos.rgb ) );
					colour				= clamp( colour, 0, 1 );
					colour				*= diffAtmos;
					colour				= lerp( colour, c, c.b );


					half diff			= max( dot( normalize( IN.norm ), normalize( _WorldSpaceLightPos0 ).xyz ), 0 );
					

					//blue shift
					half invDiff		= 1 - saturate( 10 * diff );
					colour.b			= saturate( colour.b + invDiff * 0.02 );
					

					//city lights
					colour				+= saturate( 1 - 2 * diff ) * illum * _CityEmissionStrength * ( 1 - atmos.a );
					
					return half4( colour, 1 );
				}
			ENDCG
		}
	}
	
	FallBack "SMI/PTG/Planet Parallax Specular Screen"
}