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