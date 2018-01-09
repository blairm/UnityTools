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

namespace SMI.PostProcessEffects.Runtime
{
	public class HeatwaveImageEffect : MonoBehaviour
	{
		[ HideInInspector ] public bool isActive = false;

		public float		size				= 10f;
		public float		distortion			= 128f;

		public float		duration			= 0.5f;

		public GameObject	effect;

		public void Awake()
		{
			if( isSupported )
				checkResources();

			gameObject.SetActive( false );
		}

		public void Update() 
	    {
			effect.transform.rotation		= Quaternion.FromToRotation( Vector3.back, Camera.main.transform.position - effect.transform.position );
			effect.transform.localPosition	= Vector3.forward * 0.5f;
			
			elapsedTime						+= Time.deltaTime;
			float normalizedTime			= Mathf.Clamp01( elapsedTime / duration );

			float s							= Mathf.Lerp( 0f, size * 10f, normalizedTime );
			
			effectMaterial.SetFloat( "_Distortion", ( 1f - normalizedTime ) * distortion );
			
			effect.transform.localScale		= Vector3.one * s;

			if( elapsedTime > duration )
				gameObject.SetActive( false );
		}

		public void explode()
		{
			if( isSupported )
			{
				elapsedTime					= 0f;
				effect.transform.localScale	= Vector3.zero;
				isActive					= true;
				gameObject.SetActive( true );
			}
		}

		public bool checkResources()
		{
			checkSupport();
			
			if( isSupported )
				effectMaterial = effect.GetComponent< Renderer >().material;
			else
				reportAutoDisable();
			
			return isSupported;			
		}


		private bool		isSupported		= true;

		private float		elapsedTime;

		private Material	effectMaterial;

		private void checkSupport()
		{
			isSupported = true;
			
			if( !SystemInfo.supportsImageEffects )
				notSupported();
		}
		
		private void notSupported()			{ enabled = false; isSupported = false; }
		private void reportAutoDisable()	{ Debug.LogWarning( this.ToString() + " has been disabled as it's not supported on the current platform." ); }
	}
}