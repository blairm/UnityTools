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
	[ RequireComponent( typeof( Camera ) ) ]
	[ AddComponentMenu( "SMI/PostProcessEffects/Camera/Camera Scaled RT" ) ]
	public class CameraScaledRT : MonoBehaviour
	{
		public RenderTexture scaledRT;

		[ Range( 0.1f, 8f ) ]
		public float scale = 1f;

		public void Awake()
		{
			if( scale == 1f )
				enabled = false;

			checkResources();
		}

		public void OnEnable()
		{
			if( isSupported )
			{
				int width	= Screen.width;
				int height	= Screen.height;

				if( width <= 0 && height <= 0 )
				{
					Debug.Log( "screen width &| height reported as 0" );
					width	= 960;
					height	= 540;
				}

				rtFormat			= RenderTextureFormat.Default;
				scaledRT			= new RenderTexture( Mathf.RoundToInt( width * scale ), Mathf.RoundToInt( height * scale ), 24, rtFormat );
				scaledRT.hideFlags	= HideFlags.DontSave;

				if( scaledRT )
					GetComponent< Camera >().targetTexture = scaledRT;
				else
					enabled = false;
			}
		}
			
		public void OnDisable()
		{
			if( scaledRT )
			{
				DestroyImmediate( scaledRT );
				scaledRT = null;
			}

			GetComponent< Camera >().targetTexture = null;
		}


		private RenderTextureFormat	rtFormat;
		private bool				isSupported	= true;

		private void checkResources()
		{
			isSupported = true;

			if( !SystemInfo.supportsImageEffects )
			{
				enabled = false;
				isSupported = false;
				Debug.LogWarning( this.ToString() + " has been disabled as it's not supported on the current platform." );
			}			
		}
	}
}