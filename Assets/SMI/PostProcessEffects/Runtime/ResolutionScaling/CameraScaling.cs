using UnityEngine;

namespace SMI.PostProcessEffects.Runtime
{
	[ RequireComponent( typeof( Camera ) ) ]
	[ AddComponentMenu( "SMI/PostProcessEffects/Camera/Camera Scaling" ) ]
	public class CameraScaling : MonoBehaviour
	{
		public Shader			shader;
		public CameraScaledRT	cameraScaledRT;

		public void Awake()
		{
			checkResources();
		}

		public void OnEnable()
		{
			if( cameraScaledRT.scale == 1f )
				gameObject.SetActive( false );

			cameraScaledRT.scaledRT.wrapMode	= TextureWrapMode.Clamp;
			cameraScaledRT.scaledRT.filterMode	= FilterMode.Bilinear;
		}
			
		public void OnDisable()
		{
			if( scaleMaterial )
			{
				DestroyImmediate( scaleMaterial );
				scaleMaterial = null;
			}
		}

		public void OnRenderImage( RenderTexture source, RenderTexture destination )
		{
#if UNITY_EDITOR
			checkSupport();
#endif

			if( !isSupported )
			{
				Graphics.Blit( source, destination );
				return;
			}

			Graphics.Blit( cameraScaledRT.scaledRT, destination, scaleMaterial );
		}


		private Material	scaleMaterial;
	
		private bool		isSupported		= true;

		private void checkResources()
		{
			checkSupport();
			scaleMaterial = checkShaderAndCreateMaterial( shader, scaleMaterial );
			
			if( !isSupported )
				reportAutoDisable();		
		}

		private Material checkShaderAndCreateMaterial( Shader s, Material mat )
		{
			if( !s )
			{
				Debug.Log( "Missing shader in " + this.ToString() );
				enabled = false;
				return null;
			}
			
			if( s.isSupported && mat && mat.shader == s )
				return mat;
			
			if( !s.isSupported )
			{
				notSupported();
				Debug.Log( "The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform." );
				return null;
			}
			else
			{
				mat = new Material( s );	
				
				if( mat )
				{
					mat.hideFlags = HideFlags.DontSave;
					return mat;
				}
				
				return null;
			}
		}

		private void checkSupport()
		{
			isSupported = true;
			
			if( !SystemInfo.supportsImageEffects )
				notSupported();
		}

		private void notSupported()					{ enabled = false; isSupported = false; }
		private void reportAutoDisable()			{ Debug.LogWarning( this.ToString() + " has been disabled as it's not supported on the current platform." ); }
	}
}