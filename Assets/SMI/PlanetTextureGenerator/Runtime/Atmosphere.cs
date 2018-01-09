/*
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
//*/

using UnityEngine;

namespace SMI.PlanetTextureGenerator.Runtime
{
	[ ExecuteInEditMode ]
	public class Atmosphere : MonoBehaviour
	{
		public float		innerRadius			= 99f;
		public float		outerRadius			= 102.5f;
		public float		outerOffset			= -3.8f;

		public float		Kr					= 0.0025f;
		public float		Km					= 0.001f;
		public float		ESun				= 10f;
		public float		g					= -0.99f;
	
		public float		RayleighScaleDepth	= 0.25f;
		public float		MieScaleDepth		= 0.1f;
	
		public Vector3		wavelength			= new Vector3( 0.65f, 0.57f, 0.475f );

		public float		exposure			= 0.1f;

		public Material		groundMaterial;
		public Material		atmosphereMaterial;

		public void Awake()
		{
			updateMaterials();
		}

#if UNITY_EDITOR
		public void OnValidate()
		{
			innerRadius			= Mathf.Max( innerRadius,	0.1f );
			outerRadius			= Mathf.Max( outerRadius,	0.1f );

			Kr					= Mathf.Max( Kr,			0 );
			Km					= Mathf.Max( Km,			0 );
			ESun				= Mathf.Max( ESun,			0 );
			g					= Mathf.Clamp( g, -0.99f, 0.99f );

			RayleighScaleDepth	= Mathf.Clamp( RayleighScaleDepth, 0f, 1f );
			MieScaleDepth		= Mathf.Clamp( MieScaleDepth, 0f, 1f );

			exposure			= Mathf.Max( exposure,		0.001f );

			updateMaterials();
		}
#endif


		private float	scale;

		private float	Kr4PI;
		private float	Km4PI;

		private Vector4	invWavelength;

		private void setShaderParameters( Material material )
		{
			float scale				= 1f / ( outerRadius - innerRadius );
		
			float Kr4PI				= Kr * 4f * Mathf.PI;
			float Km4PI				= Km * 4f * Mathf.PI;
		
			Vector4	invWavelength	= Vector4.zero;
			invWavelength.x			= 1f / Mathf.Pow( wavelength.x, 4f );
			invWavelength.y			= 1f / Mathf.Pow( wavelength.y, 4f );
			invWavelength.z			= 1f / Mathf.Pow( wavelength.z, 4f );

			material.SetVector( "_InvWavelength",		invWavelength );
			material.SetFloat( "_OuterRadius",			outerRadius + outerOffset );
			material.SetFloat( "_OuterRadius2",			outerRadius * outerRadius );
			material.SetFloat( "_InnerRadius",			innerRadius );
			material.SetFloat( "_InnerRadius2",			innerRadius * innerRadius );
			material.SetFloat( "_KrESun",				Kr * ESun );
			material.SetFloat( "_KmESun",				Km * ESun );
			material.SetFloat( "_Kr4PI",				Kr4PI );
			material.SetFloat( "_Km4PI",				Km4PI );
			material.SetFloat( "_Scale",				scale );
			material.SetFloat( "_ScaleDepth",			RayleighScaleDepth );
			material.SetFloat( "_ScaleOverScaleDepth",	scale / RayleighScaleDepth );

			material.SetFloat( "_G",					g );
			material.SetFloat( "_G2",					g * g );
			material.SetFloat( "_hdrExposure",			exposure );
		}

		private void updateMaterials()
		{
			if( groundMaterial )
				setShaderParameters( groundMaterial );
		
			if( atmosphereMaterial )
				setShaderParameters( atmosphereMaterial );
		}
	}
}