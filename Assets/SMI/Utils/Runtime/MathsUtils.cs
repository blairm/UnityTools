using UnityEngine;

namespace SMI.Utils.Runtime
{
	public class MathsUtils
	{
		static public Color randomColour() { return new Color( Random.Range( 0f, 1f ), Random.Range( 0f, 1f ), Random.Range( 0f, 1f ) ); }

		static public int ceilToPower2( int x )
		{
			--x;
			x = x | ( x >>  1 );
			x = x | ( x >>  2 );
			x = x | ( x >>  4 );
			x = x | ( x >>  8 );
			x = x | ( x >> 16 );
			
			return ++x;
		}

		static public float continuousAngle( Vector3 direction )
		{
			direction.z = 0;
			float angle = Vector3.Angle( Vector3.up, direction );
	       
			if( angleDirection( direction ) == -1 )
				return 360 - angle;
			else
				return angle;
		}
		
		static public float angleDirection( Vector3 direction )
		{
			Vector3 perpendicular = Vector3.Cross( Vector3.forward, direction );
			
			float dot = Vector3.Dot( perpendicular, Vector3.up );
			
			if( dot > 0 )
				return 1;
			else if( dot < 0 )
				return -1;
			
			return 0;
		}
	}
}