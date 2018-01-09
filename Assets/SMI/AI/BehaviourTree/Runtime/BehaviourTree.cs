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

using System;
using System.Collections.Generic;
using System.Reflection;

namespace SMI.AI.BehaviourTree.Runtime
{
	public enum BehaviourState
	{
		succeeded,
		failed,
		running
	}

	public enum BehaviourNodeType
	{
		none,
		action,
		inverter,
		prioritySelector,
		sequenceSelector
	}

	//agent specific node data
	//[ StructLayout( LayoutKind.Explicit ) ]
	public struct NodeData
	{
		//[ FieldOffset( 0 ) ]					//use if more data is added that is only used by 1 node type - union
		public byte		childrenProcessed;

		public ushort	currRunningChild;		//only used by sequence node
	}

	public delegate BehaviourState Action( MonoBehaviour agent );

	public struct Node
	{
		public BehaviourNodeType	type;
		public ushort				depth;

		public Action				action;			//only used by action node, shared across agents that use this bt
		public byte					childCount;
	}

	public class BehaviourTree
	{
		public NodeData[] buildTree( BehaviourTreeData treeData, Type agent )
		{
			if( treeData.nodeDepth.Count > ( uint ) ushort.MaxValue )
			{
				Debug.LogError( "node counts greater than " + ushort.MaxValue + " not supported." );
				return null;
			}

			NodeData[] nodeData			= new NodeData[ treeData.nodeDepth.Count ];
			nodeArray					= new Node[ treeData.nodeDepth.Count ];
			int depthCount				= 0;

			List< ushort >	parentStack	= new List< ushort >();
			ushort			currDepth	= 0;
			
			for( int i = 0; i < nodeArray.Length; ++i )
			{
				Node node	= nodeArray[ i ];
				node.type	= treeData.nodeList[ i ];
				node.depth	= ( ushort ) treeData.nodeDepth[ i ];
				
				depthCount	= Mathf.Max( depthCount, node.depth + 1 );

				switch( node.type )
				{
					case BehaviourNodeType.action:
						MethodInfo methodInfo = agent.GetMethod( "Action" + treeData.nodeName[ i ], BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy );

						if( methodInfo != null )
							node.action = ( Action ) Delegate.CreateDelegate( typeof( Action ), methodInfo );
						else
							Debug.LogError( "Action callback doesn't exist: Action" + treeData.nodeName[ i ] );
						break;
					case BehaviourNodeType.sequenceSelector:
						nodeData[ i ].currRunningChild = ( ushort ) ( i + 1 );
						break;
				}

				nodeArray[ i ] = node;

				while( node.depth <= currDepth && parentStack.Count > 0 )
				{
					parentStack.RemoveAt( parentStack.Count - 1 );
					--currDepth;
				}

				if( parentStack.Count > 0 )
					++nodeArray[ parentStack[ parentStack.Count - 1 ] ].childCount;

				parentStack.Add( ( ushort ) i );
				currDepth = node.depth;
			}

			nodeStack = new ushort[ depthCount ];
			return nodeData;
		}

		//returns last run action id so you can tick the behaviour tree less often than running an action
		public ushort tick( MonoBehaviour agent, NodeData[] nodeDataArray )
		{
			ushort lastRunAction	= 0;
			ushort stackCount		= 0;
			int currDepth			= 0;

			bool continueBranch		= true;

			BehaviourState result	= BehaviourState.failed;

			ushort nodeCount		= ( ushort ) nodeArray.Length;
			for( ushort i = 0; i < nodeCount; ++i )
			{
				Node node			= nodeArray[ i ];
				NodeData nodeData	= nodeDataArray[ i ];

				//skip child nodes for this branch
				if( !continueBranch )
				{
					if( currDepth < 0 )
						break;
					else if( node.depth > currDepth + 1 )
						continue;
				}

				continueBranch = true;

				switch( node.type )
				{
					case BehaviourNodeType.action:
					{
						result			= node.action( agent );
						lastRunAction	= i;

						ushort childIndex = i;

						//check action result with parent and go as far up the parent stack as necessary
						while( node.depth > currDepth && stackCount > 0 )
						{
							ushort id				= nodeStack[ stackCount - 1 ];
							node					= nodeArray[ id ];
							nodeData				= nodeDataArray[ id ];
							ushort oldStackCount	= stackCount;

							switch( node.type )
							{
								case BehaviourNodeType.inverter:
									if( result == BehaviourState.succeeded )
										result = BehaviourState.failed;
									else if( result == BehaviourState.failed )
										result = BehaviourState.succeeded;

									--stackCount;
									--currDepth;
									break;
								case BehaviourNodeType.prioritySelector:
									//gets reset to zero on next tick when added to the stack
									++nodeData.childrenProcessed;

									if( result != BehaviourState.failed )
									{
										continueBranch = false;

										--stackCount;
										--currDepth;
									}
									else if( nodeData.childrenProcessed >= node.childCount )				//if all children have been processed, go up the stack
									{
										--stackCount;
										--currDepth;
									}

									nodeDataArray[ nodeStack[ oldStackCount - 1 ] ] = nodeData;
									break;
								case BehaviourNodeType.sequenceSelector:
									nodeData.currRunningChild = ( ushort ) ( nodeStack[ oldStackCount - 1 ] + 1 );

									if( result != BehaviourState.succeeded )
									{
										if( result == BehaviourState.running )
											nodeData.currRunningChild = childIndex;

										continueBranch = false;

										--stackCount;
										--currDepth;
									}
									else
									{
										//only want to add to this if child was successful as this node starts
										//at the last running child, gets set to zero on next tick when added
										//to the stack if any child fails
										++nodeData.childrenProcessed;

										if( nodeData.childrenProcessed >= node.childCount )				//if all children have been processed, go up the stack
										{
											--stackCount;
											--currDepth;
										}
									}

									nodeDataArray[ nodeStack[ oldStackCount - 1 ] ] = nodeData;
									break;
							}

							childIndex = nodeStack[ oldStackCount - 1 ];
						}
						break;
					}
					case BehaviourNodeType.sequenceSelector:
					{
						nodeStack[ stackCount ] = i;
						++stackCount;

						currDepth = node.depth;

						//set to zero if any child failed on previous tick
						if( nodeData.currRunningChild - 1 == i )
							nodeData.childrenProcessed = 0;

						nodeDataArray[ i ] = nodeData;

						i = ( ushort ) ( nodeData.currRunningChild - 1 );
						continue;
					}
					default:
					{
						nodeStack[ stackCount ] = i;
						++stackCount;

						currDepth					= node.depth;
						nodeData.childrenProcessed	= 0;
						nodeDataArray[ i ]			= nodeData;
						break;
					}
				}
			}

			return lastRunAction;
		}

		//used for running an action without ticking the behaviour tree
		public BehaviourState tickNode( MonoBehaviour agent, ushort id )
		{
			return nodeArray[ id ].action( agent );
		}


		private Node[]		nodeArray;
		private ushort[]	nodeStack;
	}
}