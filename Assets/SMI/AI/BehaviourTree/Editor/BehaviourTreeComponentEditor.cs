using UnityEngine;
using UnityEditor;

using SMI.AI.BehaviourTree.Runtime;

namespace SMI.AI.BehaviourTree.Editor
{
	[ CustomEditor( typeof( BehaviourTreeData ) ) ]
	public class BehaviourTreeComponentEditor : UnityEditor.Editor
	{
		public void OnEnable()
		{
			behaviourTree = target as BehaviourTreeData;
		}
		
		override public void OnInspectorGUI()
		{
			BehaviourTreeData activeTreeData = BTEditor.activeTreeData;

			if( activeTreeData != null && behaviourTree != null && activeTreeData.GetInstanceID() == behaviourTree.GetInstanceID() )
			{
				BehaviourNodeEditor node = BTEditor.lastActiveNode;
				
				if( node != null && node.nodeID < behaviourTree.nodeName.Count )
				{
					switch( node.type )
					{
						case BehaviourNodeType.action:
							EditorGUILayout.BeginHorizontal();
							{
								EditorGUILayout.PrefixLabel( "Node ID: " + node.nodeID.ToString() );
							}
							EditorGUILayout.EndHorizontal();

							behaviourTree.nodeName[ node.nodeID ] = EditorGUILayout.TextField( "Node Name", behaviourTree.nodeName[ node.nodeID ], EditorStyles.textField );
							break;
						default:
							DrawDefaultInspector();
							break;
					}
				}
				else
				{
					DrawDefaultInspector();
				}

				if( GUI.changed )
					EditorUtility.SetDirty( behaviourTree );
			}

			Repaint();
		}


		private BehaviourTreeData behaviourTree;
	}
}