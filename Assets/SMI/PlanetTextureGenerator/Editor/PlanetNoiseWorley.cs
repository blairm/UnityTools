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

namespace SMI.PlanetTextureGenerator.Editor
{
	public class PlanetNoiseWorley
	{
		public int seed = 0;

		public PlanetNoiseWorley( int seed )
		{
			this.seed = seed;
		}

		public float getOctave( Vector3 pos, int octaves, float amplitude )
		{
			float result	= 0f;
			float scale		= 1f;
		
			for( int i = 0; i < octaves; ++i )
			{
				result	+= noise( pos * scale ) * ( amplitude / scale );
				scale	*= 2f;
			}

			return result;
		}

		
		private float noise( Vector3 pos )
		{
			float result	= float.MaxValue;
			
			int cellX		= Mathf.FloorToInt( pos.x );
			int cellY		= Mathf.FloorToInt( pos.y );
			int cellZ		= Mathf.FloorToInt( pos.z );

			float diffDiv	= 1f / 0xffffffff;

			for( int i = cellX - 1; i <= cellX + 1; ++i )
			{
				for( int j = cellY - 1; j <= cellY + 1; ++j )
				{
					for( int k = cellZ - 1; k <= cellZ + 1; ++k )
					{
						uint hash		= hashFNV( ( uint ) ( i + seed ), ( uint ) j, ( uint ) k );
						uint random		= randomLCG( hash );
						
						uint pointCount	= getPointProbabilityCount( random );
						
						for( uint a = 0; a < pointCount; ++a )
						{
							random			= randomLCG( random );
							float diffX		= random * diffDiv;

							random			= randomLCG( random );
							float diffY		= random * diffDiv;

							random			= randomLCG( random );
							float diffZ		= random * diffDiv;

							Vector3 point	= new Vector3( diffX + i, diffY + j, diffZ + k );
							result			= Mathf.Min( result, Vector3.SqrMagnitude( pos - point ) );
						}
					}
				}
			}

			result = Mathf.Clamp01( result );
			return result;
		}
		
		//Poisson distribution - mean density = 4, max points = 9
		private uint getPointProbabilityCount( uint value )
		{
			if( value < 393325350 )		return 1;
			if( value < 1022645910 )	return 2;
			if( value < 1861739990 )	return 3;
			if( value < 2700834071 )	return 4;
			if( value < 3372109335 )	return 5;
			if( value < 3819626178 )	return 6;
			if( value < 4075350088 )	return 7;
			if( value < 4203212043 )	return 8;
			return 9;
		}
	
		//linear congruential generator - c values
		private uint randomLCG( uint lastValue )
		{
			uint result	= 1103515245u * lastValue + 12345u;
			return result;
		}
		
		private uint hashFNV( uint i, uint j, uint k )
		{
			uint OFFSET_BASIS	= 2166136261;
			uint FNV_PRIME		= 16777619;

			uint result			= OFFSET_BASIS ^ i;
			result				*= FNV_PRIME;
			result				^= j;
			result				*= FNV_PRIME;
			result				^= k;
			result				*= FNV_PRIME;

			return result;
		}
	}
}