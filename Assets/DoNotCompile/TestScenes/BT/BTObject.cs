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

using SMI.AI.BehaviourTree.Runtime;

public class BTObject : MonoBehaviour
{
	public NodeData[]	nodeData;

	public void init( BehaviourTree btTree, NodeData[] baseNodeData )
	{
		this.btTree	= btTree;
		nodeData	= new NodeData[ baseNodeData.Length ];
		baseNodeData.CopyTo( nodeData, 0 );
	}

	public void update()
	{
		btTree.tick( this, nodeData );
	}

	static public BehaviourState ActionHasAppeared( MonoBehaviour mb )
	{
		BTObject btObject	= ( BTObject ) mb;
		btObject.count		= 0;

		Debug.Log( "has appeared" );
		return BehaviourState.succeeded;
	}

	static public BehaviourState ActionIsReadyToGo( MonoBehaviour mb )
	{
		BTObject btObject = ( BTObject ) mb;

		if( btObject.count < 100 )
		{
			if( btObject.count == 0 )
				Debug.Log( "is waiting" );

			++btObject.count;
			return BehaviourState.running;
		}

		Debug.Log( "is ready to go" );
		return BehaviourState.succeeded;
	}

	static public BehaviourState ActionHasDisappeared( MonoBehaviour mb )
	{
		Debug.Log( "has disappeared" );
		return BehaviourState.succeeded;
	}

	private int count = 0;

	private BehaviourTree	btTree;
}