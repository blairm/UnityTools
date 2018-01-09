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