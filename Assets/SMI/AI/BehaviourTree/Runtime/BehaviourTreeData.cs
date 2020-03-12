using UnityEngine;

using System.Collections.Generic;

namespace SMI.AI.BehaviourTree.Runtime
{
	public class BehaviourTreeData : ScriptableObject
	{
		public List< int >					nodeDepth;
		public List< BehaviourNodeType >	nodeList;
		public List< string >				nodeName;

		public int addChild( BehaviourNodeType type, int parent )
		{
			if( nodeDepth == null )
			{
				nodeDepth	= new List< int >();
				nodeList	= new List< BehaviourNodeType >();
				nodeName	= new List< string >();
			}

			if( parent >= 0 && ( parent >= nodeList.Count || nodeList[ parent ] == BehaviourNodeType.action ) )
			{
				Debug.LogError( "Attempting to add child to action node. " + parent.ToString() );
				return parent;
			}

			int childIndex = -1;

			for( int i = parent + 1; i < nodeList.Count; ++i )
			{
				if( nodeDepth[ i ] <= nodeDepth[ parent ] )
				{
					childIndex = i;
					nodeDepth.Insert( childIndex, nodeDepth[ parent ] + 1 );
					nodeList.Insert( childIndex, type );
					nodeName.Insert( childIndex, "" );
					break;
				}
			}

			if( childIndex < 0 )
			{
				childIndex = nodeDepth.Count;
				nodeDepth.Add( parent >= 0 ? nodeDepth[ parent ] + 1 : 0 );
				nodeList.Add( type );
				nodeName.Add( "" );
			}

			return childIndex;
		}

		public void removeChild( int id )
		{
			for( int i = id + 1; i < nodeList.Count; ++i )
			{
				if( nodeDepth[ i ] > nodeDepth[ id ] )
				{
					nodeDepth.RemoveAt( i );
					nodeList.RemoveAt( i );
					nodeName.RemoveAt( i );
					--i;
				}
				else
				{
					break;
				}
			}

			nodeDepth.RemoveAt( id );
			nodeList.RemoveAt( id );
			nodeName.RemoveAt( id );
		}
	}
}