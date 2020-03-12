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