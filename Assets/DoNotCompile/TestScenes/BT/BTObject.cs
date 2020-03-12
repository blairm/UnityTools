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