using UnityEngine;

using SMI.AI.BehaviourTree.Runtime;

public class test : MonoBehaviour
{
	public BehaviourTreeData	btTreeData;
	public BehaviourTree		btTree;

	public GameObject			testObject;
	public int					count			= 10;

	public void Awake()
	{
		btTree			= new BehaviourTree();
		baseNodeData	= btTree.buildTree( btTreeData, typeof( BTObject ) );

		btObjectList	= new BTObject[ count ];

		for( int i = 0; i < count; ++i )
		{
			GameObject go		= ( GameObject ) Instantiate( testObject, Vector3.zero, Quaternion.identity );
			btObjectList[ i ]	= go.GetComponent< BTObject >();
			btObjectList[ i ].init( btTree, baseNodeData );
		}
	}

	public void Update()
	{
		for( int i = 0; i < count; ++i )
			btObjectList[ i ].update();
	}


	private NodeData[]	baseNodeData;
	private BTObject[]	btObjectList;
}