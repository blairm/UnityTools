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