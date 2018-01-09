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
	public class ObjectPool< T > where T : Object
	{
		public ObjectPool( int size )
		{
			objArray		= new T[ size ];

			unactiveCount	= size;
			unactive		= new int[ size ];

			deactivateAllObjects();
		}

		public int getNewActiveIndex()
		{
			if( unactiveCount > 0 )
			{
				int newIndex = unactive[ --unactiveCount ];
				return newIndex;
			}
			
			return -1;
		}

		public void deactivateObject( int index )
		{
			unactive[ unactiveCount ] = index;
			++unactiveCount;
		}

		public void deactivateAllObjects()
		{
			unactiveCount = objArray.Length;

			//start high so first index retrieved is first object in array
			for( int i = 0; i < unactiveCount; ++i )
				unactive[ i ] = unactiveCount - 1 - i;
		}

		public int getCount()
		{
			return objArray.Length;
		}

		public T this[ int i ]
		{
			get	{ return objArray[ i ]; }
			set	{ objArray[ i ] = value; }
		}
		
		
		private T[]		objArray;

		private int		unactiveCount;
		private int[]	unactive;
	}
}