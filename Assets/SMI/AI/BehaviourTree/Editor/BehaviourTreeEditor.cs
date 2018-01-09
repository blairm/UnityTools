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

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

using SMI.AI.BehaviourTree.Runtime;

namespace SMI.AI.BehaviourTree.Editor
{
	public class BehaviourNodeEditor
	{
		public BehaviourNodeType	type		= BehaviourNodeType.none;

		public int					nodeID;
		public int					depth;
		public Vector2				position;
	}

	public class BehaviourTreeEditor
	{
		public BehaviourNodeEditor	root;
		public BehaviourNodeEditor	clickedNode;

		public void buildTree( BehaviourTreeData treeData )
		{
			nodeList.Clear();

			dim = Vector2.zero;

			if( treeData.nodeDepth != null && treeData.nodeDepth.Count > 0 )
			{
				Vector2[] leftRight	= new Vector2[ treeData.nodeDepth.Count ];
				int x				= 0;

				int oldDepth		= treeData.nodeDepth[ treeData.nodeDepth.Count - 1 ];
				int deepestDepth	= 0;

				//set type and y position and work out an x value for each node
				for( int i = treeData.nodeDepth.Count - 1; i >= 0 ; --i )
				{
					int depth						= treeData.nodeDepth[ i ];
					deepestDepth					= Mathf.Max( deepestDepth, depth );

					BehaviourNodeEditor editorNode	= new BehaviourNodeEditor();
					editorNode.type					= treeData.nodeList[ i ];
					editorNode.position.x			= x;
					editorNode.position.y			= depth * yGap;

					editorNode.nodeID				= i;
					editorNode.depth				= depth;

					Vector2 position				= leftRight[ depth ];

					if( depth < oldDepth )
					{
						Vector2 childPos		= leftRight[ depth + 1 ];
						editorNode.position.x	= Mathf.Lerp( childPos.x, childPos.y, 0.5f );
						position.x				= x + 1;
					}
					else if( depth > oldDepth )
					{
						for( int j = oldDepth + 1; j < depth; ++j )
							leftRight[ j ] = new Vector2( x, x );

						position = new Vector2( x, x );
						--x;
					}
					else
					{
						position.x = x;
						--x;
					}

					leftRight[ depth ] = position;

					nodeList.Insert( 0, editorNode );					//as we're going backwards through the behaviour tree, we add nodes to the front of the list

					oldDepth = depth;
				}

				//calculate x position
				x		= Mathf.Abs( x + 1 );

				dim.x	= x * ( xGap + nodeWidth );
				dim.y	= deepestDepth * yGap + nodeHeight;

				for( int i = 0; i < nodeList.Count; ++i )
				{
					BehaviourNodeEditor editorNode	= nodeList[ i ];
					editorNode.position.x			+= x;
					editorNode.position.x			*= nodeWidth + xGap;
					nodeList[ i ]					= editorNode;
				}
			}

			root = null;

			if( nodeList.Count > 0 )
				root = nodeList[ 0 ];
		}

		public void OnGUI( Rect position )
		{
			if( nodeList == null )
				return;

			float width		= Mathf.Max( dim.x, position.width );
			scrollPosition	= GUI.BeginScrollView( position, scrollPosition, new Rect( ( width - dim.x ) * -0.5f, 0, width, dim.y ) );

			clickedNode		= null;

			nodeStack.Clear();

			foreach( BehaviourNodeEditor editorNode in nodeList )
			{
#if true
				//render lines
				while( nodeStack.Count > 0 && editorNode.depth <= nodeStack[ nodeStack.Count - 1 ].depth )
					nodeStack.RemoveAt( nodeStack.Count - 1 );

				if( nodeStack.Count > 0 )
				{
					BehaviourNodeEditor parentNode = nodeStack[ nodeStack.Count - 1 ];
					Handles.color = new Color( 0, 0, 0 );
					Handles.DrawLine( new Vector3( parentNode.position.x, parentNode.position.y + nodeHeight ), new Vector3( editorNode.position.x, editorNode.position.y ) );
				}

				nodeStack.Add( editorNode );
#endif

#if true
				//render icons
				Rect rect = new Rect( editorNode.position.x - nodeHalfWidth, editorNode.position.y, nodeWidth, nodeHeight );

				switch( editorNode.type )
				{
					case BehaviourNodeType.action:
						if( GUI.Button( rect, "A!" ) )
							clickedNode = editorNode;
						break;
					case BehaviourNodeType.inverter:
						if( GUI.Button( rect, "IN" ) )
							clickedNode = editorNode;
						break;
					case BehaviourNodeType.prioritySelector:
						if( GUI.Button( rect, "PS" ) )
							clickedNode = editorNode;
						break;
					case BehaviourNodeType.sequenceSelector:
						if( GUI.Button( rect, "SS" ) )
							clickedNode = editorNode;
						break;
				}
#endif
			}

			GUI.EndScrollView();
		}


		private List< BehaviourNodeEditor >	nodeList			= new List< BehaviourNodeEditor >();
		private List< BehaviourNodeEditor >	nodeStack			= new List< BehaviourNodeEditor >();

		private Vector2						dim;
		private Vector2						scrollPosition;

		private const float nodeWidth		= 30;
		private const float nodeHalfWidth	= nodeWidth * 0.5f;
		private const float nodeHeight		= 20;

		private const float xGap			= 20;
		private const float yGap			= 100;
	}
}