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

using SMI.AI.BehaviourTree.Runtime;
using SMI.Utils.Editor;

namespace SMI.AI.BehaviourTree.Editor
{
	public class BTEditor : EditorWindow
	{
		static public BehaviourTreeData		activeTreeData;
		static public BehaviourNodeEditor	activeNode;
		static public BehaviourNodeEditor	lastActiveNode;

		[ MenuItem( "Assets/Create/SMI/AI/Behaviour Tree Data" ) ]
		static public void CreateAsset()	{ CustomAssetUtility.CreateAsset< BehaviourTreeData >(); }
	
		[ MenuItem( "SMI/AI/Behaviour Tree/Editor" ) ]
		static public void ShowWindow()		{ EditorWindow.GetWindow< BTEditor >(); }

		public void OnGUI()
		{
			if( Selection.activeObject != null && Selection.activeObject is BehaviourTreeData )
			{
				if( activeTreeData == null || activeTreeData != Selection.activeObject || editorTree.root == null || lastSelectedObject != Selection.activeObject )
				{
					activeTreeData	= Selection.activeObject as BehaviourTreeData;
					activeNode		= null;
					lastActiveNode	= null;
					editorTree.buildTree( activeTreeData );
				}
			}

			lastSelectedObject = Selection.activeObject;

#if true
			//title and asset name
			GUILayout.Label( "Behaviour Tree Editor", EditorStyles.boldLabel );
		
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.PrefixLabel( "Name" );

				if( activeTreeData != null )
					GUILayout.Label( activeTreeData.name, EditorStyles.label );
				else
					GUILayout.Label( "none", EditorStyles.label );
			}
			EditorGUILayout.EndHorizontal();
#endif

			editorTree.OnGUI( new Rect( 0, 50, position.width, position.height - 50 ) );

			if( activeNode != null )
				lastActiveNode = activeNode;

			activeNode = editorTree.clickedNode;

#if true
			//context menu
			Event evt = Event.current;

			if( evt.button == 1 && ( activeNode != null || evt.rawType == EventType.MouseUp ) )
			{
				GenericMenu menu = new GenericMenu();

				if( activeNode == null )
				{
					if( editorTree.root == null )
					{
						AddActionItem( menu );
						menu.AddSeparator( "" );
						AddInverterItem( menu );
						menu.AddSeparator( "" );
						AddPrioritySelectorItem( menu );
						AddSequenceSelectorItem( menu );
					}
				}
				else
				{
					switch( activeNode.type )
					{
						case BehaviourNodeType.action:
							AddDeleteNodeItem( menu );
							break;
						case BehaviourNodeType.inverter:
							AddDeleteNodeItem( menu );
							menu.AddSeparator( "" );
							AddActionItem( menu );
							menu.AddSeparator( "" );
							AddPrioritySelectorItem( menu );
							AddSequenceSelectorItem( menu );
							break;
						case BehaviourNodeType.prioritySelector:
							AddDeleteNodeItem( menu );
							menu.AddSeparator( "" );
							AddActionItem( menu );
							menu.AddSeparator( "" );
							AddInverterItem( menu );
							menu.AddSeparator( "" );
							AddPrioritySelectorItem( menu );
							AddSequenceSelectorItem( menu );
							break;
						case BehaviourNodeType.sequenceSelector:
							AddDeleteNodeItem( menu );
							menu.AddSeparator( "" );
							AddActionItem( menu );
							menu.AddSeparator( "" );
							AddInverterItem( menu );
							menu.AddSeparator( "" );
							AddPrioritySelectorItem( menu );
							break;
					}
				}

				menu.ShowAsContext();
				evt.Use();
			}
#endif

			if( evt.commandName == "UndoRedoPerformed" && activeTreeData != null )
				editorTree.buildTree( activeTreeData );
		}

		public void contextCallback( object obj )
		{
			int nodeID = -1;

			if( activeNode != null )
				nodeID = activeNode.nodeID;
			else if( lastActiveNode != null )
				nodeID = lastActiveNode.nodeID;
		
			switch( ( string ) obj )
			{
				case "addaction":
					Undo.RecordObject( activeTreeData, "Add Action" );
					activeTreeData.addChild( BehaviourNodeType.action, nodeID );
					editorTree.buildTree( activeTreeData );
					EditorUtility.SetDirty( activeTreeData );
					break;
				case "addinverter":
					Undo.RecordObject( activeTreeData, "Add Inverter" );
					activeTreeData.addChild( BehaviourNodeType.inverter, nodeID );
					editorTree.buildTree( activeTreeData );
					EditorUtility.SetDirty( activeTreeData );
					break;
				case "addselectorpriority":
					Undo.RecordObject( activeTreeData, "Add Priority Selector" );
					activeTreeData.addChild( BehaviourNodeType.prioritySelector, nodeID );
					editorTree.buildTree( activeTreeData );
					EditorUtility.SetDirty( activeTreeData );
					break;
				case "addselectorsequence":
					Undo.RecordObject( activeTreeData, "Add Sequence Selector" );
					activeTreeData.addChild( BehaviourNodeType.sequenceSelector, nodeID );
					editorTree.buildTree( activeTreeData );
					EditorUtility.SetDirty( activeTreeData );
					break;
				case "deletenode":
					Undo.RecordObject( activeTreeData, "Delete Node" );
					activeTreeData.removeChild( nodeID );
					editorTree.buildTree( activeTreeData );
					EditorUtility.SetDirty( activeTreeData );
					break;
			}
		}
	
		public void OnInspectorUpdate()	{ Repaint(); }
	
	
		private BehaviourTreeEditor	editorTree			= new BehaviourTreeEditor();

		private UnityEngine.Object	lastSelectedObject;

		private void AddDeleteNodeItem( GenericMenu menu )
		{
			menu.AddItem( new GUIContent( "Delete Node" ), false, contextCallback, "deletenode" );
		}

		private void AddActionItem( GenericMenu menu )
		{
			menu.AddItem( new GUIContent( "Add Action" ), false, contextCallback, "addaction" );
		}

		private void AddInverterItem( GenericMenu menu )
		{
			menu.AddItem( new GUIContent( "Add Inverter" ), false, contextCallback, "addinverter" );
		}

		private void AddPrioritySelectorItem( GenericMenu menu )
		{
			menu.AddItem( new GUIContent( "Add Priority Selector" ), false, contextCallback, "addselectorpriority" );
		}

		private void AddSequenceSelectorItem( GenericMenu menu )
		{
			menu.AddItem( new GUIContent( "Add Sequence Selector" ), false, contextCallback, "addselectorsequence" );
		}
	}
}