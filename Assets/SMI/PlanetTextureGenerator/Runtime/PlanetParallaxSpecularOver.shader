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

Shader "SMI/PTG/Planet Parallax Specular Over"
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
				
				float4		_LightColor0;
			    
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
				
				struct vertOut
				{
					float4 pos			: SV_POSITION;
					float2 uv_GroundMap	: TEXCOORD0;
					float3 norm			: TEXCOORD1;
					float4 lightDir		: TEXCOORD2;
					float3 eyeDir		: TEXCOORD3;
				};
				
				vertOut vert( appdata_tan v )
				{
					float4 lightDir		= normalize( _WorldSpaceLightPos0 );
					
					//constructs tbn matrix
					TANGENT_SPACE_ROTATION;
					
					vertOut OUT;
	    			OUT.pos				= UnityObjectToClipPos( v.vertex );
	    			OUT.uv_GroundMap	= v.texcoord.xy;
	    			OUT.norm			= normalize( mul( unity_ObjectToWorld, float4( v.normal, 1 ) ) ).xyz;
	    			OUT.lightDir		= normalize( float4( mul( rotation, mul( unity_WorldToObject, lightDir ).xyz ), 0 ) );
	    			OUT.eyeDir			= normalize( mul( rotation, ObjSpaceViewDir( v.vertex ) ) );
					
	    			return OUT;
				}
				
				half4 frag( vertOut IN ) : COLOR
				{
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

					float3 colour		= ground.rgb * _LightColor0.rgb * diffGround;


					//specular
					float3 h			= normalize( IN.lightDir.xyz + IN.eyeDir );

					float specularlight	= pow( max( dot( n, h ), 0 ), _Shininess ) * specAlpha;
					float3 spec			= _SpecularColour.rgb * specularlight * _SpecularColour.a;

					colour				+= spec;
					

					//cloud
					n					= UnpackNormal( tex2D( _CloudNormalMap, IN.uv_GroundMap ) );
					l					= IN.lightDir.xyz;
					half diffAtmos		= max( dot( n, l ), 0.2 );

					colour				= lerp( colour, atmos.rgb, atmos.a );
					colour				= clamp( colour, 0, 1 );
					colour				= colour * _LightColor0.rgb * diffAtmos;


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
	
	FallBack "SMI/PTG/Planet Specular Over"
}